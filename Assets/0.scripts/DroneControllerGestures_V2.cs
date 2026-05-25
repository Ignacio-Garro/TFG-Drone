using UnityEngine;

public class DroneControllerGamePad_V2 : MonoBehaviour
{
    [Header("Drone Settings")]
    [SerializeField] private float hoverForce = 13f;  //gravity
    [SerializeField] private float horizontalMoveForce = 5f;
    [SerializeField] private float verticalMoveForce = 3f; 
    [SerializeField] private float turnTorque = 2f;
    [SerializeField] private float turnDuration = 0.35f; 
    [SerializeField] private float turnSmoothing = 6f;     // ramp up speed
    [SerializeField] private float turnDecaySmoothing = 2f; // ramp down speed
    [SerializeField] private float BoostMultiplier = 1.8f;
    [Header("Audio")]
    [SerializeField] private AudioSource droneAudio;
    [SerializeField] private float minPitch = 1f;
    [SerializeField] private float maxPitch = 2.0f;
    [SerializeField] private float minVolume = 0.3f;
    [SerializeField] private float maxVolume = 1.0f;
    [SerializeField] private float maxAudioSpeed = 5f;
    private float turnTarget = 0f;
    private float turnDecayTimer = 0f;

    private enum MoveDirection { None, Forward, Backward, Left, Right }

    [Header("Drone Movement")]
    [SerializeField] private MoveDirection currentDirection = MoveDirection.None;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private Rigidbody rb;
    private bool moving = false;
    private float CurrentSpeedBoost;
    public bool isOn = false;

    void Awake(){
        rb = GetComponent<Rigidbody>();
        moving = false;
        CurrentSpeedBoost = 1;
        droneAudio.Play();
    }

    void Update(){
        UpdateAudio();
    }

    void FixedUpdate(){
        if (!isOn) return;
        ApplyHover();
        if (moving) ApplyContinuousMovement();
        ClampSpeed();
        ApplyTurn();
    }

    private void ApplyHover(){
        rb.AddForce(Vector3.up * hoverForce, ForceMode.Force);
    }

    private void ApplyContinuousMovement() {
        switch (currentDirection) {
            case MoveDirection.Forward:
                rb.AddForce(transform.forward * horizontalMoveForce * CurrentSpeedBoost, ForceMode.Force);
                break;
            case MoveDirection.Backward:
                rb.AddForce(-transform.forward * horizontalMoveForce * CurrentSpeedBoost, ForceMode.Force);
                break;
            case MoveDirection.Left:
                rb.AddForce(-transform.right * horizontalMoveForce * CurrentSpeedBoost, ForceMode.Force);
                break;
            case MoveDirection.Right:
                rb.AddForce(transform.right * horizontalMoveForce * CurrentSpeedBoost, ForceMode.Force);
                break;
        }
    }

    // horiozontal
    public void MoveForward(){
        if (!isOn) return;
        if(currentDirection == MoveDirection.Forward){
            CurrentSpeedBoost = BoostMultiplier;
        }
        else
        {
            CurrentSpeedBoost = 1;
            currentDirection = MoveDirection.Forward;
            moving = true;
        }
    }

    public void MoveBackward(){
        if (!isOn) return;
        if(currentDirection == MoveDirection.Backward){
            CurrentSpeedBoost = BoostMultiplier;
        }
        else
        {
            CurrentSpeedBoost = 1;
            currentDirection = MoveDirection.Backward;
            moving = true;
        }
    }

    public void MoveRight(){
        if (!isOn) return;
        if(currentDirection == MoveDirection.Right){
            CurrentSpeedBoost = BoostMultiplier;
        }
        else
        {
            CurrentSpeedBoost = 1;
            currentDirection = MoveDirection.Right;
            moving = true;
        }
    }

    public void MoveLeft(){
        if (!isOn) return;
        if(currentDirection == MoveDirection.Left){
            CurrentSpeedBoost = BoostMultiplier;
        }
        else
        {
            CurrentSpeedBoost = 1;
            currentDirection = MoveDirection.Left;
            moving = true;
        }
    }

    // vertical

    public void MoveUp(){

        if (!isOn) return;

        rb.AddForce(Vector3.up * verticalMoveForce, ForceMode.Force);

    }
    public void MoveDown(){

        if (!isOn) return;

        rb.AddForce(Vector3.down * verticalMoveForce, ForceMode.Force);

    }

    // rotation
    public void TurnRight(){
        if (!isOn) return;
        turnTarget = turnTorque;
        turnDecayTimer = turnDuration;
    }

    public void TurnLeft(){
        if (!isOn) return;
        turnTarget = -turnTorque;
        turnDecayTimer = turnDuration;
    }

    private void ApplyTurn(){
        // tick down then fade to 0
        if (turnDecayTimer > 0f)
            turnDecayTimer -= Time.fixedDeltaTime;
        else
            turnTarget = 0f;

        // snappy start, gradual stop
        bool decelerating = turnTarget == 0f || Mathf.Abs(turnTarget) < Mathf.Abs(rb.angularVelocity.y);
        float smoothing = decelerating ? turnDecaySmoothing : turnSmoothing;
        Vector3 av = rb.angularVelocity;
        av.y = Mathf.Lerp(av.y, turnTarget, Time.fixedDeltaTime * smoothing);
        rb.angularVelocity = av;
    }

    // states
    public void TurnOn(){ 
        isOn = true; 
        CurrentSpeedBoost = 1;
    }
    public void TurnOff(){ 
        isOn = false; 
        StopMoving(); 
    }
    public void ChangeOnOff(){ 
        isOn = !isOn; 
        if(!isOn) StopMoving();
    }

    public void StopMoving(){
        moving = false;
        CurrentSpeedBoost = 1;
        currentDirection = MoveDirection.None;
    }

    private void UpdateAudio(){
        if (!droneAudio.isPlaying) droneAudio.Play();
        float t = Mathf.Clamp01(rb.linearVelocity.magnitude / maxAudioSpeed);
        droneAudio.pitch  = Mathf.Lerp(minPitch,  maxPitch,  t);
        droneAudio.volume = Mathf.Lerp(minVolume, maxVolume, t);
    }

    private void ClampSpeed(){
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

}