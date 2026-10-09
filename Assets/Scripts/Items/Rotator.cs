using UnityEngine;

namespace Items
{
    public enum RotateAxis
    {
        XAxis,
        YAxis,
        ZAxis
    }
    
    public class ObjectRotator : MonoBehaviour
    {
        [SerializeField] private RotateAxis rotateAxis = RotateAxis.ZAxis;
        [SerializeField] private float rotationAngle = 45f;
        [SerializeField] private bool isRotating = true;

        private Vector3 _rotation;

        private void Awake()
        {
            _rotation = rotateAxis switch
            {
                RotateAxis.XAxis => new Vector3(rotationAngle, 0f, 0f),
                RotateAxis.YAxis => new Vector3(0f, rotationAngle, 0f),
                RotateAxis.ZAxis => new Vector3(0f, 0f, rotationAngle),
                _ => _rotation
            };
        }

        private void Update()
        {
            if (isRotating)
            {
                transform.Rotate(_rotation * Time.deltaTime, Space.Self);
            }
        }
    }
}