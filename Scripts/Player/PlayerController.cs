using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
* This is an edit I am doing to test Unity Version Control, please don't break
* Ok for some reason it decided to push to main, please work this time
*/

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    //our harpoon does 10 damage, serrated 6 + 2 / sec for 3 seconds
    //hard cap roF at 0.8
    //0.6 sec / max distance, 0.6 sec / come back
[SerializeField] float moveSpeed;
    [SerializeField] private float jumpAmount;
    [SerializeField] private int hp;
    public int maxHP = 15;

    private float lastAttack;

    [SerializeField] public Rigidbody2D rb2D;
    [SerializeField] public bool isJumping;
    public bool isSerrated = false;

    public float dirX;
    //Animator animator;

    PlayerInput input;
    InputAction moveAction;
    InputAction attackAction;
    public InputAction interactAction;

    private float prevDirX;

    public Animator animator;

    private readonly Vector2 HARPOON_ORIGIN_OFFSET = new Vector2(-0.92f, 0.42f);

    private bool IFrameLive;
    private float globalIFrameTimer;

    private float jumpTimer;
    private bool jumpTimerLive;
    public SpriteRenderer sprite;
    private Color defaultColor;
    private bool movementEnabled;
    private GameManager manager;
    private Scrollbar healthbar;
    //[SerializeField] TMP_Text log ;
    //string conglomerate = "";
    
    
    void Start(){
        //log = GameObject.Find("Log").GetComponent<TMP_Text>();

        hp = 15;
        input = new PlayerInput();
        
        moveAction = input.Player.Move;
        attackAction = input.Player.Attack;
        interactAction = input.Player.Interact;
        input.Player.Attack.performed += onAttack;
        moveAction.Enable();
        attackAction.Enable();
        interactAction.Enable();
        //conglomerate += "Enabled on start \n";
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //healthbar = GameObject.Find("Canvas").GetComponentInChildren<Scrollbar>();
        
        lastAttack = -10;
        
        animator = GetComponent<Animator>();
        if(manager.isMultiplayer){
            animator.enabled = false;
            Debug.Log("In a multiplayer room");
        }else{
            movementEnabled = true;
            Debug.Log("Not in a multiplayer room");
        }
        sprite = GetComponent<SpriteRenderer>();
        defaultColor = sprite.color;
        prevDirX = 1;
        
        //health item bumps to 20
    }

    void Update(){
        if(movementEnabled){
            Move();
            Jump();
        }
        //healthbar.value = hp / 15f;
        checkIFrames();
    }

    void checkIFrames(){
        // if(){

        // }
        if(IFrameLive){
            globalIFrameTimer -= Time.deltaTime;
            if(globalIFrameTimer < 0){
                IFrameLive = false;
                moveAction.Enable();
                attackAction.Enable();
                sprite.color = defaultColor;
            }
        }
    }


    public void onAttack(InputAction.CallbackContext context){
       if(Time.time - lastAttack > 1.25){
            lastAttack = Time.time;
            GameObject harpoon = Instantiate(GameObject.Find("Harpoon"), this.transform);
            animator.SetTrigger("HasShot");
            if(dirX != 0){
                harpoon.transform.position = new Vector2(transform.position.x, transform.position.y) - new Vector2(HARPOON_ORIGIN_OFFSET.x * dirX, HARPOON_ORIGIN_OFFSET.y); 
                harpoon.GetComponent<Harpoon>().harpoonDir = dirX;    
            }else{
                harpoon.GetComponent<Harpoon>().harpoonDir = prevDirX;
                harpoon.transform.position = new Vector2(transform.position.x, transform.position.y) - new Vector2(HARPOON_ORIGIN_OFFSET.x * prevDirX, HARPOON_ORIGIN_OFFSET.y);
            }
            
       }
    }
    public void Move()
    {
        if(moveAction.ReadValue<Vector2>().x > 0.5){ //dead zone
            dirX = 1;
        }else if(moveAction.ReadValue<Vector2>().x < -0.5){
            dirX = -1;
        }else{
            dirX = 0;
        }
        transform.Translate(new Vector3(dirX, 0, 0) * (moveSpeed * Time.deltaTime));
        if(dirX != 0){
            transform.localScale = new Vector2(dirX, 1);
            prevDirX = dirX;
            animator.SetBool("isWalking", true);
        }else{
            animator.SetBool("isWalking", false);
        }
    }

    public void Jump()
    {
        if(jumpTimerLive){
            jumpTimer += Time.deltaTime;
        }
        if (moveAction.ReadValue<Vector2>().y > 0.5 && !isJumping)
        {   
            

            //Debug.Log("here");
            animator.SetTrigger("HasJumped");
           
            jumpTimer = 0f;
            isJumping = true;
            jumpTimerLive = true;
            
        }else if(jumpTimer > 0.15f && jumpTimerLive){
            //Debug.Log("here");
            jumpTimer = 0f;
            rb2D.velocity = new Vector2(0,0);
            rb2D.AddForce(transform.up * jumpAmount, ForceMode2D.Impulse);
            //Debug.Log(rb2D.totalForce);
            jumpTimerLive = false;
            
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            animator.SetBool("HasJumped", false);
            animator.SetBool("isWalking", true);
            isJumping = false;
        }else if(collision.gameObject.name.Contains("Harpoon") && manager.isMultiplayer){
            hp -= 10;
            if(hp <= 0){
                Destroy(gameObject);
            }
        }
    }
    public void takeDamage(int damage, int dirEnemy){
        hp -= damage;
        rb2D.AddForce(new Vector2(dirEnemy * 0.9f, 0.6f), ForceMode2D.Impulse);
        moveAction.Disable();
        attackAction.Disable();
        globalIFrameTimer  = 0.5f;
        IFrameLive = true;
        //disable controls and add I frames, giving it a true retro metroidvania feel
        sprite.color = Color.red;
        //Debug.Log(dirEnemy);
        if(hp <= 0){
            //trigger a game over screen
            SceneManager.LoadScene("GameOver");
        }
        
        Debug.Log(hp);
    }
    public void heal(int health){
        hp += health;
        if(hp > maxHP){
            hp = maxHP;
        }
    }

    public void disableControls(){
        moveAction.Disable();
        attackAction.Disable();
        input.Player.Attack.performed -= onAttack;
        //conglomerate += "Disabled on join \n";
        
    }
    public void enableControls(){
        moveAction.Enable();
        attackAction.Enable();
        input.Player.Attack.performed += onAttack;
    }
}
