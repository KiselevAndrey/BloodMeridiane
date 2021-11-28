using KAP.Extension;
using System.Collections;
using UnityEngine;

namespace BloodMeridiane.Car.Moving
{
	public enum GearNames { R, N }

	public class GearBox : MonoBehaviour
	{
		[SerializeField, Range(1, 8)] private int _totalGears = 4;
		[SerializeField, Min (1)] private int _maxSpeed = 50;
		[SerializeField, Min(1)] private float _finalGearRatio = 3.23f;

		[Header("Gear Shift")]
		[SerializeField, Range(.25f, 1), Tooltip("На каком соотношении скоростей в передачи АПК попытается переключиться на более высокую передачу")] private float _gearShiftingThreshold = .75f;
		[SerializeField, Range(0, 5), Tooltip("Время переключения передачи")] private float _gearShiftingDelay = 3.5f;
		[SerializeField, Range(0, 1), Tooltip("Шанс заклинивания при переключении передачи")] private float _chanceOfJamming = 0.1f;
		[SerializeField, Range(1, 5), Tooltip("Макс увеличение времени переключения при заклинивании")] private float _jammingTimeMultiplied = 3f;

		[Header("Neutral Gear")]
		[SerializeField, Range(0, 5), Tooltip("Граничная скорость нейтральной передачи. Используется для переключения с и на нейтральную скорость")] private float _neutralBorderSpeed = 3f;
		[SerializeField, Range(0, 5), Tooltip("Время через которое можно опять переключиться на нейтральную передачу. Требуется для слабых моторов, чтобы не перескакивали с первой и реверса на нейтралку и обратно при старте авто")]
		private float _timeToSwitchToNeutral = 5f;

		private Gear[] _gears;
		private Coroutine _switchToNeutralCooldown;

		[Tooltip("Переключается сейчас передача?")] private bool _isChangingGear;
		private float _oldVerticalAxis;
		private float _currentSpeed;
		[Tooltip("Можно ли переключиться обратно на нейтральную передачу. Используется после переключения на первую и на реверс")] private bool _canSwitchBackToNeutral;
		private int _gearShiftUpRPM, _gearShiftDownRPM;

		#region Properties
		public int MaxSpeed => _maxSpeed;
		public Gear Gear => _gears[CurrentGear];
		public int CurrentGear { get; private set; }
		public bool Revers { get; private set; }
		public string GearName { get; private set; }
		/// <summary> Возвращает 0 или 1 в зависимости от: "переключается ли сейчас передача" и соотношении текущей скорости от максимальной на этой передачи </summary>
		public int GearMultiplier => (_isChangingGear == false && Mathf.Abs(_currentSpeed) <= Gear.MaxSpeed).ToInt();
        #endregion

        #region Init
        public void InitGearBox(int maxRPM)
        {
			InitGears();
			CalculateSwitchedRPM(maxRPM);

			GearName = nameof(GearNames.N);
		}

		private void InitGears()
		{
			_gears = new Gear[_totalGears];

			float[] gearRatio = CalculateGearRatio();

			for (int i = 0; i < _gears.Length; i++)
			{
				int maxSpeedForGear = _maxSpeed / _gears.Length * (i + 1);
				int targetSpeedForGear = (int)Mathf.Lerp(0, _maxSpeed * _gearShiftingThreshold, (float)(i + 1) / _gears.Length);

				_gears[i] = new Gear();
				float finalGearRatio = _finalGearRatio / gearRatio[gearRatio.Length - 1];
				_gears[i].SetGear(gearRatio[i] * finalGearRatio, maxSpeedForGear, targetSpeedForGear);
			}
		}

		private float[] CalculateGearRatio()
		{
			float[] gearRatio = new float[_totalGears];

			if (_totalGears == 1)
				gearRatio = new float[] { 1.0f };

			if (_totalGears == 2)
				gearRatio = new float[] { 2.0f, 1.0f };

			if (_totalGears == 3)
				gearRatio = new float[] { 2.0f, 1.5f, 1.0f };

			if (_totalGears == 4)
				gearRatio = new float[] { 2.86f, 1.62f, 1.0f, .72f };

			if (_totalGears == 5)
				gearRatio = new float[] { 4.23f, 2.52f, 1.66f, 1.22f, 1.0f, };

			if (_totalGears == 6)
				gearRatio = new float[] { 4.35f, 2.5f, 1.66f, 1.23f, 1.0f, .85f };

			if (_totalGears == 7)
				gearRatio = new float[] { 4.5f, 2.5f, 1.66f, 1.23f, 1.0f, .9f, .8f };

			if (_totalGears == 8)
				gearRatio = new float[] { 4.6f, 2.5f, 1.86f, 1.43f, 1.23f, 1.05f, .9f, .72f };

			return gearRatio;
		}

		public void CalculateSwitchedRPM(int maxRPM)
        {
			// числа взяты научным методом
			_gearShiftUpRPM = (int)(maxRPM * .64f);
			_gearShiftDownRPM = (int)(maxRPM * .35f);
		}
        #endregion

