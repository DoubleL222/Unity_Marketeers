using UnityEngine;

namespace Systems
{
    public class DrawColliders : MonoBehaviour
    {

        void OnDrawGizmos()
        {
            Matrix4x4 cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

            Gizmos.matrix *= cubeTransform;

            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

            Gizmos.matrix = oldGizmosMatrix;

            /*
        GL.PushMatrix();
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        GL.MultMatrix(rotationMatrix);
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        GL.PopMatrix();
        */
            //Gizmos.color = Color.yellow;
            //Gizmos.DrawCube(transform.position, transform.localScale);
        }
    }
}
