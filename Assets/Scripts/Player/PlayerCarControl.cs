using BloodMeridiane.Camera;
using BloodMeridiane.Car.Moving;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BloodMeridiane.Player.Control
{
    public static class InputAxis
    {
        public const string Horizontal = nameof(Horizontal);
        public const string Vertical = nameof(Vertical);
        public const string Jump = nameof(Jump);
    }

    [RequireComponent(typeof(CameraTarget))]
    public class PlayerCarControl : MonoBehaviour
    {
        private ICarMoveController _car;
        private Vector3 _startPosition;

        private void Awake()
        {
            _car = GetComponent<ICarMoveController>();
            _startPosition = _car.Transform.position;
        }

        private void Update()
        {
            UpdateVerticalInput();
            UpdateSteerInput();
            UpdateBreakInput();

            UpdateButtons();
        }


        private void UpdateVerticalInput() => _car.ApplyForce(Input.GetAxis(InputAxis.Vertical));
        private void UpdateSteerInput() => _car.ApplySteer(Input.GetAxis(InputAxis.Horizontal));
        private void UpdateBreakInput() => _car.ApplyBreak(Input.GetAxis(InputAxis.Jump));

        private void UpdateButtons()
        {
            if (Input.GetKeyUp(KeyCode.G))
                _car.RestCar();

            else if (Input.GetKeyUp(KeyCode.T))
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            else if (Input.GetKeyUp(KeyCode.R))
                _car.Transform.position = _startPosition;
        }
    }
}