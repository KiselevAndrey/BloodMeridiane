using UnityEngine;

// скольжение переднее и боковое
// трение
public class WheelControl : MonoBehaviour
{
    [SerializeField] private WheelCollider wheelCollider;
    [SerializeField] private float brakingForce = 1000;
    [SerializeField, Min(0)] private float maxSteerAngle = 45f;

    public float RPM { get => wheelCollider.rpm; }

    public void UpdateVisual()
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion quat);
        transform.position = pos;
        transform.rotation = quat;
    }

    #region Brake
    private void Brake() => Brake(brakingForce);

    public void Brake(float torque = -1)
    {
        if (torque > -1) wheelCollider.brakeTorque = torque;
        else Brake();
    }
    #endregion

    #region Add Torque
    public void AddMotorTorque(float torque) => wheelCollider.motorTorque = torque;
    #endregion

    #region Steering
    public void Steering(float steeringInput) => wheelCollider.steerAngle = steeringInput * maxSteerAngle;
    #endregion
}
