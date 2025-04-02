using UnityEngine;

public class SpeedAffectingMaterialScript : MonoBehaviour
{
    private float speedAffect = 1f; // > 1f = speed boost; < 1f = speed reduction

    public enum MaterialType
    {
        SlowSand,
        SlipperyIce,
        None
    }
    public MaterialType materialType;

    private void Start()
    {
        switch (materialType)
        {
            case MaterialType.SlowSand:
                speedAffect = 0.5f;
                break;
            case MaterialType.SlipperyIce:
                speedAffect = 10f;
                break;
            case MaterialType.None:
                break;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("changing speed of something");

        if (!other.gameObject.name.ToLower().Contains("bullet") && (other.gameObject.name.ToLower().Contains("player") || other.gameObject.name.ToLower().Contains("enemy")))
        {
            //if(other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy"){
            other.transform.SendMessage("AffectSpeed", speedAffect);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("reverting to original speed");

        if (!other.gameObject.name.ToLower().Contains("bullet") && (other.gameObject.name.ToLower().Contains("player") || other.gameObject.name.ToLower().Contains("enemy")))
        {
            //if(other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy"){
            other.transform.SendMessage("AffectSpeed", 1f);
        }
    }

    void OnExplosionHit()
    {
        Debug.Log(gameObject.name + " got hit be explosion, but cannot get destroyed");
    }
}
