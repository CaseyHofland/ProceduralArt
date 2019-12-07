using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Float : MonoBehaviour
{
    [SerializeField]
    private float airDrag = 1f;
    [SerializeField]
    private float waterDrag = 10f;
    [SerializeField]
    private Transform[] floatingPoints = new Transform[0];

    protected Waves waves;
    protected Rigidbody rigidbody;

    protected float waterLine;
    protected Vector3[] waterLinePoints;

    protected Vector3 centerOffset = Vector3.zero;

    public Vector3 Center => transform.position + centerOffset;

    void Awake()
    {
        waves = FindObjectOfType<Waves>();
        rigidbody = GetComponent<Rigidbody>();

        waterLinePoints = new Vector3[floatingPoints.Length];
        for(int i = 0; i < waterLinePoints.Length; ++i)
            waterLinePoints[i] = floatingPoints[i].position;
        centerOffset = PhysicsHelper.GetCenter(waterLinePoints) - transform.position;
    }

    private void Update()
    {
        // Default water surface
        var newWaterLine = 0f;
        var pointUnderwater = false;

        // Set waterline points and waterline
        for(int i = 0; i < floatingPoints.Length; ++i)
        {
            waterLinePoints[i] = floatingPoints[i].position;
            waterLinePoints[i].y = waves.Height(floatingPoints[i].position);
            newWaterLine += waterLinePoints[i].y / floatingPoints.Length;

            pointUnderwater = (waterLinePoints[i].y > floatingPoints[i].position.y);
        }

        var waterLineDelta = newWaterLine - waterLine;
        waterLine = newWaterLine;

        // Gravity
        var gravity = Physics.gravity;
        rigidbody.drag = airDrag;
        if(waterLine > Center.y)
        {
            rigidbody.drag = waterDrag;

            // Go up
            gravity = -Physics.gravity;
            transform.Translate(Vector3.up * waterLineDelta * 0.9f);
        }
        rigidbody.AddForce(gravity * Mathf.Clamp01(Mathf.Abs(waterLine - Center.y)));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if(floatingPoints == null)
            return;

        for(int i = 0; i < floatingPoints.Length; ++i)
        {
            Transform floatingPoint = floatingPoints[i];
            if(!floatingPoint)
                continue;

            if(waves != null)
            {
                Color lastColor = Gizmos.color;
                Gizmos.color = Color.red;
                Gizmos.DrawCube(waterLinePoints[i], Vector3.one * 0.3f);
                Gizmos.color = lastColor;
            }

            Gizmos.DrawSphere(floatingPoint.position, 0.1f);
        }

        // Draw Center
        if(Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(Center.x, waterLine, Center.z), Vector3.one * 1f);
        }
    }
}
