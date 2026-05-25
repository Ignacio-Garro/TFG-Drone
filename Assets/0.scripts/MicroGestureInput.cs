using UnityEngine;

public class MicroGestureInput : MonoBehaviour
{

    [SerializeField] private OVRHand ovrHandRight;
    [SerializeField] private OVRHand ovrHandLeft;
    [SerializeField] private DroneControllerGamePad_V2 drone;

    void Update()
    {
        OVRHand.MicrogestureType microgestureRight = ovrHandRight.GetMicrogestureType();
        OVRHand.MicrogestureType microgestureLeft = ovrHandLeft.GetMicrogestureType();

        switch (microgestureRight)
        {
            case OVRHand.MicrogestureType.NoGesture:
                break;
            case OVRHand.MicrogestureType.SwipeLeft:
                drone.MoveLeft();
                break;
            case OVRHand.MicrogestureType.SwipeRight:
                drone.MoveRight();
                break;
            case OVRHand.MicrogestureType.SwipeForward:
                drone.MoveForward();
                break;
            case OVRHand.MicrogestureType.SwipeBackward:
                drone.MoveBackward();
                break;
            case OVRHand.MicrogestureType.ThumbTap:
                drone.StopMoving();
                break;
            case OVRHand.MicrogestureType.Invalid:
                break;
            default:
                break;
        }
        switch (microgestureLeft)
        {
            case OVRHand.MicrogestureType.NoGesture:
                break;
            case OVRHand.MicrogestureType.SwipeLeft:
                drone.TurnLeft();
                break;
            case OVRHand.MicrogestureType.SwipeRight:
                drone.TurnRight();
                break;
            case OVRHand.MicrogestureType.SwipeForward:
                drone.MoveUp();
                break;
            case OVRHand.MicrogestureType.SwipeBackward:
                drone.MoveDown();
                break;
            case OVRHand.MicrogestureType.ThumbTap:
                //drone.ChangeOnOff();
                break;
            case OVRHand.MicrogestureType.Invalid:
                break;
            default:
                break;
        }

        
    }
}
