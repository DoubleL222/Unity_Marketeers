using UnityEngine;

namespace World
{
    public class RotateMill : MonoBehaviour {
        void LateUpdate()
        {
            transform.Rotate(Vector3.back * Time.deltaTime * 10);
        }
    }
}
