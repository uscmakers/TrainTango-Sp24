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

    private Vector3 startPos;

    public EnvironmentType type = EnvironmentType.Loop;

    public void SetSide(PlayerMovement.PlayerSide side)
    {
        transform.position = new Vector3(side == PlayerMovement.PlayerSide.Left ? sidePos.x : side == PlayerMovement.PlayerSide.Center ? sidePos.y : sidePos.z, transform.position.y, transform.position.z + 75f);
    }

    void Start()
    {
        startPos = transform.position;
        GameManager.instance.movingObjects.Add(this);

        if (isOriginal && type == EnvironmentType.Loop)
        {
            for (int i = 0; i < 20; i++)
            {
                GameObject copy = Instantiate(gameObject);
                copy.GetComponent<MovingObject>().isOriginal = false;
                copy.transform.position += Vector3.forward * transform.localScale.z * (i + 1);
            }
        }
        else if (type == EnvironmentType.Hazard)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + Random.Range(-55f, 55f), transform.rotation.eulerAngles.z);
        }
    }

    public void GameplayUpdate()
    {
        if (type == EnvironmentType.Loop)
        {
            if (startPos.z - transform.position.z > transform.localScale.z)
            {
                transform.position += Vector3.forward * transform.localScale.z;
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
