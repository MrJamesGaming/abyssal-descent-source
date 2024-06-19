using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
* When the player colliders with this GameObject, a door will lock
*
* Script and associated GameObject are only to test Horizontal Door locking
* When this is properly implemented into the boss rooms please delete this
*/
public class LockDoorTest : MonoBehaviour
{

    [SerializeField] HorizontalDoor door;
    [SerializeField] bool isUnlockTrigger;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D trigger){
        
        if(isUnlockTrigger)
            door.unlockDoor();
        else
            door.lockDoor();
    }

    void OnTriggerExit2D(Collider2D trigger){
        //door.unlockDoor();
    }
}
