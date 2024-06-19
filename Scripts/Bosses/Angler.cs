using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angler : Boss
{
     bool left;
    float patrolSpeed;
    int frameCounter;
    int direction;
    // Start is called before the first frame update
    void Start()
    {
        attackTimeDelay = 4f;
        IFrameWindow = 0.5f;
        lastAttackTime = 0;
        isPlayerPresent = false;
        animator = GetComponent<Animator>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        sprite =  GetComponent<SpriteRenderer>();
        defaultColor = sprite.color;
        hp = 250;
        patrolSpeed = 0.8f;
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null && pc != null){
            if(roomIndex == manager.currRoomIndex){
                bossDoor.lockDoor();
            lastAttackTime += Time.deltaTime;
            if(lastAttackTime > attackTimeDelay){
                lastAttackTime = 0;
                float rand = Random.value;
                if(rand >= 0.5){
                    animator.SetTrigger("UpDown");
                }else{
                    animator.SetTrigger("Chomp");
                }

            }
        }
        checkTicks();
        }else{
            player = GameObject.Find("Player(Clone)");
            if(player != null){
                pc = player.GetComponent<PlayerController>();
            }
        }
    }
     void FixedUpdate(){
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
    void OnCollisionEnter2D(Collision2D other){
        //Debug.Log("Collision");
        if(other.gameObject.name.Contains("Harpoon")){
            Debug.Log("Damage");
            TakeDamage();
        }else if(other.gameObject.tag.Equals("Player")){
            pc.takeDamage(6, -1);
        }
    }
    
}
