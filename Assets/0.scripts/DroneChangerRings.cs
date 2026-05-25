using UnityEngine;

public class DroneChanger : MonoBehaviour
{
    [SerializeField] private GameObject thumbDrone;
    [SerializeField] private GameObject FPVDrone;
    [SerializeField] private GameObject pinchDrone;
    [SerializeField] private GameObject gamepadDrone;
    [SerializeField] private PerspectiveChanger perspectiveChanger;
    [SerializeField] private timerScript timerScript;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private RingManager ringManager;
    public GameObject droneSelectedDefualt;

    public void ChangeToThumbDrone()
    {
        SetDroneStates(thumbDrone);
    }

    public void ChangeToFPVDrone()
    {
        SetDroneStates(FPVDrone);
    }

    public void ChangeToPinchDrone()
    {
        SetDroneStates(pinchDrone);
    }

    public void ChangeToGamepadDrone()
    {
        SetDroneStates(gamepadDrone);
    }

    private void SetDroneStates(GameObject activeDrone)
    {
        thumbDrone.SetActive(thumbDrone == activeDrone);
        FPVDrone.SetActive(FPVDrone == activeDrone);
        pinchDrone.SetActive(pinchDrone == activeDrone);
        gamepadDrone.SetActive(gamepadDrone == activeDrone);

        droneSelectedDefualt = activeDrone;

        perspectiveChanger.UpdateDroneMounts(activeDrone);
        //resete position
        activeDrone.transform.position = spawnPosition.position;
        activeDrone.transform.rotation = spawnPosition.rotation;
        //reset timer
        if (timerScript != null) timerScript.ResetTimer();
        //reset rings
        if (ringManager != null) ringManager.ResetRings();
    }
}
