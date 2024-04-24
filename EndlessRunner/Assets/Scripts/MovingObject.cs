using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public enum EnvironmentType
    {
        Loop,
        Single,
        Hazard
    }

    public Vector3 sidePos = new Vector3(-1, 0, 1);

    [HideInInspector]
    public bool isOriginal = true;
    [HideInInspector]
    public bool shouldDestroy = false;

    public int maxLoopCount = 20;
    public float loopOffset = 1f;

    private Vector3 startPos;

    public EnvironmentType type = EnvironmentType.Loop;

    public void SetSide(PlayerMovement.PlayerSide side)
    {
        transform.position = new Vector3((side == PlayerMovement.PlayerSide.Left ? sidePos.x : side == PlayerMovement.PlayerSide.Center ? sidePos.y : sidePos.z) * 4f, transform.position.y, transform.position.z + 75f);
    }

    void Start()
    {
        startPos = transform.position;
        GameManager.instance.movingObjects.Add(this);

        if (isOriginal && type == EnvironmentType.Loop)
        {
            for (int i = 0; i < maxLoopCount; i++)
            {
                GameObject copy = Instantiate(gameObject);
                copy.GetComponent<MovingObject>().isOriginal = false;
                copy.transform.position += PlayerMovement.instance.transform.forward * transform.localScale.z * (i + 1) * loopOffset;
            }
        }
        else if (type == EnvironmentType.Hazard)
        {
            //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + Random.Range(-55f, 55f), transform.rotation.eulerAngles.z);
        }
    }

    public void GameplayUpdate()
    {
        if (type == EnvironmentType.Loop)
        {
            if (startPos.z - transform.position.z > transform.localScale.z * loopOffset)
            {
                transform.position += Vector3.forward * transform.localScale.z * loopOffset;
            }
        }
        else if (type == EnvironmentType.Hazard)
        {
            if (transform.position.z < -10)
            {
                shouldDestroy = true;
            }
        }
    }
}
