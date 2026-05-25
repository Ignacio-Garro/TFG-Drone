using UnityEngine;
using UnityEngine.InputSystem;

public class DroneControllerControlsFPV : MonoBehaviour
{
    [Header("Drone Settings")]
    [SerializeField] private float throttleForce = 15f;
    [SerializeField] private float horizontalForce = 8f;   // fwd/back/left/right push
    [SerializeField] private float yawSpeed = 90f;         // degrees per second
    [SerializeField] private float yawSmoothing = 5f;      // ramp speed
    [SerializeField] private float maxTiltAngle = 20f;     // visual tilt cap
    [SerializeField] private float tiltSpeed = 5f;         // how fast it tilts towards target
    [SerializeField] private float angularDamping = 8f;
    [SerializeField] private float deadzone = 0.1f;

    [Header("Audio")]
    [SerializeField] private AudioSource droneAudio;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 2.0f;
    [SerializeField] private float minVolume = 0.3f;
    [SerializeField] private float maxVolume = 1.0f;
    [SerializeField] private float maxAudioSpeed = 5f;

    [SerializeField] private Transform droneModel;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector2 leftStickInput;   // throttle and yaw
    [SerializeField] private Vector2 rightStickInput;  // fwd/back and left/right

    private float currentYaw = 0f;
    private float currentYawRate = 0f;
    private float targetPitch = 0f;
    private float targetRoll = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.angularDamping = angularDamping;
        rb.interpolation = RigidbodyInterpolation.Interpolate; // smooth between physics frames
        currentYaw = transform.eulerAngles.y;
        droneAudio.Play();
    }

    void Update()
    {
        UpdateAudio();
        UpdateModelTilt();
    }

    public void OnLMove(InputValue value)
    {
        Vector2 rawInput = value.Get<Vector2>();
        leftStickInput = ApplyDeadzone(rawInput, deadzone);
        Debug.Log($" L Move: {leftStickInput}");
    }

    public void OnRMove(InputValue value)
    {
        Vector2 rawInput = value.Get<Vector2>();
        rightStickInput = ApplyDeadzone(rawInput, deadzone);
    }

    void FixedUpdate()
    {
        ApplyThrottle();
        ApplyHorizontalMovement();
        ApplyYawAndTilt();
    }

    private void ApplyThrottle()
    {
        float hoverForce = -Physics.gravity.y * rb.mass; // cancel gravity
        float extraForce = leftStickInput.y * throttleForce;
        rb.AddForce(Vector3.up * (hoverForce + extraForce), ForceMode.Force);
    }

    // moves in the yaw only facing direction
    private void ApplyHorizontalMovement()
    {
        Quaternion yawOnly = Quaternion.Euler(0f, currentYaw, 0f);
        Vector3 moveDir = yawOnly * new Vector3(rightStickInput.x, 0f, rightStickInput.y);
        rb.AddForce(moveDir * horizontalForce, ForceMode.Force);
    }

    // yaw on the rigidbody only
    private void ApplyYawAndTilt()
    {
        float targetYawRate = leftStickInput.x * yawSpeed;
        currentYawRate = Mathf.Lerp(currentYawRate, targetYawRate, Time.fixedDeltaTime * yawSmoothing);
        currentYaw += currentYawRate * Time.fixedDeltaTime;
        rb.MoveRotation(Quaternion.Euler(0f, currentYaw, 0f));

        targetPitch = rightStickInput.y * maxTiltAngle;
        targetRoll = -rightStickInput.x * maxTiltAngle;
    }

    private void UpdateModelTilt()
    {
        Quaternion targetLocalRot = Quaternion.Euler(targetPitch, 0f, targetRoll);
        droneModel.localRotation = Quaternion.Slerp(droneModel.localRotation, targetLocalRot, Time.deltaTime * tiltSpeed);
    }

    private void UpdateAudio()
    {
        if (!droneAudio.isPlaying) droneAudio.Play();
        float t = Mathf.Clamp01(rb.linearVelocity.magnitude / maxAudioSpeed);
        droneAudio.pitch  = Mathf.Lerp(minPitch,  maxPitch,  t);
        droneAudio.volume = Mathf.Lerp(minVolume, maxVolume, t);
    }

    public void OnButtonSquare()
    {
        Debug.Log("Square pressed");
    }

    private Vector2 ApplyDeadzone(Vector2 input, float threshold)
    {
        input.x = Mathf.Abs(input.x) < threshold ? 0f : input.x;
        input.y = Mathf.Abs(input.y) < threshold ? 0f : input.y;
        return input;
    }
}
