using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;

    private int score = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateDisplay();
    }

    // add 1
    public void AddPoint()
    {
        score++;
        UpdateDisplay();
    }

    // back to 0
    public void ResetScore()
    {
        score = 0;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        scoreText.text = score.ToString();
    }
}
