using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDetector : MonoBehaviour
{
    // Start is called before the first frame update
   private List<IInteractable> interactables = new List<IInteractable>();
   PlayerController pc;
   bool prevFrameInput;


    // Update is called once per frame
    void Update()
    {
        pc = (gameObject.GetComponentInParent<PlayerController>());
        if(pc.interactAction.IsPressed() && prevFrameInput != pc.interactAction.IsPressed() && interactables.Count > 0){
            var interactable = interactables[0];
            interactable.interact();
            if(!interactable.canInteract()){
                interactables.Remove(interactable);
            }
        }   
        prevFrameInput = pc.interactAction.IsPressed();
    }
    private void OnTriggerEnter2D(Collider2D other){
        var interactable = other.GetComponent<IInteractable>();
        if(interactable != null && interactable.canInteract()){
            interactables.Add(interactable);

        }
    }
    private void OnTriggerExit2D(Collider2D other){
         var interactable = other.GetComponent<IInteractable>();
        if(interactables.Contains(interactable)){
            interactables.Remove(interactable);

        }
    }
}
