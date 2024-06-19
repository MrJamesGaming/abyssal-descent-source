using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cinemachine;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    BoxCollider2D hitbox;

    [SerializeField]Door other;
    GameObject player;
    PlayerController pc;
    float closeTimer;
    bool closeTimerLive;
    [SerializeField] bool isUpDoor;
    float launchEventMagnitude = 3.35f;
    bool isFirstFrame;
    float timerThreshold;
    int outRoomIndex;
    bool isLocked;
    Color defaultColor;
    Color lockedColor;
    SpriteRenderer sprite;

    void Start()
    {
        animator = GetComponent<Animator>();
        hitbox = GetComponent<BoxCollider2D>();
        isFirstFrame = true;
        sprite = GetComponent<SpriteRenderer>();
        defaultColor = sprite.color;
        lockedColor = new Color(0.88f, 0, 0.2f);
    }

    public void lockDoor(){
        sprite.color = lockedColor;
        isLocked = true;
        if(!other.isLocked){
            other.lockDoor();
        }
    }
    public void unlockDoor(){
        sprite.color = defaultColor;
        isLocked = false;
        if(other.isLocked){
            other.unlockDoor();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(closeTimerLive){
            closeTimer += Time.deltaTime;
            if(other.isUpDoor || isUpDoor){
                timerThreshold = 2.4f;
            }else{
                timerThreshold = 3f;
            }
            if(closeTimer > timerThreshold){
                closeTimer = 0f;
                closeTimerLive = false;
                animator.SetTrigger("Close");
                other.animator.SetTrigger("Close");
                hitbox.enabled = true;
                //Debug.Log(hitbox.enabled + " name: " + gameObject.name);
                isFirstFrame = true;
                
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision){
        if(!isLocked){
            if(collision.gameObject.name.Contains("Harpoon")){
                hitbox.enabled = false;
                animator.SetTrigger("Open");
                other.openAsLinked();
                closeTimerLive = true;
                closeTimer = 0f;
            }
            if(isUpDoor && collision.gameObject.tag.Equals("Player") && isFirstFrame){
                hitbox.enabled = false;
                pc = collision.gameObject.GetComponent<PlayerController>();
                animator.SetTrigger("Open");
                other.openAsLinked();
                closeTimerLive = true;
                closeTimer = 0f;
                pc.rb2D.AddForce(new Vector2(0,launchEventMagnitude), ForceMode2D.Impulse);
                isFirstFrame = false;
            }
        }
        
    }

    public void openAsLinked(){
        hitbox.enabled = false;
        animator.SetTrigger("Open");
        closeTimerLive = true;
        closeTimer = 0f;

    }
}
