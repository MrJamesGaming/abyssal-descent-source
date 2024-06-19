using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyHitbox : MonoBehaviour
{
    float speed = 2.1f;
    float start;
    static bool isClone = false;
    bool isOriginal;

    public float projDir;
    public RangedEnemyAnimationManager manager;
    public RangedEnemy re;
    bool projLive;
    // Start is called before the first frame update
    void Start()
    {
        isOriginal = !isClone;
        isClone = true;
        projLive = false;
        start = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(manager != null && projDir != 0){
            if(re == null){
                re = manager.re;
                projLive = true;
            }
            if(!isOriginal){
                transform.Translate(projDir * speed * Time.deltaTime, 0, 0);
                if(Time.time - start > 1.0){
                    manager.shoot = false;
                    Destroy(gameObject);
                }
                if(manager.transform.localScale.x != projDir){
                    Destroy(gameObject);
                }
            //Debug.Log(transform.position);
        }
        }
        if(projLive && re == null && manager == null){
            Destroy(gameObject);
        }
        
        
    }
     void OnCollisionEnter2D(Collision2D collision){
        
        if(collision.gameObject.tag == "Player"){
            manager.shoot = false;
            GameObject enemy = collision.gameObject;
            enemy.GetComponent<PlayerController>().takeDamage(4,(int)projDir);
            Destroy(gameObject);
        }
    }
}
