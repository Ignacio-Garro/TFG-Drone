using UnityEngine;
using UnityEngine.InputSystem;

public class DroneButtonsController : MonoBehaviour
{
    [Header("Cameras")]
    public Camera camera1;
    public Camera camera2;

    [Header("Drone Settings")]
    public float flipImpulse = 5f;  // fuerza de impulso al enderezar

    private Rigidbody rb;
    private bool usingCamera1 = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (camera1 != null && camera2 != null)
        {
            camera1.enabled = true;
            camera2.enabled = false;
        }
    }

    // Cambia entre camaras
    public void OnSwitchCamera(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            usingCamera1 = !usingCamera1;
            camera1.enabled = usingCamera1;
            camera2.enabled = !usingCamera1;
        }
    }

    // Enderezar el dron cuando está boca abajo
    public void OnFlipDrone(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // pequeño impulso hacia arriba
            rb.AddForce(Vector3.up * flipImpulse, ForceMode.Impulse);

            // resetear rotación a 0,0,0
            transform.rotation = Quaternion.identity;

            // también puedes resetear velocidades para más control
            rb.angularVelocity = Vector3.zero;
        }
    }
}
