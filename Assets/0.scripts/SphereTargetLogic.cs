using System.Collections;
using UnityEngine;

public class SphereTargetLogic : MonoBehaviour
{
    [SerializeField] private Collider spawnZone;

    [SerializeField] private float initialTime = 5f;
    [SerializeField] private float timeReductionPerRound = 0.5f;
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private float spawnPadding = 0.5f;

    [SerializeField] private float maxBeepInterval = 1f;
    [SerializeField] private float minBeepInterval = 0.1f;

    private float currentTime;
    private bool isActive = false;
    private float beepTimer;
    private int round;
    private MeshRenderer meshRenderer;
    private Collider sphereCollider;
    [SerializeField] private AudioSource audioSourceBeep;
    [SerializeField] private AudioSource audioSourceHitTarget;
    [SerializeField] private ScoreManager scoreManager;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        sphereCollider = GetComponent<Collider>();
    }

    void Start()
    {
        SetVisible(false);
    }

    // also resets mid game
    public void StartGame()
    {
        StopAllCoroutines();
        round = 0;
        currentTime = initialTime;
        beepTimer = 0f;
        scoreManager.ResetScore();
        MoveToRandomPosition();
        SetVisible(true);
        isActive = true;
    }

    void Update()
    {
        if (!isActive) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            OnTimerExpired();
            return;
        }

        // beep faster with less time
        beepTimer -= Time.deltaTime;
        float interval = Mathf.Lerp(minBeepInterval, maxBeepInterval, currentTime / 30f);
        if (beepTimer <= 0f)
        {
            audioSourceBeep.Play();
            beepTimer = interval;
        }
    }
    //drone enters
    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (other.CompareTag("Player")){
            isActive = false;
            audioSourceHitTarget.Play();
            scoreManager.AddPoint();
            StartCoroutine(RespawnRoutine()); //to run coroutine
        }
    }

    // hide -> respawn less time
    private IEnumerator RespawnRoutine()
    {
        SetVisible(false); //hide
        yield return new WaitForSeconds(respawnDelay); //wait

        round++;
        currentTime = Mathf.Max(initialTime - timeReductionPerRound * round, 0.5f);
        beepTimer = 0f;
        MoveToRandomPosition();
        SetVisible(true);
        isActive = true;
    }


    private void OnTimerExpired()
    {
        isActive = false;
        SetVisible(false);
    }

    // random spot inside the cube
    private void MoveToRandomPosition()
    {
        Bounds bounds = spawnZone.bounds;
        Vector3 randomPosition = new(
            Random.Range(bounds.min.x + spawnPadding, bounds.max.x - spawnPadding),
            Random.Range(bounds.min.y + spawnPadding, bounds.max.y - spawnPadding),
            Random.Range(bounds.min.z + spawnPadding, bounds.max.z - spawnPadding)
        );
        transform.position = randomPosition;
    }

    // show / hide the ball
    private void SetVisible(bool visible)
    {
        meshRenderer.enabled = visible;
        sphereCollider.enabled = visible;
    }


}
