using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomOut : MonoBehaviour
{
    const float maxOutzoom = 180f;

    [SerializeField]
    private float speed = 1f;

    void Update()
    {
        if(transform.position.y < maxOutzoom)
            transform.Translate(Vector3.back * Time.deltaTime * speed);
    }
}
