using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        defaultColor = sprite.color;
        frameCounter = frameOffset;
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        pc = player.GetComponent<PlayerController>();
        
    }

    void Update(){
        if(player != null && pc != null){
            Transform playerPos = player.transform;
            Vector2 deltaFromPlayer = playerPos.position - transform.position;
        if(!attackTimerLive){
            if(deltaFromPlayer.magnitude < range && deltaFromPlayer.x * -direction < 0){
                attack();
            }
        }else{
            attackTimer += Time.deltaTime;
            Debug.Log(attackTimer);
            if(attackTimer >= 0.25 ){
                Debug.Log("here");
                attackTimerLive = false;
                pc.takeDamage(3,direction);
            }
        }
        }else{
            player = GameObject.Find("Player (Clone)");
            if(player != null){
                pc = player.GetComponent<PlayerController>();
            }
            
        }
        
        
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!attackTimerLive){
            Vector2 currPos = transform.position;
            if(left){
            //delta for this frame
           direction = -1;
            }else{
            direction = 1;
            }
            Vector2 delta = patrolSpeed * Time.deltaTime * direction * new Vector2(1,0);
            transform.localScale = new Vector2(-direction, 1);
            transform.position = currPos + delta;
            frameCounter++;
            if(frameCounter > 100){
                frameCounter = 0;
                left = !left;
            }
        }
        checkTicks();
        
    }
    void attack(){
        //Debug.Log("I can attack");
        if(Time.time - lastAttack > attackInterval){
            animator.SetTrigger("Attack");
            lastAttack = Time.time;
            attackTimer = 0;
            attackTimerLive = true;
        }
    }

    
}
