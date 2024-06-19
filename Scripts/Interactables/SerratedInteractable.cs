using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerratedInteractable : MonoBehaviour
{
    // Start is called before the first frame update
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Player"){
            GameManager manager = GameObject.Find("GameManager").GetComponent<GameManager>();
            manager.serratedUnlocked = true;
            Debug.Log("Congratulations on unlocking the harpoon");
        }
    }
}
