using UnityEngine;

public class WristRotationDrone : MonoBehaviour
{
    [Header("Quest Ref")]
    [SerializeField] private OVRHand ovrHandRight;
    [SerializeField] private OVRHand ovrHandLeft;

    [Header("Visual Debug")]
    [SerializeField] private LineRenderer droneLine;
    [SerializeField] private LineRenderer rightHandLine;
    [SerializeField] private LineRenderer leftHandLine;
    [SerializeField] private float debugLineLength = 0.5f;

    [Header("Settings")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform droneModel;
    [SerializeField] private float hoverForce = 13f;
    [SerializeField] private float movePower = 15f;
    [SerializeField] private float verticalPower = 20f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float horizontalDeadzone = 5f; // degree de inclinacion minima
    [SerializeField] private float horizontalDeadzoneYaw = 5f; // degree de inclinacion minima
    [SerializeField] private float yawSpeed = 50f; // Velocidad de giro yaw iz der
    [SerializeField] private float yawSensitivity = 3f;
    [SerializeField] private float yawAngleForFullSpeed = 15f;
    [SerializeField] private float yawSmoothing = 10f;
    [SerializeField] private float pitchSensitivity = 1.5f; // forward/back boost
    [SerializeField] private float tiltAngleForFullSpeed = 20f; // degrees to reach max horizontal force
    [SerializeField] private float maxTiltAngle = 20f;
    [SerializeField] private float tiltSpeed = 5f;

    [Header("Audio")]
    [SerializeField] private AudioSource droneAudio;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 2.0f;
    [SerializeField] private float minVolume = 0.3f;
    [SerializeField] private float maxVolume = 1.0f;
    [SerializeField] private float maxAudioSpeed = 5f;

    [Header("State")]
    public bool isOn = true; // Empezamos encendido para testear

    private float currentDroneYaw = 0f;
    private float currentYawRate = 0f;
    private float targetYawRate = 0f;
    private Vector3 horizontalInputForce = Vector3.zero;
    private float verticalInputForce = 0f;
    private float targetModelPitch = 0f;
    private float targetModelRoll = 0f;

    void Start()
    {
        currentDroneYaw = transform.eulerAngles.y;
        droneAudio.Play();
    }

    public void ToggleEngine() => isOn = !isOn;

    void Update()
    {
        UpdateAudio();
        DrawLine(droneLine, transform.position, transform.forward, Color.cyan, true);

        if (!isOn) return;

        // MANO DERECHA (Rotación y Movimiento Horizontal)
        if (ovrHandRight != null && ovrHandRight.IsTracked)
        {
            Vector3 handPos = ovrHandRight.PointerPose.position;
            Quaternion handRot = ovrHandRight.PointerPose.rotation;

            float handPitch = NormalizeAngle(handRot.eulerAngles.x);
            float handRoll  = NormalizeAngle(handRot.eulerAngles.z);

            DrawLine(droneLine, transform.position, transform.forward, Color.green, true);
            DrawLine(rightHandLine, handPos + Vector3.down, handRot * Vector3.forward, Color.red, false);
            CalculateHorizontalMovement(handPitch, handRoll);
        }
        else
        {
            if (rightHandLine) rightHandLine.enabled = false;
            horizontalInputForce = Vector3.zero;
            targetModelPitch = 0f;
            targetModelRoll = 0f;
        }

        // MANO IZQUIERDA (Vertical + Yaw)
        if (ovrHandLeft != null && ovrHandLeft.IsTracked)
        {
            Vector3 handPos = ovrHandLeft.PointerPose.position;
            Quaternion handRot = ovrHandLeft.PointerPose.rotation;

            float handYaw = NormalizeAngle(handRot.eulerAngles.y);
            float dYaw = ApplyDeadzone(handYaw, horizontalDeadzoneYaw);
            float t = Mathf.Clamp01(Mathf.Abs(dYaw) / yawAngleForFullSpeed);
            // solo se calcula el target, integracion va en FixedUpdate para que coincida con el physics step
            targetYawRate = Mathf.Sign(dYaw) * t * yawSensitivity * yawSpeed;

            DrawLine(leftHandLine, handPos + Vector3.down, handRot * Vector3.forward, Color.green, false);
            CalculateVerticalMovement(handRot);
        }
        else
        {
            if (leftHandLine) leftHandLine.enabled = false;
            targetYawRate = 0f;
        }

        // tilt visual del modelo, corre a fps completo
        if (droneModel != null)
        {
            Quaternion targetLocalRot = Quaternion.Euler(targetModelPitch, 0f, targetModelRoll);
            droneModel.localRotation = Quaternion.Slerp(droneModel.localRotation, targetLocalRot, Time.deltaTime * tiltSpeed);
        }
    }

    void FixedUpdate()
    {
        ApplyHover();

        if (!isOn) return;

        // suavizado e integracion en fixed step, asi no hay jitter por mezclar dt variable con fixedDeltaTime
        currentYawRate = Mathf.Lerp(currentYawRate, targetYawRate, Time.fixedDeltaTime * yawSmoothing);
        currentDroneYaw += currentYawRate * Time.fixedDeltaTime;

        Quaternion targetRot = Quaternion.Euler(0f, currentDroneYaw, 0f);
        rb.MoveRotation(targetRot);

        if (horizontalInputForce.magnitude > 0.1f && rb.linearVelocity.magnitude < maxSpeed)
            rb.AddForce(horizontalInputForce * movePower, ForceMode.Acceleration);

        if (Mathf.Abs(verticalInputForce) > 0.1f)
            rb.AddForce(Vector3.up * verticalInputForce * verticalPower, ForceMode.Acceleration);
    }

    private void CalculateHorizontalMovement(float pitch, float roll)
    {
        // direccion basada solo en yaw, sin tilt
        Quaternion yawOnly = Quaternion.Euler(0f, currentDroneYaw, 0f);
        Vector3 forward = yawOnly * Vector3.forward;
        Vector3 right   = yawOnly * Vector3.right;

        Vector3 direction = Vector3.zero;

        float dPitch = ApplyDeadzone(pitch, horizontalDeadzone);
        float dRoll  = ApplyDeadzone(roll,  horizontalDeadzone);

        float tPitch = Mathf.Clamp01(Mathf.Abs(dPitch) / tiltAngleForFullSpeed) * Mathf.Sign(dPitch);

        direction += forward * (tPitch * pitchSensitivity);
        direction += right   * (-dRoll / 45f);

        targetModelPitch = Mathf.Clamp(dPitch, -maxTiltAngle, maxTiltAngle);
        targetModelRoll  = Mathf.Clamp(dRoll,  -maxTiltAngle, maxTiltAngle);

        horizontalInputForce = direction;
    }

    private void CalculateVerticalMovement(Quaternion handRot) //mano iz
    {
        float pitch = NormalizeAngle(handRot.eulerAngles.x);
        float dPitch = ApplyDeadzone(pitch, horizontalDeadzone);
        verticalInputForce = -dPitch / 45f;
    }

    private void ApplyHover() => rb.AddForce(Vector3.up * hoverForce, ForceMode.Acceleration);

    private void UpdateAudio()
    {
        if (!droneAudio.isPlaying) droneAudio.Play();
        float t = Mathf.Clamp01(rb.linearVelocity.magnitude / maxAudioSpeed);
        droneAudio.pitch  = Mathf.Lerp(minPitch,  maxPitch,  t);
        droneAudio.volume = Mathf.Lerp(minVolume, maxVolume, t);
    }

    // returns 0 inside threshold, remaps so output starts from 0 at the boundary
    private float ApplyDeadzone(float value, float threshold)
    {
        if (Mathf.Abs(value) < threshold) return 0f;
        return value - Mathf.Sign(value) * threshold;
    }

    private float NormalizeAngle(float angle)
    {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }

    private void DrawLine(LineRenderer line, Vector3 start, Vector3 direction, Color color, bool worldSpace)
    {
        if (line == null) return;
        line.enabled = true;
        line.useWorldSpace = worldSpace;
        line.positionCount = 2;
        line.startColor = color;
        line.endColor = new Color(color.r, color.g, color.b, 0);
        line.startWidth = 0.01f;
        line.SetPosition(0, start);
        line.SetPosition(1, start + debugLineLength * direction);
    }
}
