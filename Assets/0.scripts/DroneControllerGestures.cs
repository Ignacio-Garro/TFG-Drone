using UnityEngine;

public class DroneControllerGamePad : MonoBehaviour
{
    [Header("Drone Settings")]
    public float hoverForce = 13f;  //gravity right now
    public float horizontalMoveForce = 5f;
    public float verticalMoveForce = 3f; 
    public float turnTorque = 2f;      

    [SerializeField] private Rigidbody rb;
    public bool isOn = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void FixedUpdate()
    {
        if (!isOn) return;

        ApplyHover();
    }

    // hover
    private void ApplyHover()
    {
        // fight gravity
        rb.AddForce(Vector3.up * hoverForce, ForceMode.Force);
    }

    // movement
    public void MoveForward()
    {
        if (!isOn) return;
        rb.AddForce(transform.forward * horizontalMoveForce, ForceMode.Force);
    }

    public void MoveBackward()
    {
        if (!isOn) return;
        rb.AddForce(-transform.forward * horizontalMoveForce, ForceMode.Force);
    }

    public void MoveRight()
    {
        if (!isOn) return;
        rb.AddForce(transform.right * horizontalMoveForce, ForceMode.Force);
    }

    public void MoveLeft()
    {
        if (!isOn) return;
        rb.AddForce(-transform.right * horizontalMoveForce, ForceMode.Force);
    }

    public void MoveUp()
    {
        if (!isOn) return;
        rb.AddForce(Vector3.up * verticalMoveForce, ForceMode.Force);
    }

    public void MoveDown()
    {
        if (!isOn) return;
        rb.AddForce(Vector3.down * verticalMoveForce, ForceMode.Force);
    }

    // rotation
    public void TurnRight()
    {
        if (!isOn) return;
        rb.AddTorque(Vector3.up * turnTorque, ForceMode.Force);
    }

    public void TurnLeft()
    {
        if (!isOn) return;
        rb.AddTorque(Vector3.down * turnTorque, ForceMode.Force);
    }

    // power
    public void TurnOn()
    {
        isOn = true;
    }

    public void TurnOff()
    {
        isOn = false;
    }
    public void ChangeOnOff()
    {
        isOn = !isOn;
    }

/*
    void FixedUpdate()
    {
        // throttle up down
        Vector3 upForce = rb.transform.up * leftStickInput.y * throttleForce;
        rb.AddForce(upForce, ForceMode.Force);

        // yaw
        Vector3 yawTorqueVector = rb.transform.up * leftStickInput.x * yawTorque;
        rb.AddTorque(yawTorqueVector, ForceMode.Force);

        // pitch
        Vector3 pitchTorque = rb.transform.right * rightStickInput.y * pitchForce;
        rb.AddTorque(pitchTorque, ForceMode.Force);

        // roll
        Vector3 rollTorque  = rb.transform.forward * -rightStickInput.x * rollForce;
        rb.AddTorque(rollTorque, ForceMode.Force);
    }

    
    private Vector2 ApplyDeadzone(Vector2 input, float threshold)
    {
    input.x = Mathf.Abs(input.x) < threshold ? 0f : input.x;
    input.y = Mathf.Abs(input.y) < threshold ? 0f : input.y;
    return input;
    }
      */
}
