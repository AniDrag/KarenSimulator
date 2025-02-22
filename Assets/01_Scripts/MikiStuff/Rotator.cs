using UnityEngine; 

namespace UnityFundamentals
{
    public class Rotator : MonoBehaviour 
    {
        public float speed = 50f;
        [SerializeField] private Vector3 rotationAxis = Vector3.up;
        private void Update()
        {
            transform.Rotate(rotationAxis, speed * Time.deltaTime);
        }
    }
}
