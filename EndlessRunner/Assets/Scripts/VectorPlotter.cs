using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorPlotter : MonoBehaviour
{
    public MovementDetect detect;
    public float magnitude = 1000f;
    private void OnDrawGizmos()
    {
        if (detect == null)
            return;

        // draw detect.gyroscope
        Gizmos.color = Color.blue;
        Vector3 jump = detect.actionThresholds["Jump"].normDirection;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * detect.mAcceleration.z / magnitude);
        Gizmos.DrawSphere(transform.position + jump, 0.1f);

        Gizmos.color = Color.red;
        Vector3 left = detect.actionThresholds["Left"].normDirection;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * (detect.mAcceleration.x / magnitude));
        Gizmos.DrawSphere(transform.position + left, 0.1f);

        // Gizmos.color = Color.green;
        // Vector3 right = detect.actionThresholds["Right"].normDirection;
        // Gizmos.DrawLine(transform.position, transform.position + transform.right * (detect.mAcceleration.x / magnitude));
        // Gizmos.DrawSphere(transform.position + right, 0.1f);
    }
}
