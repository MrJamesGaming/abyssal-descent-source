using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class DestructibleEnemy : Enemy
{
    // Start is called before the first frame update
    float timerAttack;

    float startingPolarity; //relative to player
    bool despawn;
    GameManager manager;
    [SerializeField] int roomIndex;
    void Start()
    {
        despawn = false;
        hp = 5;
        range = 0.5f;
        patrolSpeed = 1.5f;
        left = true;
        animator = GetComponent<Animator>();
        attackTimerLive = false;
        sprite = GetComponent<SpriteRenderer>();
        defaultColor = sprite.color;
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        pc = player.GetComponent<PlayerController>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();   
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null && pc != null && manager.currRoomIndex == roomIndex){
        Transform playerPos = player.transform;
        Vector2 deltaFromPlayer = playerPos.position - transform.position;
        if(!attackTimerLive){
            Vector2 currPos = transform.position;
            Vector2 delta = patrolSpeed * Time.deltaTime * direction * new Vector2(1,0);
            transform.position = currPos + delta;
            if(deltaFromPlayer.magnitude < range ){
                attack();
                animator.SetTrigger("Explode");
            }
            if(deltaFromPlayer.magnitude * startingPolarity> 5){
                despawn = true;
                Destroy(gameObject);
            }
        }else{
            timerAttack += Time.deltaTime;
            if(timerAttack >= 0.25 ){
                if( deltaFromPlayer.magnitude < range){
                    pc.takeDamage(4,(int)startingPolarity);
                }
                Destroy(gameObject);
            }
        }
        checkTicks();
        }else{
            player = GameObject.Find("Player(Clone)");
            if(player != null){
                pc = player.GetComponent<PlayerController>();
                startingPolarity = Mathf.Sign(player.transform.position.x - transform.position.x);
            }
            
        }
        

    }
    void attack(){
        timerAttack = 0;
        attackTimerLive = true;
        
    }
    void OnDestroy(){
        
        float rng = Random.value;
        if(despawn){
            rng = 1;   
        }
        if(killed){
            if(rng < 0.3){
                GameObject health = Instantiate(GameObject.Find("HealthItem"));
                health.transform.position = transform.position;
            }
        }
        
    }
}
