using System.Collections.Generic;
using UnityEngine;

// отображение скорости в км или милях
// добавление газа
// торможение
// реверс
// свет
public class CarControl : MonoBehaviour
{
    [Header("Classes")]
    [SerializeField] public GearControl gear;
    //[SerializeField] public MotorControl motor;

    [Header("Wheels")]
    [SerializeField] private List<WheelControl> acceleratingWheels;
    [SerializeField] private List<WheelControl> swivelWheels;
    [SerializeField] private List<WheelControl> breakWheels;
    [SerializeField] private List<WheelControl> allWheels;

    [Header("Parameters")]
    [SerializeField] private float speedMultiplier = 2.7f;

    [Header("References")]
    [SerializeField] private Rigidbody rb;

    [Header("Reading")]
    private float _currentSpeed;
    private bool _brakeInput;
    [SerializeField] private int Gear;
    [SerializeField] private float _currentRPM;
    [SerializeField] private float _fixedRpm;

    private int _startedRpmInnacuracy;
    private int _nextGearRPM;
    private int _previousGearRpm;
    private float _accelInput;

    #region properties
    private float CurrentMotorRPM
    {
        get => _currentRPM;
        set
        {
            _currentRPM = value;
            //motor.RPM = value;
        }
    }
    #endregion

    private void Start()
    {
        // настройка колес
        // настройка пружин
        // настройка трения колес
        CalculateSwitchingRpm();
        gear.Gear = 1; // потом удалить
    }

    private void FixedUpdate()
    {
        _currentSpeed = rb.velocity.magnitude * speedMultiplier;
        
        // calculating
        ResetParameters();
        CalculateCurrentRpm();

        // проверить физику
        CheckSlip();
        CheckGear();
        Idling();

        // визуальные эффекты
        UpdateWheelsVisual();
        CheckLights();
    }

    #region Update Calculating
    private void ResetParameters()
    {
        Brake(0);   // reset brake
    }

    private void CalculateCurrentRpm()
    {
        _fixedRpm = acceleratingWheels[0].RPM * gear.Ratio / (gear.IsForwardGears ? gear.Gear : 1); // / (_gear != 0 ? _gear : 1);

        CurrentMotorRPM = Mathf.Max(_fixedRpm, 1);// motor.StartEngineRPM);
        //print("acceleratingWheels[0].rpm " + acceleratingWheels[0].RPM);
    }
    #endregion

    #region Update Phisics
    /// <summary> Проверка на занос </summary>
    private void CheckSlip() { }

    /// <summary> Проверка включенной передачи </summary>
    private void CheckGear()
    {
        if (CurrentMotorRPM <= 1)// motor.StartEngineRPM + _startedRpmInnacuracy)
        {
            gear.gearType = GearControl.GearType.Neutral;
            if (_accelInput < 0)
                gear.gearType = GearControl.GearType.Rear;
        }

        if (gear.Gear > 0)
        {
            // если готов переключиться на следующую и эта передача не максимальная
            if (CurrentMotorRPM > _nextGearRPM && !gear.IsMaxGear)
                gear.Gear++;

            // если переключается на пониженную и это не первая и не задняя
            if (CurrentMotorRPM < _previousGearRpm && !gear.IsFirstGear && !gear.IsReversingGear)
                gear.Gear--;
        }
        else if (CurrentMotorRPM > 1)//motor.StartEngineRPM + _startedRpmInnacuracy)
            gear.gearType = GearControl.GearType.Forward;

        if (CurrentMotorRPM < 1)//motor.maxRPM)
        {
            if (gear.IsReversingGear)
            {
                if (_accelInput > 0) Brake();
                if (_accelInput < 0) AddWheelTorque();
            }
            else
            {
                if (_accelInput < 0) Brake();
                if (_accelInput > 0) AddWheelTorque();
            }
        }
        else
        {
            AddWheelTorque(0);
            Brake(0);
        }
    }

    private void Idling()
    {
        if(_accelInput == 0)
        {
            Brake(1);
        }
    }
    #endregion


    private void UpdateWheelsVisual()
    {
        foreach (var wheel in allWheels)
        {
            wheel.UpdateVisual();
        }
    }

    #region Gears
    private void CalculateSwitchingRpm()
    {
        _previousGearRpm = (int)(1);//motor.maxRPM * .3f);
        _nextGearRPM = (int)(1);// motor.maxRPM * .7f);
        _startedRpmInnacuracy = 1;// motor.maxRPM / 10;

        CurrentMotorRPM = 1;// motor.StartEngineRPM;
    }
    #endregion

    private void CheckLights() { }

    #region For CarManager
    public void AccelInputs(float accelInput)
    {
        AddWheelTorqueInpit(accelInput);
    }
    #endregion

    #region WheelControl
    #region Brake
    private void Brake() => Brake(-1);

    private void Brake(float torque)
    {
        foreach (var wheel in breakWheels)
        {
            wheel.Brake(torque);
        }
    }
    #endregion

    #region Add wheel Torgue
    private void AddWheelTorqueInpit(float accelInput) => _accelInput = accelInput;
    private void AddWheelTorque() => AddWheelTorque(_accelInput * gear.Ratios() * 1);// motor.GetTorq());
    private void AddWheelTorque(float torque)
    {
        foreach (var wheel in acceleratingWheels)
        {
            wheel.AddMotorTorque(torque);
        }
    }
    #endregion

    #region Steering
    public void Steering(float steeringInput)
    {
        // Check if ABS(input) > 1
        float checkInput = Mathf.Abs(steeringInput);
        if (checkInput > 1) steeringInput /= checkInput;

        foreach (var wheel in swivelWheels)
        {
            wheel.Steering(steeringInput);
        }
    }
    #endregion
    #endregion
}
