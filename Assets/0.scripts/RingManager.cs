using UnityEngine;

public class RingManager : MonoBehaviour
{
    [SerializeField] private timerScript timer;
    [SerializeField] private AudioSource audioSource;
    private RingPass[] allRings;
    [SerializeField]private int totalRings;
    private int ringsPassed = 0;

    void Start()
    {
        allRings = FindObjectsOfType<RingPass>();
        totalRings = allRings.Length;
    }

    public void OnRingPassed()
    {
        ringsPassed++;

        // first ring-> reset timer and start fresh
        if (ringsPassed == 1)
        {
            timer.ResetTimer();
            timer.StartTimer();
        }

        // last ring
        if (ringsPassed == totalRings)
        {
            timer.StopTimer();
            ResetRings();
            audioSource.Play();
        }
    }
    public void ResetRings()
    {
        ringsPassed = 0;
        foreach (RingPass ring in allRings)
            ring.ResetRing();
    }
}
