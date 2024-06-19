using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float range;
    [SerializeField] protected float hp;
    [SerializeField] protected float patrolSpeed;
    [SerializeField] protected bool left;
    [SerializeField] public int direction;

    //Dictates how long an enemy move in a given direction during patrol state
    [SerializeField] protected int frameCounter;

    [SerializeField] protected float lastAttack;

    //Rate of Fire for any attack, incldes melee and ranged
    [SerializeField] protected  float RoF;

    //Used in rate of fire calculations, the inverse of RoF
    protected float attackInterval;

    protected PlayerController pc;
    protected GameObject player;
    
    // lastHit and serratedTicks are for serrated harpoon calculations
    private float lastHit;
    private int serratedTicks;

    /*
    * Convert 'isBleeding' to an enum containing all the elemental effects we want
    * Method for serrated becomes 'ApplyElemental' or something
    */
    private bool isBleeding;

    /*
    * attackTimer and attacktimerLive add 'hitstop' when the enemy attacks
    * (e.g: it stays still when bitting for melee)
    * Might want to switch to a more sophisticated system later down the line, this
    * Is only here to maintain compataiblity with GameJam code.
    */
    protected float attackTimer;
    protected bool attackTimerLive;

    protected Animator animator;
    private bool damageIndicatorLive;
    private float damageIndicatorTimer;
    protected SpriteRenderer sprite;
    protected Color defaultColor;
    [SerializeField]
    protected int frameOffset;
    protected bool killed;

    //destructible dies to 1, 5
    //ranged dies to 2 serrated or not, 15
    //melee should die to 2 but not serrated, 20 hp
    //

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        //patrol
        //if sees someone, attack
        //
    }

    void attack(){

    }
    public virtual void takeDamage(){
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
    //TODO EXTREMELY IMPORTANT call this during every update!!!
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
            //Debug.Log(this);
            //Debug.Log("hba");
            killed = true;
            if(this is RangedEnemy){
                Debug.Log("here");
                RangedEnemy curr = gameObject.GetComponent<RangedEnemy>();
                Destroy(curr.manager.gameObject);
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

    void OnDestroy(){
        if(Application.isPlaying && SceneManager.GetActiveScene().name.Equals("BaseSinglePlayer") && killed){
            float rng = Random.value;
            if(rng < 0.3){
                GameObject health = Instantiate(GameObject.Find("HealthItem"));
                health.transform.position = transform.position;
            }
        }
    }
    
}
