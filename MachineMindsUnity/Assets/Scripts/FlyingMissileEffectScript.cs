using UnityEngine;

public class FlyingMissileEffectScript : MonoBehaviour{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector3 targetPosition = Vector3.zero;
    private float totalDistanceToTarget;
    private float currentDistanceFromTarget;

    public GameObject missileObject;
    public GameObject targetReticle;
    private GameObject currentReticle;
    public GameObject explosionObject;
    public float explosionRadius = 1f;
    
    private float worldScale = 2.0f;
    private float angleChangeScale = 10f;
    private float sizeChangeScale = 0.02f;
    private float speedScale = 0.05f;
    

    void setTargetPosition(Vector3 newPosition){
        targetPosition = newPosition;

        currentReticle = Instantiate(targetReticle, targetPosition, new Quaternion(0, 0, 0, 0));

        totalDistanceToTarget = Vector3.Distance(transform.position, targetPosition);
        transform.up = targetPosition - transform.position;
        angleChangeScale /= totalDistanceToTarget;
        sizeChangeScale /= totalDistanceToTarget;
    }

    void setSpeedScale(float newSpeed){
        speedScale = newSpeed;
    }

    void setAngleChangeSpeed(float newSpeed){
        angleChangeScale = newSpeed;
        angleChangeScale /= totalDistanceToTarget;
    }

    void Start(){
        /*targetPosition = new Vector3(Random.Range(-20, 0), Random.Range(-5, 5), 0);
        //targetPosition = (Vector3)((Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition));
        currentReticle = Instantiate(targetReticle, targetPosition, new Quaternion(0, 0, 0, 0));

        totalDistanceToTarget = Vector3.Distance(transform.position, targetPosition);
        transform.up = targetPosition - transform.position;
        angleChangeScale /= totalDistanceToTarget;
        sizeChangeScale /= totalDistanceToTarget;*/
    }

    void EndOfPath(){
        Collider2D[] allExplodedObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        GameObject currentExplosionObject = (GameObject)Instantiate(explosionObject, transform.position, transform.rotation);
        currentExplosionObject.transform.localScale = new Vector3(explosionRadius * worldScale, explosionRadius * worldScale, 1);

        Debug.DrawLine(transform.position - new Vector3(explosionRadius, 0, 0), transform.position + new Vector3(explosionRadius, 0, 0), Color.red, 2.5f);
        Debug.DrawLine(transform.position - new Vector3(0, explosionRadius, 0), transform.position + new Vector3(0, explosionRadius, 0), Color.red, 2.5f);


        foreach (Collider2D currentExplodedObject in allExplodedObjects){
            currentExplodedObject.transform.SendMessageUpwards("OnExplosionHit");
        }
    }

    // Update is called once per frame
    void Update(){
        currentDistanceFromTarget = Vector3.Distance(transform.position, targetPosition);
        float sizeChangeScaleWeighted = sizeChangeScale * Time.deltaTime * 500f;
        float speedScaleWeighted = speedScale * Time.deltaTime * 500f;
        float angleChangeScaleWeighted = angleChangeScale * Time.deltaTime * 500f;

        if(currentDistanceFromTarget > totalDistanceToTarget / 2){
            missileObject.transform.localScale = new Vector3(missileObject.transform.localScale.x + sizeChangeScaleWeighted, 
                                                missileObject.transform.localScale.y + sizeChangeScaleWeighted,
                                                missileObject.transform.localScale.z + sizeChangeScaleWeighted);

            transform.position += transform.up * speedScaleWeighted;
        }else if(currentDistanceFromTarget > 0.1f){
            missileObject.transform.Rotate(angleChangeScaleWeighted, 0, 0);

            missileObject.transform.localScale = new Vector3(missileObject.transform.localScale.x - sizeChangeScaleWeighted, 
                                                missileObject.transform.localScale.y - sizeChangeScaleWeighted,
                                                missileObject.transform.localScale.z - sizeChangeScaleWeighted);
            transform.position += transform.up * speedScaleWeighted;
        }else{
                EndOfPath();
                Destroy(currentReticle);
                Destroy(gameObject);
        }
    }
}
