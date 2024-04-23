using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    static List<MovingObject> movingObjects = new List<MovingObject>();

    private void Awake()
    {
        movingObjects.Add(this);
    }

    private void OnDestroy()
    {
        movingObjects.Remove(this);
    }

    public static void MoveAll(Vector3 move)
    {
        foreach (var objRb in movingObjects)
        {
            objRb.transform.position += move;
        }
    }
}
