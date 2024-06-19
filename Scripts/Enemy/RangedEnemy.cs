using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class RangedEnemy : Enemy
{
    public RangedEnemyAnimationManager manager;   
    // Start is called before the first frame update
    void Start()
    {
        hp = 15;
        range = 1.6f;
        patrolSpeed = 0.25f;
        RoF = 0.66f; //todo is tune this
        left = true;
        lastAttack = -10000;
        attackInterval = 1.0f / RoF;
       
        animator = GetComponent<Animator>();
        GameObject projAnimation = Instantiate(GameObject.Find("RangedAnimation"));
        manager = projAnimation.GetComponent<RangedEnemyAnimationManager>();
        manager.enemy = gameObject;
        manager.re = this;
        player = GameObject.Find("Player(Clone)");
        //pc = player.GetComponent<PlayerController>();
        manager.enemyAnimator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        defaultColor = sprite.color;
        frameCounter = frameOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null && pc != null){
            Transform playerPos = player.transform;
            Vector2 deltaFromPlayer = playerPos.position - transform.position;
            if(deltaFromPlayer.magnitude < range && deltaFromPlayer.x * -direction < 0 && Math.Abs(deltaFromPlayer.y) < 0.2){
                attack();
            }
        }else{
            player = GameObject.Find("Player (Clone)");
            
        }
           
        
    }

    void FixedUpdate(){
        Vector2 currPos = transform.position;
        if(!attackTimerLive){
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
        if(frameCounter > 50){
            frameCounter = 0;
            left = !left;
        }
        }else{
            attackTimer += Time.deltaTime;
            if(attackTimer > 0.5){
                attackTimerLive = false;
                GameObject hitObj = Instantiate(GameObject.Find("RangedHitbox"), new Vector3(transform.position.x -0.23f, transform.position.y + 0.13f, 0), transform.rotation);
                RangedEnemyHitbox hitbox = hitObj.GetComponent<RangedEnemyHitbox>();
                manager.shoot = true;
                hitbox.manager = manager; 
                hitbox.projDir = direction;
            }
        }
        checkTicks();
    }
    void attack(){
        //Debug.Log(Time.time - lastAttack);
        if(Time.time - lastAttack > attackInterval){
            //Debug.Log("attack animation");
            lastAttack = Time.time;
            animator.SetTrigger("Attack");
            attackTimer = 0;
            attackTimerLive = true;
        }
    }
}
