using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private float cameraOffsetX;
    private float cameraOffsetY;
    private float targetY;
    private bool targetReached = true;

    // Use this for initialization
    void Start()
    {
        //calculate camera offsets against player position
        cameraOffsetX = transform.position.x - player.transform.position.x;
        cameraOffsetY = transform.position.y - player.transform.position.y;
    }

    void LateUpdate()
    {
        // check verical position of camera, and move when player is getting out of view
        cameraOffsetY = transform.position.y - player.transform.position.y;

        // check if vertical position of camera gets out of specified range
        if (cameraOffsetY < -0.8f || cameraOffsetY > 1.2f)
        {
            // sets new vertical target
            targetY = transform.position.y - cameraOffsetY;
            targetReached = false;
        }
        else
        {
            // if previous vertical target was reached, set new target to current camera position
            if (targetReached)
            {
                targetY = transform.position.y;
            } 
        }

        // slowly moving camera to target vertical position based on relative position of camera vs target, also horizontally follow player
        if(targetY > transform.position.y)
        {
            transform.position = new Vector3(player.transform.position.x + cameraOffsetX, transform.position.y + 0.0035f, transform.position.z);
        }
        else if (targetY < transform.position.y)
        {
            transform.position = new Vector3(player.transform.position.x + cameraOffsetX, transform.position.y - 0.0035f, transform.position.z);
        }
        // if current position is vertical target, only move horizontally
        else
        {
            transform.position = new Vector3(player.transform.position.x + cameraOffsetX, transform.position.y, transform.position.z);
        }

        // checking if camera reached vertical target with some allowed deviation
        if (targetY - transform.position.y < 0.1f)
        {
            targetReached = true;
        }

    }
}
