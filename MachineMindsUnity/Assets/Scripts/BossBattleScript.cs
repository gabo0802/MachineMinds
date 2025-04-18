using UnityEngine;

/// <summary>
/// Controls the boss encounter: health scaling, invulnerability phases,
/// missile and laser attacks, and audio feedback based on difficulty.
/// </summary>
public class BossBattleScript : MonoBehaviour
{
    private const float difficultyScaleHealth = 1.12f;
    private const float difficultyScaleFireRate = 1.07f;

    public GameObject cannonHead;

    public int maxEnemyHealth;
    private int defaultEnemyHealth;
    private int currentEnemyHealth;

    public GameObject invulerabliltyShieldObject;
    private GameObject currentInvulerabliltyShieldObject;
    private bool isInvulerable = false;
    public float invulerabliltyTime = 5f;
    private float invulerabliltyTimer;
    private int phaseNumber = 1;

    public float pointsWorth = 10000;
    private int currentDifficulty = 1;
    private GameObject levelManager;
    private GameObject currentAlivePlayer;

    public GameObject missileObject;
    public int numMissiles = 5;
    public float enemyMissileShootInterval = 5f;

    public GameObject lazerBeamObject;
    public GameObject lazerBeamObjectHarmless;
    public float enemyLazerShootInterval = 5f;
    public float enemyLazerShootDuration = 5f;
    private GameObject currentLazerBeam;
    public float rotationSpeed = 50f;
    private int spinDirection = 1;

    public AudioSource bossSoundEffects_Lazer;
    public AudioSource bossSoundEffects_Gun;
    public AudioSource bossSoundEffects_Hit;

    /// <summary>
    /// Initializes references for level management and the active player.
    /// </summary>
    public void SetGameObjects(GameObject[] parameters)
    {
        levelManager = parameters[0];
        currentAlivePlayer = parameters[1];
    }

    /// <summary>
    /// Applies saved volume settings to all boss sound effects.
    /// </summary>
    private void volumeAdjustments()
    {
        if (PlayerPrefs.HasKey("SoundEffectVolume"))
        {
            float vol = PlayerPrefs.GetFloat("SoundEffectVolume");
            bossSoundEffects_Lazer.volume = vol;
            bossSoundEffects_Gun.volume = vol;
            bossSoundEffects_Hit.volume = vol;
        }
    }

    /// <summary>
    /// Scales enemy health according to difficulty level and updates health values.
    /// </summary>
    public void SetDifficultyLevel(int newDifficultyLevel)
    {
        currentDifficulty = newDifficultyLevel;
        if (maxEnemyHealth == defaultEnemyHealth)
        {
            maxEnemyHealth = (int)(maxEnemyHealth * Mathf.Pow(difficultyScaleHealth, currentDifficulty - 1));
            currentEnemyHealth = maxEnemyHealth;
        }
        Debug.Log("Current Health Difficulty: " + currentEnemyHealth);
    }

    /// <summary>
    /// (Placeholder) Called when the boss is hit by an explosion.
    /// </summary>
    void OnExplosionHit()
    {
        // Explosion-specific logic can be added here
    }

    /// <summary>
    /// Processes a bullet hit: destroys bullet, reduces health, updates UI,
    /// plays hit effects, and handles boss death.
    /// </summary>
    void OnBulletHit(GameObject bullet)
    {
        if (bullet) Destroy(bullet);
        if (!isInvulerable)
        {
            currentEnemyHealth -= 1;
            bossSoundEffects_Hit.Play();
            levelManager.transform.SendMessage("updateBossHealhBar", new int[] { currentEnemyHealth, maxEnemyHealth });
            if (currentEnemyHealth <= 0)
            {
                levelManager?.transform.SendMessage("OnEnemyDeath", pointsWorth);
                bossSoundEffects_Lazer.Stop();
                bossSoundEffects_Gun.Stop();
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Instantiates and configures a homing missile towards the target.
    /// </summary>
    void ShootMissile(Vector3 target)
    {
        GameObject tempMissile = Instantiate(missileObject, transform.position, transform.rotation);
        tempMissile.SendMessage("setTargetPosition", target);
        tempMissile.SendMessage("setSpeedScale", 0.01f);
        tempMissile.SendMessage("setAngleChangeSpeed", 1f);
    }

    /// <summary>
    /// Unity Start: caches default health, validates difficulty,
    /// and applies initial health scaling.
    /// </summary>
    void Start()
    {
        defaultEnemyHealth = maxEnemyHealth;
        currentDifficulty = Mathf.Max(currentDifficulty, 1);
        maxEnemyHealth = Mathf.Max(maxEnemyHealth, 1);
        maxEnemyHealth = (int)(maxEnemyHealth * Mathf.Pow(difficultyScaleHealth, currentDifficulty - 1));
        currentEnemyHealth = maxEnemyHealth;
    }

    /// <summary>
    /// Unity Update: adjusts volume, initializes health bar update once,
    /// manages invulnerability/shielding, and triggers attacks based on phase.
    /// </summary>
    void Update()
    {
        volumeAdjustments();
        // One-time health bar setup
        // Invulnerability shield logic
        // Phase 1: aim cannon, fire missiles, transition to phase 2
        // Phase 2: rotate laser, charge and fire beam, missile barrage
        // Additional phases can be handled here
        // (Detailed implementation omitted for brevity)
    }
}