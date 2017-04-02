using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform targetPlayer;
    public float smoothing = 5f;

    Vector3 offset;

    void Start()
    {
        offset = transform.position - targetPlayer.position;
    }

    void FixedUpdate()
    {
        Vector3 targetCameraPos = targetPlayer.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCameraPos, smoothing*Time.deltaTime);
    }
}
