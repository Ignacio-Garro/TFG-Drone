using UnityEngine;

public class obstacleMapStart : MonoBehaviour
{
     [SerializeField] private timerScript timer;
    [SerializeField] private AudioSource audioSource;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Drone has started the obstacle course!");
            audioSource.Play();
            timer.StartTimer();
        }
    }
}