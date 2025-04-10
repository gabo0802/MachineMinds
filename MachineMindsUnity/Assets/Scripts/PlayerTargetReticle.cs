using UnityEngine;

public class ReticleScript : MonoBehaviour
{ 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
    }

    // Update is called once per frame
    void Update(){
        if(Time.deltaTime != 0f){
            Cursor.visible = false;
            transform.position = (Vector3)((Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

}
