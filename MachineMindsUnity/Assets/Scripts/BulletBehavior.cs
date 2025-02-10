using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    public float bulletSpeed = 1f;
    public float bulletBounceAngle = 45f; //do we really want to have bouncy bullets?
    public float bulletLifeTime = 10f;
    private float bulletLifeTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update(){
        //Constantly move
        rb.linearVelocity = transform.up * bulletSpeed; 

        //Object Hit Detection:
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 0.25F); //in theory should work, if not need to change where looking (might be transform.up)
        if(hit){
            try{
                hit.transform.SendMessageUpwards("OnBulletHit", gameObject.name);
                Destroy(gameObject);
            }catch(System.Exception e){
                Destroy(gameObject);
                //could add bouncy behavior here since in theory the only objects where message could be sent would be player or enemy (will throw error if can't send message)
                //only walls or other bullets remain for now (could add bullets destroying other bullets or bullets bouncing on bullets)
            }
        }

        //Object Lifetime:
        if(bulletLifeTimer >= bulletLifeTime){
            Destroy(gameObject);
        }else{
            bulletLifeTimer += Time.deltaTime;
        }

    }
}
