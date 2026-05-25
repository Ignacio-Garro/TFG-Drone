using UnityEngine;

public class ComfortVignette : MonoBehaviour
{
    public OVRVignette vignette;
    public Transform[] drones;

    [Range(0f, 1f)] public float maxVignetteIntensity = 0.6f;
    [SerializeField] private float speedThreshold = 0.5f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float angularThreshold = 15f;
    [SerializeField] private float maxAngularSpeed = 180f;

    [SerializeField] private float fadeInSpeed = 5f;
    [SerializeField] private float fadeOutSpeed = 2f;

    private float currentIntensity = 0f;
    private Transform activeDrone;
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    [SerializeField] private bool vignetteEnabled = true;

    void Update(){

    if (!vignetteEnabled) return;
    
    Transform current = GetActiveDrone();
    Debug.Log($"[Vignette] Active drone: {(current != null ? current.name : "NONE")}");

    if (current != activeDrone){
        activeDrone = current;
        if (activeDrone != null)
        {
            lastPosition = activeDrone.position;
            lastRotation = activeDrone.rotation;
        }
        return;
    }

    float speed = Vector3.Distance(activeDrone.position, lastPosition) / Time.deltaTime;
    float angularSpeed = Quaternion.Angle(activeDrone.rotation, lastRotation) / Time.deltaTime;

    lastPosition = activeDrone.position;
    lastRotation = activeDrone.rotation;

    float linearFactor = Mathf.InverseLerp(speedThreshold, maxSpeed, speed);
    float angularFactor = Mathf.InverseLerp(angularThreshold, maxAngularSpeed, angularSpeed);
    float targetIntensity = Mathf.Max(linearFactor, angularFactor) * maxVignetteIntensity;

    //Debug.Log($"[Vignette] Speed: {speed:F2} | AngSpeed: {angularSpeed:F2} | LinearF: {linearFactor:F2} | AngF: {angularFactor:F2} | Target: {targetIntensity:F2} | Current: {currentIntensity:F2} | FOV: {vignette.VignetteFieldOfView:F1}");

    float fadeSpeed = targetIntensity > currentIntensity ? fadeInSpeed : fadeOutSpeed;
    currentIntensity = Mathf.MoveTowards(currentIntensity, targetIntensity, fadeSpeed * Time.deltaTime); //cambiar vignette

    vignette.VignetteFieldOfView = Mathf.Lerp(135f, 60f, currentIntensity);
    }

    Transform GetActiveDrone(){
        foreach (Transform drone in drones){
            if (drone.gameObject.activeInHierarchy)
                return drone;
        }
        return null;
    }


    public void SetVignetteActive(bool active){
        vignetteEnabled = active;
        if (!active){
            currentIntensity = 0f;
            vignette.VignetteFieldOfView = 135f;
        }
    }

    public void ToggleVignette(){
        SetVignetteActive(!vignetteEnabled);
    }
}