using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealingInteractable : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Player"){
            GameObject player = GameObject.Find("Player(Clone)");
            PlayerController pc = player.GetComponent<PlayerController>();
            pc.heal(3);
            Destroy(gameObject);
            
        }
    }
}
