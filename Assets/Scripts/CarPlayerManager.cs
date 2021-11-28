using UnityEngine;

public class CarPlayerManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CarControl car;


    [Header("Visual Parameters")]
    [SerializeField] private float _horizontalInput;
    [SerializeField] private float _verticalInput;
    [SerializeField] private bool _brakeInput;

    private void FixedUpdate()
    {
        CheckInputs();

        car.Steering(_horizontalInput);

        if (!_brakeInput && _verticalInput != 0)
            car.AccelInputs(_verticalInput);
    }

    private void CheckInputs()
    {
        _horizontalInput = Mathf.MoveTowards(_horizontalInput, Input.GetAxis("Horizontal"), 0.2f);
        _verticalInput = Input.GetAxis("Vertical");
        _brakeInput = Input.GetButton("Jump");
    }
}
