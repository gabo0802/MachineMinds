using UnityEngine;

public class BossBattleScript : MonoBehaviour{
    private const float difficultyScale = 1.12f;

    public GameObject cannonHead;

    public int maxEnemyHealth; //# bullets they can survive
    private int defaultEnemyHealth;
    private int currentEnemyHealth;

    private bool isInvulerable = false;
    private int phaseNumber = 2;

    public float pointsWorth = 10000;
    private int currentDifficulty = 1;
    private GameObject levelManager;

    private GameObject currentAlivePlayer;

    public GameObject missileObject;
    public int numMissiles = 5;
    public float enemyMissileShootInterval = 5f;
    private float enemyShootTimer = 0f;

    public GameObject lazerBeamObject;
    public float enemyLazerShootInterval = 5f;
    public float enemyLazerShootDuration = 5f;
    private float enemyShootTimer2 = 0f;
    private GameObject currentLazerBeam;
    public float rotationSpeed = 50f;

    public AudioSource bossSoundEffects;

    public void SetGameObjects(GameObject[] parameters)
    {
        levelManager = parameters[0];
        currentAlivePlayer = parameters[1];
    }

    public void SetDifficultyLevel(int newDifficultyLevel)
    {
        currentDifficulty = newDifficultyLevel;

        if(maxEnemyHealth == defaultEnemyHealth){
            maxEnemyHealth = (int)(maxEnemyHealth * Mathf.Pow(difficultyScale, currentDifficulty - 1));
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

            levelManager.transform.SendMessage("updateBossHealhBar", new int[] { currentEnemyHealth, maxEnemyHealth });

            if (currentEnemyHealth <= 0){
                if (levelManager){
                    levelManager.transform.SendMessage("OnEnemyDeath", pointsWorth);
                }
                bossSoundEffects.Stop();
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

        maxEnemyHealth = (int)(maxEnemyHealth * Mathf.Pow(difficultyScale, currentDifficulty - 1));
        currentEnemyHealth = maxEnemyHealth;
    }

    bool notUpdated = false;
    // Update is called once per frame
    void Update(){
        if(!notUpdated){
            notUpdated = true;
            levelManager.transform.SendMessage("updateBossHealhBar", new int[] { currentEnemyHealth, maxEnemyHealth });
        }

        if(currentAlivePlayer){
            phaseNumber = (int)(3 * ((float) currentEnemyHealth / (float) maxEnemyHealth)) + 1;

            if(phaseNumber == 1){
                cannonHead.transform.up = currentAlivePlayer.transform.position - transform.position;
                cannonHead.transform.rotation = Quaternion.Euler(new Vector3(0, 0, cannonHead.transform.eulerAngles.z));


                if (enemyShootTimer >= enemyMissileShootInterval / Mathf.Pow(difficultyScale, currentDifficulty - 1)){
                    enemyShootTimer = 0f;
                    ShootMissile(currentAlivePlayer.transform.position);
                }else{
                    enemyShootTimer += Time.deltaTime;
                }   
            }else if(phaseNumber == 2){
                cannonHead.transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));

                if(currentLazerBeam && enemyShootTimer2 > 0){
                    enemyShootTimer2 -= Time.deltaTime; 
                }else{
                   if (enemyShootTimer2 >= enemyLazerShootInterval / Mathf.Pow(difficultyScale, currentDifficulty - 1)){
                        bossSoundEffects.Stop();
                        enemyShootTimer2 = enemyLazerShootDuration;
                        currentLazerBeam = Instantiate(lazerBeamObject, cannonHead.transform.position + (cannonHead.transform.up * 10f), cannonHead.transform.rotation);
                        currentLazerBeam.transform.SetParent(cannonHead.transform);
                    }else{
                        if(enemyShootTimer2 >= (enemyLazerShootInterval / Mathf.Pow(difficultyScale, currentDifficulty - 1) - 3) && (!bossSoundEffects.isPlaying || bossSoundEffects.time >= 0.1f)){
                            bossSoundEffects.pitch = 0.75f + (enemyShootTimer2 / enemyLazerShootInterval);
                            bossSoundEffects.Play();
                        }

                        if(currentLazerBeam){
                            Destroy(currentLazerBeam);
                        }
                        enemyShootTimer2 += Time.deltaTime;
                    }       
                } 

                if (enemyShootTimer >= enemyMissileShootInterval / Mathf.Pow(difficultyScale, currentDifficulty - 1)){
                    enemyShootTimer = 0f;
                    
                    for(int i = 0; i < numMissiles; i++){
                        Vector3 targetVector = Vector3.zero;
                        
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
            }
        }
    }
}
