using UnityEngine;

// количество передач
// тип передач (высокие, низкие, средние)
// макс скорость
[System.Serializable]
public class GearControl
{
    public enum GearType { Neutral, Forward, Rear };

    [SerializeField] private float[] gearRatios = {-10f, 9f, 6f, 4.5f, 3f, 2.5f };

    [SerializeField] private float LimitBackwardSpeed = 60.0f;
    [SerializeField] private float LimitForwardSpeed = 220.0f;

    private int _currentGear;
    private GearType _gearType;

    #region Properties
    /// <summary> BackMoving gear </summary>
    public int RearGear => 0;
    private int NeutralGear => -1;
    public int FirstGear => 1;
    public int MaxGear => gearRatios.Length - 1;

    public bool IsReversingGear => Gear == RearGear;
    public bool IsMaxGear => Gear == MaxGear;
    public bool IsFirstGear => Gear == FirstGear;
    public bool IsNeutralGear => Gear == NeutralGear;
    public bool IsForwardGears => Gear != NeutralGear && Gear != RearGear;

    private bool IsWrongGear => Gear < 0 || Gear >= gearRatios.Length;

    public int VerifiedGear
    {
        get
        {
            switch (_gearType)
            {
                case GearType.Neutral: return FirstGear;
                case GearType.Forward: return FirstGear;
                case GearType.Rear: return RearGear;
                default: return _currentGear;
            }
        }
        set => _currentGear = value; 
    }

    public int Gear
    {
        get => _currentGear;
        set
        {
            _currentGear = value;
            Debug.Log(value);
        }
    }

    public float Ratio
    {
        get
        {
            //Debug.Log(Gear);
            if (IsWrongGear) return gearRatios[FirstGear];
            return gearRatios[Gear];
        }
    }

    public GearType gearType
    {
        get => _gearType;
        set
        {
            _gearType = value;
            switch (value)
            {
                case GearType.Neutral:
                    Gear = NeutralGear;
                    break;
                case GearType.Forward:
                    Gear = FirstGear;
                    break;
                case GearType.Rear:
                    Gear = RearGear;
                    break;
            }
        }
    }
    #endregion

    public float Ratios()
    {
        float result = 0;
        for (int i = 1; i <= VerifiedGear; i++)
        {
            result += gearRatios[i];
        }
        return result;
    }

}
