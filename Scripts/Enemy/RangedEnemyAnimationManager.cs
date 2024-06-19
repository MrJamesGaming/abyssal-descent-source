using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyAnimationManager : MonoBehaviour
{
   Animator animator;
    public bool shoot;
    public bool hit;
    float elapsedShootTime;
    bool shootTimerLive;
    public GameObject enemy;
    public RangedEnemy re;
    public Animator enemyAnimator;
    private float idleTimer;
    private bool idleDown;

    private float prevDirX;

    private readonly Vector2 OFFSET = new Vector2(1.4f, -1f);
    
    
    void Start()
    {  
        animator = gameObject.GetComponent<Animator>();
        //while(enemy == null);
    }

    // Update is called once per frame
    void Update()
    {
        if(re != null && enemyAnimator != null){
            
            if(shoot){
                animator.Play("SquidProjectile");

            }else {
                animator.Play("Idle");
                transform.localScale = new Vector2(re.direction, 1);
                if(re.direction != 0){
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(enemy.transform.position.x, enemy.transform.position.y) + new Vector2(re.direction * OFFSET.x, OFFSET.y),50);
                    prevDirX = re.direction;
                }else{
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(enemy.transform.position.x, enemy.transform.position.y) + new Vector2(prevDirX * OFFSET.x, OFFSET.y),50);
                }
            }
        }
    }
}
