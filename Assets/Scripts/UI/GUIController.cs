using BloodMeridiane.Car.Moving;
using System.Collections.Generic;
using UnityEngine;


namespace BloodMeridiane.UI
{
    public class GUIController : MonoBehaviour
    {
        [SerializeField] private List<TahometerController> _tahometers;

        private CarMoveController _car;

        private void Awake()
        {
            var targets = FindObjectsOfType<CarMoveController>();

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].gameObject.activeSelf)
                {
                    _car = targets[i];
                    break;
                }
            }

            if (_car == null)
            {
                Debug.Log("Tahometer not found car!");
                Destroy(this);
            }
        }

        private void Update()
        {
            foreach (var taho in _tahometers)
            {
                taho.UpdateTaho(_car);
            }
        }
    }
}