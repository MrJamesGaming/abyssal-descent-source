using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    protected int hp;
    [SerializeField]protected bool isPlayerPresent;
    protected float attackTimeDelay;
    protected bool IFrameTimerLive;
    protected float IFrameWindow;
    protected float IFrameTimer;
    
    protected GameObject player;
    protected PlayerController pc;
    protected List<BossAttack> attacks;
    protected List<int> probabilites;
    protected float lastAttackTime;
    protected bool isBleeding;
    protected float lastHit;
    protected SpriteRenderer sprite;
    protected Animator animator;
    private float damageIndicatorTimer;
    private bool damageIndicatorLive;
    private int serratedTicks;
    protected Color defaultColor;
    protected GameManager manager;
    [SerializeField]protected HorizontalDoor bossDoor;
    [SerializeField]protected int roomIndex;
    [SerializeField]List<Doors> Doors;
    

    //onStart
    //if(playerprefs.bossDead){
    //  Destroy(gameObject)
    //}
    //p
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void TakeDamage(){
       
        if(pc.isSerrated){
            hp -= 6;
            lastHit = Time.time;
            isBleeding = true;
        }else{
            hp -= 10;
        }

        damageIndicatorTimer = 0f;
        damageIndicatorLive = true;
        sprite.color = Color.red;
    }

    public virtual void checkTicks(){
        if(isBleeding){
            if(Time.time - lastHit > 0.5){
                hp--;
                serratedTicks++;
                lastHit = Time.time;
                damageIndicatorTimer = 0f;
                damageIndicatorLive = true;
                sprite.color = Color.red;
            }
            if(serratedTicks >= 6){
                isBleeding = false;
                serratedTicks = 0;
            }
        }
        if(hp <= 0){
            bossDoor.unlockDoor();
           
            manager = GameObject.Find("GameManager").GetComponent<GameManager>();
            if(this is Merman){
                manager.MiniBossDead = true;
            }else if(this is Swordfish){
                manager.tutorialBossDead = true;
            }else if(this is Angler){
                manager.anglerDead = true;
            }
            Destroy(gameObject);
        }
        if(damageIndicatorLive){
            damageIndicatorTimer += Time.deltaTime;
            if(damageIndicatorTimer >= 0.25){
                damageIndicatorLive = false;
                sprite.color = defaultColor;
            }
        }
    }
}
