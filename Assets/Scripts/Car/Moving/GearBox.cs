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
		[SerializeField, Range(0, 1), Tooltip("Время переключения передачи")] private float _gearShiftingDelay = 0.5f;
		[SerializeField, Range(0, 1), Tooltip("Шанс заклинивания при переключении передачи")] private float _chanceOfJamming = 0.1f;
		[SerializeField, Range(1, 5), Tooltip("Макс увеличение времени переключения при заклинивании")] private float _jammingTimeMultiplied = 3f;
		[SerializeField, Range(0.5f, 1), Tooltip("Когда переключаться на более высокую передачу")] private float _gearShiftUpRpmPercent = 0.64f;
		[SerializeField, Range(0, 0.5f), Tooltip("Когда переключаться на более низкую передачу")] private float _gearShiftDownRpmPercent = 0.35f;

		[Header("Neutral Gear")]
		[SerializeField, Range(0, 5), Tooltip("Граничная скорость нейтральной передачи. Используется для переключения с и на нейтральную скорость")] private float _neutralBorderSpeed = 3f;

		private Gear[] _gears;

		[Tooltip("Переключается сейчас передача?")] private bool _isChangingGear;
		private int _gearShiftUpRPM, _gearShiftDownRPM;
		private int _maxRPM;
		private int _currentSpeed;

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
			_maxRPM = maxRPM;
			CalculateSwitchedRPM();

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

		private void CalculateSwitchedRPM()
        {
			_gearShiftUpRPM = (int)(_maxRPM * _gearShiftUpRpmPercent);
			_gearShiftDownRPM = (int)(_maxRPM * _gearShiftDownRpmPercent);
		}

        private void OnValidate()
        {
			GearName = nameof(GearNames.N);
			CurrentGear = 0;
			InitGearBox(_maxRPM);
        }
        #endregion

        #region Check & Change Gear
        public void CheckGears(float verticalAxis, int currentSpeed, float currentRPM)
        {
			if (_isChangingGear == true) return;

			_currentSpeed = currentSpeed;

			if (ItsYourSpeed(verticalAxis) || ItsYourRPM(currentRPM)) return;

			if(CalculateCurrentGear(verticalAxis) != GearName)
				StartCoroutine(ShiftGear(verticalAxis));
        }

		#region Check Gear Shift
		private bool ItsYourSpeed(float verticalAxis) 
		{
			// проверка макс передачи
			if (CurrentGear == _gears.Length - 1)
				return _currentSpeed > _gears[CurrentGear - 1].TargetSpeedForNextGear;
			// проверка средних передач
			else if (CurrentGear > 0)
				return _currentSpeed <= _gears[CurrentGear].TargetSpeedForNextGear && _currentSpeed > _gears[CurrentGear - 1].TargetSpeedForNextGear;
			// проверка первой и задней передачи
			else
			{
				if (verticalAxis > 0 && GearName == "1")
					return _currentSpeed <= _gears[CurrentGear].TargetSpeedForNextGear;
				else if(verticalAxis < 0 && GearName == "R")
					return Mathf.Abs(_currentSpeed) <= _gears[CurrentGear].MaxSpeed;
            }
			return false;
		}

		private bool ItsYourRPM(float currentRPM)
        {
			// проверка макс передачи
			if (CurrentGear == _gears.Length - 1)
				return currentRPM > _gearShiftDownRPM;
			// проверка средних передач
			else if (CurrentGear > 0)
				return currentRPM > _gearShiftDownRPM && currentRPM < _gearShiftUpRPM;
			// проверка первой и задней передачи
			else
			{
				if (_currentSpeed > 0 && GearName == "1")
					return currentRPM > _gearShiftDownRPM && currentRPM < _gearShiftUpRPM;
				else if(_currentSpeed < 0 && GearName == "R")
					return currentRPM > _gearShiftDownRPM;
			}
			return false;
		}

		private IEnumerator ShiftGear(float verticalAxis)
		{
			_isChangingGear = true;

			var oldGearName = GearName;
			var gearShiftingDelay = _gearShiftingDelay * (oldGearName == nameof(GearNames.N) ? 0.5f : 1f);

			SwitchToNeutral();

			if (Random.value > _chanceOfJamming)
				yield return new WaitForSeconds(gearShiftingDelay);
			else
			{
				// запустить loop звук заклинивания при переключении
				//print("Заклинило АКПП");

				yield return new WaitForSeconds(gearShiftingDelay * Random.Range(1, _jammingTimeMultiplied));

				// выключить loop звук заклинивания при переключении
				//print("АКПП отклинило");
			}

			GearName = CalculateCurrentGear(verticalAxis, out int gear);
			CurrentGear = gear;

			_isChangingGear = false;
		}

		private string CalculateCurrentGear(float verticalAxis, out int gear)
        {
			if(Mathf.Abs(_currentSpeed) < _neutralBorderSpeed)
            {
				gear = 0;
				return nameof(GearNames.N);
            }
            else {
				if (verticalAxis < 0)
				{
					gear = 0;
					return nameof(GearNames.R);
				}
				else
				{
					int gearIndex = 0;

					for (int i = 0; i < _gears.Length; i++)
					{
						gearIndex = i;
						if (_currentSpeed < _gears[i].TargetSpeedForNextGear)
							break;
					}

					gear = gearIndex;
					return (gearIndex + 1).ToString();
				}
			}
        }

		private string CalculateCurrentGear(float verticalAxis) => CalculateCurrentGear(verticalAxis, out int gear);
		#endregion

		private void SwitchToNeutral()
		{
			if (GearName == nameof(GearNames.N)) return;

			GearName = nameof(GearNames.N);
            // здесь звук переключения на нейтралку
            //print($"Переключились на N");
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
