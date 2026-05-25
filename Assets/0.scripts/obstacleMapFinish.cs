using UnityEngine;

public class obstacleMapFinish : MonoBehaviour
{
    [SerializeField] private timerScript timer;
    [SerializeField] private AudioSource audioSource;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Drone has passed the obstacle course!");
            audioSource.Play();
            timer.StopTimer();
        }
    }
}
