using UnityEngine;

public class DroneChangerObstacle : MonoBehaviour
{
    [SerializeField] private GameObject thumbDrone;
    [SerializeField] private GameObject FPVDrone;
    [SerializeField] private GameObject pinchDrone;
    [SerializeField] private GameObject gamepadDrone;
    [SerializeField] private PerspectiveChanger perspectiveChanger;
    [SerializeField] private timerScript timerScript;
    [SerializeField] private Transform spawnPosition;
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
        if (activeDrone.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.MovePosition(spawnPosition.position);
            rb.MoveRotation(spawnPosition.rotation);
        }
        else
        {
            activeDrone.transform.SetPositionAndRotation(spawnPosition.position, spawnPosition.rotation);
        }
        //reset timer
        timerScript.ResetTimer();
    }
}
