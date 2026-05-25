using UnityEngine;
public class DroneBladeController : MonoBehaviour
{
    [Header("Blades Settings")]
    [SerializeField] private Transform[] blades;
    [SerializeField] private float maxRPM = 2000f;
    [SerializeField] private float acceleration = 500f;    
    [SerializeField] private float deceleration = 300f;    

    [Header("References")]
    [SerializeField] private DroneControllerGamePad_V2 droneController;     
    [SerializeField] private WristRotationDrone droneControllerWrist;  

    [SerializeField] private bool alwaysOn;  

    private float currentRPM = 0f;
    private float targetRPM = 0;

    void FixedUpdate()
    {
        bool gamePadIsOn = (droneController != null) && droneController.isOn;
        bool wristIsOn = (droneControllerWrist != null) && droneControllerWrist.isOn;

        if (alwaysOn || gamePadIsOn || wristIsOn){
            targetRPM = maxRPM;
        }
        else{
            targetRPM = 0;
        }

        if (currentRPM < targetRPM)
        {
            currentRPM += acceleration * Time.fixedDeltaTime;
            if (currentRPM > targetRPM) currentRPM = targetRPM;
        }
        else if (currentRPM > targetRPM)
        {
            currentRPM -= deceleration * Time.fixedDeltaTime;
            if (currentRPM < targetRPM) currentRPM = targetRPM;
        }

        // Rotar las blades
        float rotationStep = currentRPM * Time.fixedDeltaTime;
        foreach (Transform blade in blades)
        {
            blade.Rotate(Vector3.forward, rotationStep, Space.Self);
        }
    }
}
