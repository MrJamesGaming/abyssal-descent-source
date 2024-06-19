using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

/*
* The Horizontal Door Prefab consists of two Colliders (one CircleCollider and one CapsuleCollider),
* two door sprites with Animator components (one for each side), a Slab sprite (with Collider/Rigidbody to prevent jumping 
* thorugh the door) and a LoadingTrigger.
*
* The smaller CapsuleCollider implements the RigidBody that repels the player and is what the harpoon collides with,
* when the CapsuleCollider is hit by the harpoon the door opens.
*
* The larger CircleCollider acts as a trigger, if the player opens the door and stands inside this trigger the door
* will never shut, when the player exits the trigger the door closes. This is used to prevent the door shutting on 
* the player while they are still inside it, which previously caused a softlock when the player got stuck inside a boss door.
*
* If the player shoots the door while outside of the trigger and never enters it, the door will shut by itself
* after a given 'timerThreshold', specified in seconds. The timer will only tick while 'closeTimerLive' is true.
*/

enum DoorState{

    idle,
    open,
    playerInRange,
    inRangeAndOpen,
    needLocking,
    locked
    
}

public class HorizontalDoor : MonoBehaviour
{
    protected List<Animator> doorAnimators = new List<Animator>();
    protected List<SpriteRenderer> doorSprites = new List<SpriteRenderer>();
    protected float closeTimer;
    protected bool closeTimerLive;
    protected bool doorIsOpen;
    protected bool playerInDoor;
    bool doorIsLocked;
    bool doorNeedsLocking;
    [SerializeField] float timerThreshold;
    protected CapsuleCollider2D capsule;
    Color defaultColor;
    Color lockedColor = new Color(1, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        /*
        * The door Components are stored in child GameObjects, so need to fetch them
        * 
        * I'm using two different lists for Animators and Sprite Renderers for now, consolidation
        * both of these into one list at some point to save memory seems like a good idea
        */
        for (int i = 0; i < transform.childCount; i++){
            doorAnimators.Add(transform.GetChild(i).gameObject.GetComponent<Animator>());
            doorSprites.Add(transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>());
        }

        capsule = this.GetComponent<CapsuleCollider2D>();      
        defaultColor = doorSprites[0].color;
    }

    // Update is called once per frame
    void Update()
    {
        if(closeTimerLive){
            closeTimer += Time.deltaTime;
            //Debug.Log(closeTimer);
        
            if(closeTimer > timerThreshold){
                closeTimer = 0f;
                closeTimerLive = false;

                if(!playerInDoor && doorIsOpen)
                    modifyDoors(false);
            }      
        }
        if(doorNeedsLocking && !doorIsOpen)
            lockDoor();
    }

    void OnTriggerEnter2D(Collider2D trigger){

        if(trigger.gameObject.name.Equals("InteractableHitbox")){
            playerInDoor = true;        
            closeTimerLive = false;
        }
        //Debug.Log("You are colliding with parent trigger");
    }

    void OnTriggerExit2D(Collider2D trigger){

        if(trigger.gameObject.name.Equals("InteractableHitbox") && doorIsOpen){
            modifyDoors(false);
            playerInDoor = false;
            closeTimer = 0f;   
        }
    }

    void OnCollisionEnter2D(Collision2D collision){

        if(collision.gameObject.transform.name.Equals("WeaponAnimation") && !doorIsLocked){
            modifyDoors(true);

            if(!playerInDoor){
                closeTimerLive = true;
                closeTimer = 0f;
            }
        }
    }

    /*
    * Method to open/close doors, disables the CapsuleCollider/RigidBody and plays the door animations
    * open - if open is true, will run code to open the doors; if false will run code to shut doors 
    */
    void modifyDoors(bool open){

        if(open){
            doorIsOpen = true;
            capsule.enabled = false;
            doorAnimators[0].SetTrigger("Open");
            doorAnimators[1].SetTrigger("Open");
        }
        else{
            doorIsOpen = false;
            capsule.enabled = true;
            doorAnimators[0].SetTrigger("Close");
            doorAnimators[1].SetTrigger("Close");
        }
    }

    /*
    * Method to lock door, used when the player triggers a boss fight. The door will lock the next time it is closed
    * after the method call.
    * Simply call this method and it should work, no prep needed
    *
    * The door can only be unlocked by calling the unlockDoor() method
    */
    public void lockDoor(){
        
        //Adding !playerInDoor here to prevent the door somehow locking with the player in it, realistically that won't happen
        if(!doorIsOpen && !playerInDoor){
            doorSprites[0].color = lockedColor;
            doorSprites[1].color = lockedColor;
            doorIsLocked = true;
        }
        else
            doorNeedsLocking = true;
        
    }

    /*
    * Method to unlock a locked door, only does anything when the door has been locked by lockDoor()
    */
    public void unlockDoor(){
        
        if(doorIsLocked){
            doorSprites[0].color = defaultColor;
            doorSprites[1].color = defaultColor;
            doorNeedsLocking = false;
            doorIsLocked = false;
        }
    }
}
