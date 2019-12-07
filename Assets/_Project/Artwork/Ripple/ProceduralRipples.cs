using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralRipples : MonoBehaviour
{
    [SerializeField]
    private Camera camera = null;
    [SerializeField]
    private AudioPeer audioPeer = null;
    [SerializeField]
    private float forceMultiplier = 1f;
    [SerializeField]
    private Vector2 viewportRange = new Vector2(0.9f, 0.9f);
    [SerializeField]
    private float amplitudeChangeThreshold = 0.1f;
    [SerializeField]
    private float beatOffset = 1f / 8f;

    private RippleDetector rippleDetector;

    private float currentOffset = 0f;
    private float lastAmplitude = 0f;

    private void Start()
    {
        rippleDetector = GetComponent<RippleDetector>();
    }

    private void Update()
    {
        if(currentOffset > 0)
        {
            currentOffset = Mathf.Max(currentOffset - Time.deltaTime, 0);
            return;
        }

        float amplitude = audioPeer._Amplitude;
        if(amplitude - lastAmplitude > amplitudeChangeThreshold)
        {
            Ripple(amplitude);
            currentOffset = beatOffset;
        }

        lastAmplitude = amplitude;
    }

    private void Ripple(float force)
    {
        force = 1f;

        // Get viewport point
        Vector2 viewportPoint = RandomViewportPoint;
        viewportPoint.x = Mathf.Lerp(1f - viewportRange.x, viewportRange.x, viewportPoint.x);
        viewportPoint.y = Mathf.Lerp(1f - viewportRange.y, viewportRange.y, viewportPoint.y);

        // Get world point from viewport point
        Vector3 worldPoint = Vector3.zero;
        Ray cameraRay = camera.ViewportPointToRay(new Vector3(viewportPoint.x, viewportPoint.y, 0));
        if(Physics.Raycast(cameraRay, out RaycastHit hit))
            worldPoint = hit.point;

        // Let 'er Rip!
        rippleDetector.RippleAt(new Vector2(worldPoint.x, worldPoint.z), force * forceMultiplier);
    }

    Vector2 RandomViewportPoint => new Vector2(Random.value, Random.value);
}
