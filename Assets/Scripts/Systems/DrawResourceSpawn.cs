using UnityEngine;

namespace Systems
{
    public class DrawResourceSpawn : MonoBehaviour
    {
        public bool toggleGizmos = true;
        void OnDrawGizmos()
        {
            if (toggleGizmos)
            {
                if (gameObject.name.Contains("Iron"))
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawCube(transform.position, transform.localScale * 2);
                }
                else if (gameObject.name.Contains("Wood"))
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(transform.position, transform.localScale * 2);
                }
                else if (gameObject.name.Contains("Spice"))
                {

                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(transform.position, transform.localScale * 2);
                }
                else if (gameObject.name.Contains("Gem"))
                {

                    Gizmos.color = Color.blue;
                    Gizmos.DrawCube(transform.position, transform.localScale * 2);
                }
            }
        }
    }
}
