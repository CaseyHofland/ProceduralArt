using UnityEngine;

public class RippleDetector : Singleton<RippleDetector>
{
    private const int waveNumberMax = 128;
    private static readonly int ripplesID = Shader.PropertyToID("_Ripples");

    [Header("Impact Properties")]
    [SerializeField]
    [Tooltip("The maximum amount of impact the wave can receive.")]
    private float maxImpact = float.PositiveInfinity;
    [SerializeField]
    [Tooltip("An impact multiplier. 1 is the normal impact, 2 double the impact and -1 the inverse impact.")]
    private float impactStrength = 1f;
    [Header("Wave Properties")]
    [SerializeField] [Range(0, 0.999999f)]
    [Tooltip("Determines how much the waves fade with each update step.")]
    private float persistence = 0.98f;
    [SerializeField]
    [Tooltip("The speed at which the wave spreads out.")]
    private float spreadSpeed = 1f;
    [SerializeField]
    [Tooltip("Stop updating the wave after its amplitude has reached the deadzone.")]
    private float deadzone = 0.05f;

    private int waveIndex = 0;

    private void Awake()
    {
        Shader.SetGlobalVectorArray(ripplesID, new Vector4[waveNumberMax]);
    }

#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        Shader.SetGlobalVectorArray(ripplesID, new Vector4[waveNumberMax]);
    }
#endif

    private void Update()
    {
        // Update Shader Amplitudes
        var ripples = Shader.GetGlobalVectorArray(ripplesID);

        for(int i = 0; i < ripples.Length; i++)
        {
            var amplitude = ripples[i].z;
            if(Mathf.Approximately(amplitude, 0))
                continue;

            amplitude *= persistence;
            amplitude = (amplitude > deadzone) ? amplitude : 0;

            ripples[i].z = amplitude;
            ripples[i].w += spreadSpeed;
        }

        Shader.SetGlobalVectorArray(ripplesID, ripples);
    }

    private void OnTriggerEnter(Collider other)
    {
        float waveImpact = other.attachedRigidbody.velocity.magnitude;

        Vector2 position = new Vector2(other.transform.position.x, other.transform.position.z);
        RippleAt(position, waveImpact);
    }

    private void OnCollisionEnter(Collision collision)
    {
        float waveImpact = collision.impulse.magnitude;

        foreach(var contact in collision.contacts)
        {
            Vector2 position = new Vector2(contact.point.x, contact.point.z);
            RippleAt(position, waveImpact);
        }
    }

    public void RippleAt(Vector2 position, float force)
    {
        force = Mathf.Clamp(Mathf.LerpUnclamped(0f, force, impactStrength), -maxImpact, maxImpact);

        var ripples = Shader.GetGlobalVectorArray(ripplesID);
        ripples[waveIndex].x = position.x;
        ripples[waveIndex].y = position.y;
        ripples[waveIndex].z = force;
        ripples[waveIndex].w = 0;
        Shader.SetGlobalVectorArray(ripplesID, ripples);

        waveIndex = (waveIndex + 1) % ripples.Length;
    }
}
