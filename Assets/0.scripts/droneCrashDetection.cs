using UnityEngine;

public class droneCrashDetection : MonoBehaviour
{
    public float crashSpeedThreshold = 2.0f;

    public LayerMask crashLayers = ~0;

    public float ignoreCollisionsOnStart = 0.5f;

    public bool HasCrashed { get; private set; }

    private Rigidbody droneRigidbody;
    private float spawnTime;

    void Awake()
    {
        droneRigidbody = GetComponent<Rigidbody>();
        spawnTime = Time.time;
    }

    // checks speed and layer
    private bool IsCrashImpact(Collision collision)
    {
        if (Time.time - spawnTime < ignoreCollisionsOnStart) return false;
        if ((crashLayers.value & (1 << collision.gameObject.layer)) == 0) return false;
        return collision.relativeVelocity.magnitude >= crashSpeedThreshold;
    }

    // only 1 collision even when multiple hits
    void OnCollisionEnter(Collision collision)
    {
        if (HasCrashed) return;
        if (!IsCrashImpact(collision)) return;

        HandleCrash(collision.gameObject, collision.GetContact(0).point, collision.relativeVelocity.magnitude);
    }

    // logs the crash and freezes the drone
    private void HandleCrash(GameObject hitObject, Vector3 hitPoint, float impactSpeed)
    {
        HasCrashed = true;
        Debug.Log($"Drone crashed into {hitObject.name} at {hitPoint} (impact {impactSpeed:F2} m/s)");

        droneRigidbody.linearVelocity = Vector3.zero;
        droneRigidbody.angularVelocity = Vector3.zero;
    }

    // call this from respawn logic
    public void ResetCrashState()
    {
        HasCrashed = false;
        spawnTime = Time.time;
    }
}
