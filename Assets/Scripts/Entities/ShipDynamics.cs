using UnityEngine;

namespace Entities
{
    public class ShipDynamics : MonoBehaviour
    {

        public float occilationSpeedX;
        public float occilationSpeedY;
        public float occilationSpeedZ;

        void Start()
        {
            transform.Rotate(-2.0f, 0, -2.0f);
        }

        void Update()
        {
            transform.Rotate((Mathf.Sin(Time.time) * occilationSpeedX), (Mathf.Sin(Time.time) * occilationSpeedY), (Mathf.Sin(Time.time) * occilationSpeedZ));
        }
    }
}