        #region Check & Change Gear
        public void CheckGears(float verticalAxis, float currentSpeed, float currentRPM)
        {
			if (_isChangingGear == true) return;

			_currentSpeed = currentSpeed;

			// если ускоряемся
			if (verticalAxis > 0)
			{
				CheckGearShiftFirst();
				CheckGearShiftUp(currentRPM);
				CheckGearShiftNeutral();
			}

			// если тормозим
			else if(verticalAxis < 0)
            {
				CheckGearShiftReverce();
				CheckGearShiftNeutral();
			}

			// если ничего не делаем
			else
			{
				CheckGearShiftNeutral();
			}

			CheckGearShiftDown(currentRPM);

			if (CurrentGear == 0 && GearName != nameof(GearNames.N)					// если первая или реверс
				&& _canSwitchBackToNeutral == false									// и переключиться на нейтралку нельзя
				&& _oldVerticalAxis.ToString()[0] != verticalAxis.ToString()[0])    // первый знак(число) у старой и новой скорости не совпадает
            {
				StopCoroutine(_switchToNeutralCooldown);
				_switchToNeutralCooldown = null;
				_canSwitchBackToNeutral = true;
            }				

			_oldVerticalAxis = verticalAxis;
        }

		#region Check Gear Shift
		private void CheckGearShiftUp(float currentRPM)
		{
			if (CurrentGear < _gears.Length - 1                                 // если не макс передача
				&& _currentSpeed >= _gears[CurrentGear].TargetSpeedForNextGear  // и скорость больше скорости переключения передачи
				&& Revers == false
				&& currentRPM >= _gearShiftUpRPM)
			{
				StartCoroutine(ChangeGear(CurrentGear + 1));
			}
		}

		private void CheckGearShiftDown(float currentRPM)
		{
			if (CurrentGear > 0                                                     // если не первая передача
				&& _currentSpeed < _gears[CurrentGear - 1].TargetSpeedForNextGear   // и скорость меньше скорости переключения пониженной передачи
				&& Revers == false
				)//&& currentRPM <= _gearShiftDownRPM)
			{
				StartCoroutine(ChangeGear(CurrentGear - 1));
			}
		}

		private void CheckGearShiftReverce()
		{
			if (CurrentGear == 0 && _currentSpeed < -_neutralBorderSpeed && GearName == nameof(GearNames.N))
			{
				StartCoroutine(ChangeGear(-1));     // включаем реверс
				Revers = true;

				if (_switchToNeutralCooldown == null)
					_switchToNeutralCooldown = StartCoroutine(SwitchToNeutralCooldown());
			}
		}

		private void CheckGearShiftFirst()
		{
			if (CurrentGear == 0 && _currentSpeed > _neutralBorderSpeed && GearName == nameof(GearNames.N))
			{
				StartCoroutine(ChangeGear(0));     // включаем первую передачу

				if (_switchToNeutralCooldown == null)
					_switchToNeutralCooldown = StartCoroutine(SwitchToNeutralCooldown());
			}
		}

		private void CheckGearShiftNeutral()
		{
			if (CurrentGear == 0 
				&& _canSwitchBackToNeutral
				&& ((_currentSpeed < _neutralBorderSpeed && GearName == "1")
				|| (_currentSpeed > -_neutralBorderSpeed && GearName == nameof(GearNames.R))))
			{
				SwitchToNeutral();
				Revers = false;
			}
		}
		#endregion

		/// <summary> Переключение передачи </summary>
		private IEnumerator ChangeGear(int gear)
		{
			_isChangingGear = true;

			SwitchToNeutral();

			if (Random.value > _chanceOfJamming)
				yield return new WaitForSeconds(_gearShiftingDelay);
			else
			{
				// запустить loop звук заклинивания при переключении
				//print("Заклинило АКПП");

				yield return new WaitForSeconds(_gearShiftingDelay * Random.Range(1,_jammingTimeMultiplied));

				// выключить loop звук заклинивания при переключении
				//print("АКПП отклинило");
			}

			// вкл звук включения передачи

			if (gear == -1)
			{
				CurrentGear = 0;
				GearName = nameof(GearNames.R);
			}
			else
			{
				CurrentGear = gear;
				GearName = (gear + 1).ToString();
			}

			//print($"Переключились на {GearName}");
			_isChangingGear = false;
		}

		private void SwitchToNeutral()
		{
			if (GearName == nameof(GearNames.N)) return;

			GearName = nameof(GearNames.N);
			// здесь звук переключения на нейтралку
			//print($"Переключились на N");
		}

		private IEnumerator SwitchToNeutralCooldown()
        {
			_canSwitchBackToNeutral = false;
			yield return new WaitForSeconds(_timeToSwitchToNeutral);
			_canSwitchBackToNeutral = true;
			_switchToNeutralCooldown = null;
		}
		#endregion
	}


	[System.Serializable]
	public class Gear
	{
		public float Ratio;
		public int MaxSpeed;
		public int TargetSpeedForNextGear;

		public void SetGear(float ratio, int speed, int targetSpeed)
		{
			Ratio = ratio;
			MaxSpeed = speed;
			TargetSpeedForNextGear = targetSpeed;
		}
	}
}
