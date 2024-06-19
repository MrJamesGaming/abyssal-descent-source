using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonCollision : MonoBehaviour
{

    GameObject player;
    PlayerAttack harpoon;
    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.gameObject;
        harpoon = player.GetComponent<PlayerAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D collision){
        harpoon.animationState = PlayerAttack.AttackAnimationState.hit;
    }
}
