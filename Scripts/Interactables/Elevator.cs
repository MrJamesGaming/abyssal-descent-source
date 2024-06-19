using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Using placeholder sprites for now
*/

public class Elevator : Doors
{
    [SerializeField] bool isBossElevator;

    //VS Code was complaining and said this was the fix, idk what this does
    new

        // Start is called before the first frame update
        void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D trigger){

        if(trigger.gameObject.tag.Equals("Player")){
            /*Insert code for elevator
            * make interact icon appear, implement interactability
            * if the elevator is tagged as a boss elevator move to boss elevator code
            * if player presses interact button move to interact method
            */
        }
    }

    void OnTriggerExit2D(Collider2D trigger){
        //make interact icon dissapear
    }

    void CallElevator(){
        
    }
}
