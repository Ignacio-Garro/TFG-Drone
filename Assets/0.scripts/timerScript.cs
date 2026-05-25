using UnityEngine;
using TMPro;
using System.Threading;

public class timerScript : MonoBehaviour
{
    public float time;
    [SerializeField] private TextMeshProUGUI timerText;
    private bool isRunning = false;

    void Update()
    {
        if (isRunning)
        {
            time += Time.deltaTime;
            
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        time = 0;
        timerText.text = "00:00";
        isRunning = false;
    }
}
