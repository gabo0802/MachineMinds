using UnityEngine;

public class BossBattleScript : MonoBehaviour{
    private const float difficultyScaleHealth = 1.12f; //12% increases
    private const float difficultyScaleFireRate = 1.07f; //7% increases

    public GameObject cannonHead;

    public int maxEnemyHealth; //# bullets they can survive
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
    private float enemyShootTimer = 0f;

    public GameObject lazerBeamObject;
    public GameObject lazerBeamObjectHarmless;
    public float enemyLazerShootInterval = 5f;
    public float enemyLazerShootDuration = 5f;
    private float enemyShootTimer2 = 0f;
    private GameObject currentLazerBeam;
    public float rotationSpeed = 50f;
    private int spinDirection = 1;

    public AudioSource bossSoundEffects_Lazer;
    public AudioSource bossSoundEffects_Gun;
    public AudioSource bossSoundEffects_Hit;

    public void SetGameObjects(GameObject[] parameters)
    {
        levelManager = parameters[0];
        currentAlivePlayer = parameters[1];
    }

    private void volumeAdjustments(){
        if (PlayerPrefs.HasKey("SoundEffectVolume")){
            bossSoundEffects_Lazer.volume = PlayerPrefs.GetFloat("SoundEffectVolume");
            bossSoundEffects_Gun.volume = PlayerPrefs.GetFloat("SoundEffectVolume");
            bossSoundEffects_Hit.volume = PlayerPrefs.GetFloat("SoundEffectVolume");
        }
    }

    public void SetDifficultyLevel(int newDifficultyLevel)
    {
        currentDifficulty = newDifficultyLevel;

        if(maxEnemyHealth == defaultEnemyHealth){
            maxEnemyHealth = (int)(maxEnemyHealth * Mathf.Pow(difficultyScaleHealth, currentDifficulty - 1));
            currentEnemyHealth = maxEnemyHealth;
        }
        Debug.Log("Current Health Difficulty: " + currentEnemyHealth);
    }

    void OnExplosionHit(){

    }

    void OnBulletHit(GameObject bullet){
        if (bullet){
            Destroy(bullet);
        }

        if(!isInvulerable){
            currentEnemyHealth -= 1;
            bossSoundEffects_Hit.Play();
            
            levelManager.transform.SendMessage("updateBossHealhBar", new int[] { currentEnemyHealth, maxEnemyHealth });

            if (currentEnemyHealth <= 0){
                if (levelManager){
                    levelManager.transform.SendMessage("OnEnemyDeath", pointsWorth);
                }
                bossSoundEffects_Lazer.Stop();
                bossSoundEffects_Gun.Stop();
                Destroy(gameObject);
            }
        }
    }

