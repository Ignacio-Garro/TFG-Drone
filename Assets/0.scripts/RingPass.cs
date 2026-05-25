using UnityEngine;

public class RingPass : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Material matSuccess;
    [SerializeField] private Material matDefault; 
    [SerializeField] private RingManager ringManager;
    [SerializeField] private AudioSource audioSource;
    private bool hasPassed = false;

    void Start()
    {
        // Busca al gestor en la escena al iniciar
        ringManager = FindObjectOfType<RingManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPassed)
        {
            hasPassed = true;
            Debug.Log("Drone passed through the ring!");
            targetRenderer.material = matSuccess;
            audioSource.Play();
            ringManager.OnRingPassed();
        }
    }
    public void ResetRing()
    {
        hasPassed = false;
        targetRenderer.material = matDefault;
    }
}
