using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadTest : MonoBehaviour
{
    void Update()
    {
        var gp = Gamepad.current;
        if (gp == null) return;

        if (gp.buttonSouth.wasPressedThisFrame) Debug.Log("X pressed");
        Vector2 move = gp.leftStick.ReadValue();
        if (move.sqrMagnitude > 0.01f) Debug.Log($"Stick: {move}");
    }
}
