using UnityEngine;
using Oculus.Interaction.Input;

public class DroneControllerPinch : MonoBehaviour
{
    [SerializeField] private Hand rightHand;
    [SerializeField] private PerspectiveChanger perspectiveChanger;
    [SerializeField] private float hoverForce = 13f;  //gravity
    [SerializeField] private GameObject sphere;
    [SerializeField] private LineRenderer inputLineRenderer;
    [SerializeField] private LineRenderer forceLineRenderer;
    [SerializeField] private float forceVisualScale = 2.0f;
    [SerializeField] private Rigidbody rbDrone;
    [SerializeField] private float forceAmount;
    [SerializeField] private float rotationThreshold = 0.15f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float maxRotationDistance = 0.1f; // at that distance it rot max
    [SerializeField] private int pinchReleaseGraceFrames = 5;
    [SerializeField] private float pinchDistanceThreshold = 0.03f; // min dist pitch
    [Header("Audio")]
    [SerializeField] private AudioSource droneAudio;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 2.0f;
    [SerializeField] private float minVolume = 0.3f;
    [SerializeField] private float maxVolume = 1.0f;
    [SerializeField] private float maxSpeed = 5f;       // max speed pitch
    [SerializeField] private float rotationBlendSpeed = 2f;
    private float currentDisplacementDistance = 0f;
    private Vector3? initialPosition = null; // ? for null
    private UnityEngine.Vector3 appliedForceVector;
    private int pinchLostFrames = 0;
    private bool isPinching = false;
    private float rotationBlend = 0f; // 0-1

    void Start()
    {
        SetupLine(inputLineRenderer);
        SetupLine(forceLineRenderer);
        droneAudio.Play();
    }

    private void SetupLine(LineRenderer lr)
    {
        lr.positionCount = 2;
        lr.enabled = false;
        lr.startWidth = 0.005f;
        lr.endWidth = 0.005f;
    }

    void Update()
    {
        HandleRotation();
        UpdateAudio();

        bool isPinching = rightHand.GetIndexFingerIsPinching();

        // keep pinch if fingers still close
        if (!isPinching && this.isPinching &&
            rightHand.GetJointPose(HandJointId.HandIndexTip, out Pose indexCheck) &&
            rightHand.GetJointPose(HandJointId.HandThumbTip, out Pose thumbCheck))
        {
            if (Vector3.Distance(indexCheck.position, thumbCheck.position) <= pinchDistanceThreshold)
                isPinching = true;
        }

        if (isPinching)
        {
            this.isPinching = true;
            pinchLostFrames = 0;

            if (rightHand.GetJointPose(HandJointId.HandIndexTip, out Pose indexPose)){
                //sphere tester
                sphere.transform.position = indexPose.position;
                sphere.SetActive(true);

                //currentPosition = indexPose.position;
                Vector3 currentLocalPos = rightHand.transform.InverseTransformPoint(indexPose.position); //pos relativce to ovrcamera

                if(initialPosition == null){
                    initialPosition = currentLocalPos;
                }
                else
                {
                    Vector3 localDisplacement = currentLocalPos - initialPosition.Value;
                    currentDisplacementDistance = localDisplacement.magnitude;
                    Vector3 localDirection = localDisplacement.normalized;//local displacement

                    // en land mode la mano no esta orientada con el drone, usamos el drone como referencia
                    bool isLandMode = perspectiveChanger != null && perspectiveChanger.currentView == PerspectiveChanger.ViewState.Land;
                    Transform forceFrame = isLandMode ? rbDrone.transform : rightHand.transform;
                    Vector3 worldDirection = forceFrame.TransformDirection(localDirection);
                    appliedForceVector = worldDirection * (currentDisplacementDistance * forceAmount);

                    Vector3 dronePos = rbDrone.transform.position;

                    //linerederers
                    inputLineRenderer.enabled = true;
                    inputLineRenderer.SetPosition(0, rightHand.transform.TransformPoint(initialPosition.Value)); 
                    inputLineRenderer.SetPosition(1, indexPose.position);

                    forceLineRenderer.enabled = true;
                    forceLineRenderer.SetPosition(0, dronePos);
                    forceLineRenderer.SetPosition(1, dronePos + (appliedForceVector * forceVisualScale));
                }
            }
        }
        else
        {
            //que el pinch se pueda perder unos frames antes de desactivarse
            pinchLostFrames++;
            if (pinchLostFrames >= pinchReleaseGraceFrames)
            {
                this.isPinching = false;
                sphere.SetActive(false);

                inputLineRenderer.enabled = false;
                forceLineRenderer.enabled = false;

                initialPosition = null;
                appliedForceVector = Vector3.zero;
                currentDisplacementDistance = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        ApplyHover();
        rbDrone.AddForce(appliedForceVector, ForceMode.Acceleration);
    }

    private void HandleRotation()
    {
        Vector3 horizontalVelocity = new Vector3(rbDrone.linearVelocity.x, 0, rbDrone.linearVelocity.z);
        Vector3 horizontalForce = new Vector3(appliedForceVector.x, 0, appliedForceVector.z);

        if (horizontalVelocity.magnitude > 0.1f && horizontalForce.magnitude > rotationThreshold)//your moving enough to rotate
        {
            Vector3 droneForward = new Vector3(rbDrone.transform.forward.x, 0, rbDrone.transform.forward.z).normalized;
            float forwardDot = Vector3.Dot(horizontalForce.normalized, droneForward);// 1 if forwards, 0 if sideways, 01 if backwards

            // back: 0 instantaneo. forward: vuelve gradual a 1
            if (forwardDot > 0f)
                rotationBlend = Mathf.MoveTowards(rotationBlend, 1f, Time.deltaTime * rotationBlendSpeed);
            else
                rotationBlend = 0f;

            if (rotationBlend <= 0f) return;

            //do rotation if not going backwards
            float forceMagnitudeWeight = Mathf.Clamp01((horizontalForce.magnitude - rotationThreshold) / rotationThreshold);//more distance == more rotation
            float distanceScale = Mathf.Clamp01(currentDisplacementDistance / maxRotationDistance); // slower spin at small distances
            float rotationWeight = forceMagnitudeWeight * rotationBlend * distanceScale;
            //rotate towards dir
            Quaternion targetRotation = Quaternion.LookRotation(horizontalForce.normalized);
            rbDrone.MoveRotation(Quaternion.Slerp(rbDrone.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed * rotationWeight));
        }
        else
        {
            // blende drops
            rotationBlend = Mathf.MoveTowards(rotationBlend, 0f, Time.deltaTime * rotationBlendSpeed);
        }
    }

    private void ApplyHover(){
        rbDrone.AddForce(Vector3.up * hoverForce, ForceMode.Force);
    }

    private void UpdateAudio()
    {
        if (!droneAudio.isPlaying) droneAudio.Play(); // recover if stopped
        float speed = rbDrone.linearVelocity.magnitude;
        float t = Mathf.Clamp01(speed / maxSpeed);
        droneAudio.pitch  = Mathf.Lerp(minPitch,  maxPitch,  t);
        droneAudio.volume = Mathf.Lerp(minVolume, maxVolume, t);
    }
}