    void ShootMissile(Vector3 target){
        GameObject tempMissile = (GameObject) Instantiate(missileObject, transform.position, transform.rotation);
        tempMissile.SendMessage("setTargetPosition", target);
        tempMissile.SendMessage("setSpeedScale", 0.01f);
        tempMissile.SendMessage("setAngleChangeSpeed", 1f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        defaultEnemyHealth = maxEnemyHealth;

        if (currentDifficulty < 1){
            currentDifficulty = 1;
        }

        if (maxEnemyHealth < 1){
            maxEnemyHealth = 1;
        }

        maxEnemyHealth = (int)(maxEnemyHealth * Mathf.Pow(difficultyScaleHealth, currentDifficulty - 1));
        currentEnemyHealth = maxEnemyHealth;
    }

    bool notUpdated = false;
    // Update is called once per frame
    void Update(){
        volumeAdjustments();
        if(!notUpdated){
            notUpdated = true;
            levelManager.transform.SendMessage("updateBossHealhBar", new int[] { currentEnemyHealth, maxEnemyHealth });
        }

        if(isInvulerable && invulerabliltyTimer > 0){
            if(!currentInvulerabliltyShieldObject){
                currentInvulerabliltyShieldObject = (GameObject) Instantiate(invulerabliltyShieldObject, transform.position, transform.rotation);
            }
            invulerabliltyTimer -= Time.deltaTime;
        }else{
            if(currentInvulerabliltyShieldObject){
                Destroy(currentInvulerabliltyShieldObject);
            }
            isInvulerable = false;
        }

        if(currentAlivePlayer){
            if(phaseNumber == 1){
                cannonHead.transform.up = currentAlivePlayer.transform.position - transform.position;
                cannonHead.transform.rotation = Quaternion.Euler(new Vector3(0, 0, cannonHead.transform.eulerAngles.z));

                //Missile Attack
                if (enemyShootTimer >= enemyMissileShootInterval / (Mathf.Pow(difficultyScaleFireRate, currentDifficulty - 1))){
                    enemyShootTimer = 0f;
                    bossSoundEffects_Gun.Play();
                    ShootMissile(currentAlivePlayer.transform.position);
                }else{
                    enemyShootTimer += Time.deltaTime;
                }

                //Phase Transition
                if(currentEnemyHealth <= 0.6f * maxEnemyHealth){ // < 60%
                    phaseNumber = 2;
                    invulerabliltyTimer = enemyLazerShootInterval / Mathf.Pow(difficultyScaleFireRate, currentDifficulty - 1);
                    isInvulerable = true;
                }   
            }else if(phaseNumber == 2){

                //Lazer Beam
                cannonHead.transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime * spinDirection));
                if(currentLazerBeam && enemyShootTimer2 > 0){
                    enemyShootTimer2 -= Time.deltaTime; 
                }else{
                   if (enemyShootTimer2 >= enemyLazerShootInterval / Mathf.Pow(difficultyScaleFireRate, currentDifficulty - 1)){
                        lazerBeamObjectHarmless.GetComponent<SpriteRenderer>().color = new Color(1f, 0.5f, 0.5f, 0f);
                        bossSoundEffects_Lazer.Stop();
                        enemyShootTimer2 = enemyLazerShootDuration;
                        currentLazerBeam = Instantiate(lazerBeamObject, cannonHead.transform.position + (cannonHead.transform.up * 10f), cannonHead.transform.rotation);
                        currentLazerBeam.transform.SetParent(cannonHead.transform);
                        lazerBeamObjectHarmless.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                    }else{
                        if(enemyShootTimer2 >= (enemyLazerShootInterval / Mathf.Pow(difficultyScaleFireRate, currentDifficulty - 1) - 3) && (!bossSoundEffects_Lazer.isPlaying || bossSoundEffects_Lazer.time >= 0.1f)){
                            Color tempColor = lazerBeamObjectHarmless.GetComponent<SpriteRenderer>().color;
                            tempColor.a += (Time.deltaTime * 4f);
                            lazerBeamObjectHarmless.GetComponent<SpriteRenderer>().color = tempColor;
                            lazerBeamObjectHarmless.transform.localScale *= 1.05f;

                            bossSoundEffects_Lazer.pitch = 0.75f + (enemyShootTimer2 / enemyLazerShootInterval);
                            bossSoundEffects_Lazer.Play();
                        }

                        if(currentLazerBeam){
                            invulerabliltyTimer = enemyLazerShootInterval / Mathf.Pow(difficultyScaleFireRate, currentDifficulty - 1);
                            isInvulerable = true;
                            spinDirection *= -1;
                            Destroy(currentLazerBeam);
                        }
                        enemyShootTimer2 += Time.deltaTime;
                    }       
                } 

                //Missile Barrage
                if (enemyShootTimer >= 2f * (enemyMissileShootInterval / Mathf.Pow(difficultyScaleFireRate, currentDifficulty - 1))){
                    enemyShootTimer = 0f;

                    for(int i = 0; i < numMissiles; i++){
                        Vector3 targetVector = Vector3.zero;
                        
                        bossSoundEffects_Gun.Play();
                        int randomVal = Random.Range(0, 10);
                        float multiplierVal = 1f;

                        if(randomVal < 5){
                            randomVal = Random.Range(0, 10);
                            if(randomVal > 5){
                                multiplierVal = -1f;
                            }

                            targetVector = new Vector3(-12 + ((Random.Range(3, 7)) * multiplierVal), Random.Range(-4, 5), 0);
                        }else{
                            randomVal = Random.Range(0, 10);
                            if(randomVal > 5){
                                multiplierVal = -1f;
                            }

                            targetVector = new Vector3(Random.Range(-19, -5), (Random.Range(3, 5) * multiplierVal), 0);
                        }


                        ShootMissile(targetVector);
                    }
                }else{
                    enemyShootTimer += Time.deltaTime;
                }

                //Phase Transition?
                /*
                if(currentEnemyHealth <= 0.3f * maxEnemyHealth){ // < 30%

                }
                */   
            }else{

            }
        }
    }
}
