using BloodMeridiane.Camera;
using UnityEditor;
using UnityEngine;

namespace BloodMeridiane.Guns
{
    [RequireComponent(typeof(Gun))]
    public class Turret : MonoBehaviour
    {
        [SerializeField] private MountingPoint _base;
        [SerializeField] private MountingPoint _barrel;
        [SerializeField] private CameraViewPoint _cameraViewPoint;

        [Space, SerializeField] private bool _isDrawGizmos;

        private Gun _gun;

        private Vector3 Target => _cameraViewPoint.PointPosition;

        #region Draw Gizmos
        private void OnDrawGizmosSelected()
        {
            if (_cameraViewPoint == null || _isDrawGizmos == false)
                return;

            DrawGizmos(_base);
            DrawGizmos(_barrel);
        }

        private void DrawGizmos(MountingPoint mountingPoint)
        {
            var hardpoint = mountingPoint.transform;
            var from = Quaternion.AngleAxis(-mountingPoint.AngleLimit / 2, hardpoint.up) * hardpoint.forward;
            var projection = Vector3.ProjectOnPlane(Target - hardpoint.position, hardpoint.up);

            // projection line
            var dashLineSize = 2f;
            Handles.color = Color.white;
            Handles.DrawDottedLine(Target, hardpoint.position + projection, dashLineSize);

            // do not draw target indicator when out of angle
            if (Vector3.Angle(hardpoint.forward, projection) > mountingPoint.AngleLimit / 2)
                return;

            // target line
            Handles.color = Color.red;
            Handles.DrawLine(hardpoint.position, hardpoint.position + projection);

            // range line
            Handles.color = Color.green;
            Handles.DrawWireArc(hardpoint.position, hardpoint.up, from, mountingPoint.AngleLimit, projection.magnitude);
            Handles.DrawSolidDisc(hardpoint.position + projection, hardpoint.up, .5f);

            // Solid arc
            var range = 10f;
            Handles.color = new Color(0, 1, 0, .2f);
            Handles.DrawSolidArc(hardpoint.position, hardpoint.up, from, mountingPoint.AngleLimit, range);
        }
        #endregion Draw Gizmos

        private void Awake()
        {
            _gun = GetComponent<Gun>();
        }

        private void Update()
        {
            // do nothing when no target
            if (_cameraViewPoint == null)
                return;

            // shoot when aimed
            if (_base.Aim(Target) && _barrel.Aim(Target) 
                && Input.GetAxis("Fire1") > 0)
            {
                _gun.Fire();
            }
        }
    }
}