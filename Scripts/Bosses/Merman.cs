using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merman : Boss
{

    private GameObject tridentHitbox;
    bool isStabbing;
    bool isUpDown;
    bool left;
    float patrolSpeed;
    int frameCounter;
    int direction;

    /// <summary>
    /// -0.508 0.511
    /// -0.64 0.306
    /// -0.581, 0
    /// -0.5, 0.059
    /// frame 9-34
    /// -0.271
    /// frame 34-51
    /// -0.67
    /// 
    /// </summary>
    
    // Start is called before the first frame update
    void Start()
    {
        tridentHitbox = transform.GetChild(0).gameObject;
        attackTimeDelay = 4f;
        IFrameWindow = 0.5f;
        lastAttackTime = 0;
        isPlayerPresent = false;
        animator = GetComponent<Animator>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        sprite =  GetComponent<SpriteRenderer>();
        defaultColor = sprite.color;
        hp = 250;
        patrolSpeed = 0.7f;
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
                    animator.SetTrigger("Stab");
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
