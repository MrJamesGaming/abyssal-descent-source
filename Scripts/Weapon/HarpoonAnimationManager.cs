using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.Rendering.Universal.Internal;

public class HarpoonAnimationManager : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    public bool shoot;
    public bool hit;
    float elapsedShootTime;
    bool shootTimerLive;
    GameObject player;
    PlayerController pc;
    Animator playerAnimator;
    private float idleTimer;
    private bool idleDown;

    private float prevDirX;

    private readonly Vector2 OFFSET = new Vector2(-0.937f, 0.415f);
    private string prevAnimState;
    private bool retractTimerLive;
    private float retractTimer;
    
    
    void Start()
    {  
        animator = gameObject.GetComponent<Animator>();
        player = transform.parent.gameObject;
        pc = player.GetComponent<PlayerController>();
        playerAnimator = player.GetComponent<Animator>();
        prevDirX = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(shootTimerLive){
            //Debug.Log("Timer live");
            elapsedShootTime += Time.deltaTime;
        }
        if(retractTimerLive){
            retractTimer -= Time.deltaTime;
            if(retractTimer < 0){
                retractTimerLive = false;
            }
        }
        if(shoot){
            if(pc.isSerrated){
                animator.Play("ExtendSerrated",0,0);

            }else{
                animator.Play("Shoot", 0,0);
            }
            shoot = false;
            shootTimerLive = true;
            elapsedShootTime = 0f;
        }
        if(hit){
            float length =  elapsedShootTime / 0.6f;
            retractTimer = elapsedShootTime;
            if(pc.isSerrated){
                animator.Play("RetractSerrated", 0, 1- length);
            }else{
                animator.Play("Retract", 0, 1 - length);
            }
            hit = false;
            shootTimerLive = false;
            retractTimerLive = true;
        }
        if(!shootTimerLive && !retractTimerLive){
            animator.Play("IdleWeapon");
        }

        if(pc.dirX != 0){
            //transform.localScale = new Vector2(pc.dirX, 1);
            transform.position = new Vector2(player.transform.position.x, player.transform.position.y) - new Vector2(pc.dirX * OFFSET.x, OFFSET.y);
            prevDirX = pc.dirX;
        }else{
            transform.position =  new Vector2(player.transform.position.x, player.transform.position.y) - new Vector2(prevDirX * OFFSET.x, OFFSET.y);
        }
    }
}
