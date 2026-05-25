using UnityEngine;

public class PerspectiveChanger : MonoBehaviour
{
    [SerializeField] private GameObject cameraRig;
    [SerializeField] private Transform landViewMount; // same always
    [SerializeField]private Transform currentFPVMount;
    [SerializeField]private Transform currentThirdPersonMount;
    private Transform activeMount;//current mount type
    public enum ViewState { FPV, ThirdPerson, Land }
    public ViewState currentView = ViewState.Land;

    public GameObject droneSelectedDefualt;

    void Start()
    {
        if (!droneSelectedDefualt.TryGetComponent<DroneMounts>(out var mounts)) return;
        currentFPVMount = mounts.fpvPoint;
        currentThirdPersonMount = mounts.thirdPersonPoint;
    }

    public void UpdateDroneMounts(GameObject newDrone)
    {
        //get mount postions fpv and 3rd per
        if (!newDrone.TryGetComponent<DroneMounts>(out var mounts)) // no mount points, fall back to land
        {
            currentFPVMount = null;
            currentThirdPersonMount = null;
            SwitchToLandView();
            return;
        }

        currentFPVMount = mounts.fpvPoint;
        currentThirdPersonMount = mounts.thirdPersonPoint;

        //cahnge camera if not landView
        if (currentView == ViewState.FPV) {
            activeMount = currentFPVMount;
        }
        else if (currentView == ViewState.ThirdPerson) {
            activeMount = currentThirdPersonMount;
        }
        CameraRigApply();
    }

    public void SwitchToFPV()
    {
        if (currentFPVMount == null) return;
        activeMount = currentFPVMount;
        currentView = ViewState.FPV;
        CameraRigApply();
    }

    public void SwitchToThirdPerson()
    {
        if (currentThirdPersonMount == null) return;
        activeMount = currentThirdPersonMount;
        currentView = ViewState.ThirdPerson;
        CameraRigApply();
    }

    public void SwitchToLandView()
    {
        activeMount = landViewMount;
        currentView = ViewState.Land;
        CameraRigApply();
    }

    private void CameraRigApply()
    {
        if (activeMount == null) return;
        
        cameraRig.transform.SetParent(activeMount);
        cameraRig.transform.localPosition = Vector3.zero;
        cameraRig.transform.localRotation = Quaternion.identity;
    }
}
