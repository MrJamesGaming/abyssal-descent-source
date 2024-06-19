using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Cinemachine;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

//copied this entire thing from a YouTube Tutorial
public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [Header("References")]
    public PlayerMovementStats MoveStats;
    [SerializeField] private Collider2D feetCollider;
    [SerializeField] private Collider2D bodyCollider;
    private Rigidbody2D rigidBody;

    //Movement variables
    private Vector2 movementVelocity;
    public bool isFacingRight;
    private PlayerAttack playerAttack;
    private bool isIdle;

    //Collison Check variables
    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;
    private bool isGrounded;
    private bool bumpedHead;
    private Vector2 boxCastOrigin;
    private Vector2 boxCastSize;

    //Jump variables
    public float verticalVelocity { get; private set; }
    private bool isJumping;
    private bool isFastFalling;
    private bool isFalling;
    private float fastFallTime;
    private float fastFallReleaseSpeed;
    private int numberOfJumpsUsed;

    //Apex variables
    private float apexPoint;
    private float timePastApexThreshold;
    private bool isPastApexThreshold;

    //Jump Buffer variables
    private float jumpBufferTimer;
    private bool jumpReleasedDuringBuffer;

    //Coyote Timing variables
    private float coyoteTimer;

    //Look Up/Down variables
    private float playerCamOffset;
    private float playerUpTimer;
    private float playerBackDownTimer;
    private float playerDownTimer;
    private float playerBackUpTimer;
    private LookUpDownStates lookStates = LookUpDownStates.idle;
    private bool notInUp;
    private bool notInDown;

    private CinemachineTransposer transposer;
    //update active VCam as part of reload logic
    [SerializeField]private CinemachineVirtualCamera vcam; //temp serialize for testing reasons


    #endregion

    #region Startup/Update

    private void Awake()
    {
        isFacingRight = true;
        rigidBody = GetComponent<Rigidbody2D>();
        transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
        transposer.m_FollowOffset = new Vector3(0,0, transposer.m_FollowOffset.z);
        playerAttack = this.GetComponent<PlayerAttack>();
    }

    void Update()
    {
        countTimers();
        jumpChecks();
        interact();
        look();
    }

    private void FixedUpdate()
    {
        CollisionChecks();
        jump();

        if(isGrounded)
            move(MoveStats.groundedAcceleration, MoveStats.groundedDeceleration, InputManager.movement);
        else
            move(MoveStats.airAcceleration, MoveStats.airDeceleration, InputManager.movement);
    }
    #endregion

    #region Movement

    private void move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (moveInput != Vector2.zero){

            turnCheck(moveInput);

            Vector2 targetVelocity = Vector2.zero;
            targetVelocity = new Vector2(moveInput.x, 0f) * MoveStats.maxWalkSpeed;

            movementVelocity = Vector2.Lerp(movementVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            rigidBody.velocity = new Vector2(movementVelocity.x, rigidBody.velocity.y);
        }

        else if (moveInput == Vector2.zero)
        {
            movementVelocity = Vector2.Lerp(movementVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            rigidBody.velocity = new Vector2(movementVelocity.x, rigidBody.velocity.y);
        }

    }

    private void turnCheck(Vector2 moveInput)
    {
        isIdle = (playerAttack.animationState == PlayerAttack.AttackAnimationState.idle);

        if(isFacingRight && moveInput.x < 0 && isIdle)
        {
            turn(false);
        }
        else if (!isFacingRight && moveInput.x > 0 && isIdle)
        {
            turn(true);
        }
    }

    private void turn(bool turnRight)
    {
        if(turnRight)
        {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }

    }

    #endregion

    #region Jump
    
    //I want to refactor this to an enum-based switch statement like PlayerAttack, but this is way more complicated
    private void jumpChecks()
    {
        //when we press jump
        if(InputManager.jumpWasPressed){
            jumpBufferTimer = MoveStats.jumpBufferTime;
            jumpReleasedDuringBuffer = false;
        }

        //when we release jump
        if(InputManager.jumpWasReleased){
            if(jumpBufferTimer > 0f){
                jumpReleasedDuringBuffer = true;
            }

            if(isJumping && verticalVelocity > 0f){

                if(isPastApexThreshold){
                    isPastApexThreshold = false;
                    isFastFalling = true;
                    fastFallTime = MoveStats.timeForUpwardsCancel;
                    verticalVelocity = 0f;
                }
                else{
                    isFastFalling = true;
                    fastFallReleaseSpeed = verticalVelocity;
                }
            }
        }

        //initiate jump with jump buffer and coyote timing
        if(jumpBufferTimer > 0f && !isJumping && (isGrounded || coyoteTimer > 0f)){
            initiateJump(1);
            if (jumpReleasedDuringBuffer){
                isFastFalling = true;
                fastFallReleaseSpeed = verticalVelocity;
            }
        }
        //double jump
        else if(jumpBufferTimer > 0f && isJumping && numberOfJumpsUsed < MoveStats.numberOfJumpsAllowed){
            isFastFalling = false;
            initiateJump(1);
        }
        //air jump after coyote time lapsed
        else if(jumpBufferTimer > 0f && isFalling && numberOfJumpsUsed < MoveStats.numberOfJumpsAllowed - 1){
            initiateJump(2); //this needs to be changed to be 2 jumps if we add a double jump to prevent coyote cheesing
            isFastFalling = false; 
            //for now this should do nothing because the player should not be able to jump after coyote timing

            //Exairia - I'm just gonna copy the tutorial for now and see if that makes things work
        }
        //landed
        else if((isJumping || isFalling) && isGrounded && verticalVelocity <= 0f){

            isJumping = false;
            isFalling = false;
            isFastFalling = false;
            fastFallTime = 0f;
            isPastApexThreshold = false;
            numberOfJumpsUsed = 0;

            verticalVelocity = Physics2D.gravity.y;
        }
    }

    private void initiateJump(int jumpsUsed){

        if(!isJumping){
            isJumping = true;
        }
        jumpBufferTimer = 0f;
        numberOfJumpsUsed += jumpsUsed;
        verticalVelocity = MoveStats.initialJumpVelocity;
    }

    private void jump(){

        //apply gravity while jumping
        if(isJumping){

            //check for head bumps
            if(bumpedHead)
                isFastFalling = true;
        
            //gravity on ascending
            if(verticalVelocity >= 0f){

                //apex controls
                apexPoint = Mathf.InverseLerp(MoveStats.initialJumpVelocity, 0f, verticalVelocity);

                if(apexPoint > MoveStats.apexThreshold){
                    
                    if(!isPastApexThreshold){
                        isPastApexThreshold = true;
                        timePastApexThreshold = 0f;
                    }

                    if(isPastApexThreshold){
                        timePastApexThreshold += Time.fixedDeltaTime;

                        if (timePastApexThreshold < MoveStats.apexHangTime){
                            verticalVelocity = 0f;
                        }
                        else{
                            verticalVelocity = -0.01f;
                        }
                    }
                }
                //gravity on ascending but not at the apex threshold
                else{
                    verticalVelocity += MoveStats.jumpGravity * Time.fixedDeltaTime;

                    if (isPastApexThreshold){
                        isPastApexThreshold = false;
                    }
                }
            }
            //gravity on descending
            else if(!isFastFalling){
                verticalVelocity += MoveStats.jumpGravity * MoveStats.gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if(verticalVelocity < 0f){
            
                if (!isFalling){
                    isFalling = true;
                }
            }
        }
        //jump cut
        if(isFastFalling){

            if(fastFallTime >= MoveStats.timeForUpwardsCancel){
                verticalVelocity += MoveStats.jumpGravity * MoveStats.gravityOnReleaseMultiplier * Time.fixedDeltaTime; 
            }
            else if(fastFallTime < MoveStats.timeForUpwardsCancel){
                verticalVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0f, fastFallTime / MoveStats.timeForUpwardsCancel);
            }
            fastFallTime += Time.fixedDeltaTime;
        }
        //normal gravity while falling
        if(!isGrounded && !isJumping){

            if(!isFalling){
                isFalling = true;
            }
            verticalVelocity += MoveStats.jumpGravity * Time.fixedDeltaTime;
        }
        
        //clamp fall speed
        verticalVelocity = Mathf.Clamp(verticalVelocity, -MoveStats.maxFallSpeed, 50f);
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, verticalVelocity);
    }


    #endregion

    #region Collison Checks

    private void groundedCheck()
    {

        boxCastOrigin = new Vector2(feetCollider.bounds.center.x, feetCollider.bounds.min.y);
        boxCastSize = new Vector2(feetCollider.bounds.size.x, MoveStats.groundDetectionRayLength);

        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, MoveStats.groundDetectionRayLength, MoveStats.groundLayer);
        if (groundHit.collider != null)
            isGrounded = true;
        else
            isGrounded = false;

        // Optional Code to visuals ground raycast
        #region Debug Visulaization
        if(MoveStats.debugShowIsGroundedBox){

            Color rayColor;
            if(isGrounded)  
                rayColor = Color.green;
            else
                rayColor = Color.red;

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * MoveStats.groundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * MoveStats.groundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - MoveStats.groundDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);
        }

        #endregion
    }

    private void headBumpCheck(){
        boxCastOrigin = new Vector2(feetCollider.bounds.center.x, bodyCollider.bounds.max.y);
        boxCastSize = new Vector2(feetCollider.bounds.size.x * MoveStats.headWidth, MoveStats.headDetectionRayLength);

        headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, MoveStats.headDetectionRayLength, MoveStats.groundLayer);

        if(headHit.collider != null)
            bumpedHead = true;
        else
            bumpedHead = false;
        
        //optional visualisation code
        #region Debug Visualisation

        if(MoveStats.debugShowHead){

            float headWidth = MoveStats.headWidth;
            Color rayColor;

            if(bumpedHead)
                rayColor = Color.green;
            else
                rayColor = Color.red;
             
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y), Vector2.up * MoveStats.groundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + (boxCastSize.x / 2) * headWidth, boxCastOrigin.y), Vector2.up * MoveStats.headDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y + MoveStats.groundDetectionRayLength), Vector2.right * boxCastSize.x * headWidth, rayColor);
        }

        #endregion
    }

    private void CollisionChecks()
    {
        groundedCheck();
        headBumpCheck();
    }
    #endregion

    #region Timer
    
    private void countTimers()
    {
        jumpBufferTimer -= Time.deltaTime;
        if (!isGrounded){
            coyoteTimer -= Time.deltaTime;
        }
        else{
            coyoteTimer = MoveStats.jumpCoyoteTiming;
        }
    }

    #endregion

    #region Interact
    
    //Testing function
    private void interact(){
        if(InputManager.interactWasPressed)
            Debug.Log("Interact was pressed");
            
    }

    #endregion

    #region Look Up/Down
    
    private enum LookUpDownStates{
        idle,
        up,
        backDown,
        down,
        backUp,
    }

    private void look(){
        //I still don't understand how I condensed both Look functions into this

        notInUp = !(lookStates == LookUpDownStates.up || lookStates == LookUpDownStates.backDown);
        notInDown = !(lookStates == LookUpDownStates.down || lookStates == LookUpDownStates.backUp);

        //This probably isn't much faster than an else if chain, but it looks way nicer
        switch(0){

            case 0 when InputManager.lookUpWasPressed && notInDown:
                playerUpTimer = 0f;
                lookStates = LookUpDownStates.up;
                break;

            case 0 when InputManager.lookUpWasReleased && notInDown:
                playerBackDownTimer = 0f;
                playerCamOffset = transposer.m_FollowOffset.y;
                lookStates = LookUpDownStates.backDown;
                break;
            
            case 0 when InputManager.lookDownWasPressed && notInUp:
                playerDownTimer = 0f;
                lookStates = LookUpDownStates.down;
                break;
            
            case 0 when InputManager.lookDownWasReleased && notInUp:
                playerBackUpTimer = 0f;
                playerCamOffset = transposer.m_FollowOffset.y;
                lookStates = LookUpDownStates.backUp;
                return;
        }

        switch(lookStates){
            
            case LookUpDownStates.idle:
                break;

            case LookUpDownStates.up:
                playerUpTimer += Time.deltaTime;
                transposer.m_FollowOffset = new Vector3(transposer.m_FollowOffset.x, sigmoid(5f, 2f, 2f, playerUpTimer), transposer.m_FollowOffset.z);
                //Add trigger for head moving up animation here
                break;

            case LookUpDownStates.backDown:
                playerBackDownTimer += Time.deltaTime;
                transposer.m_FollowOffset = new Vector3(transposer.m_FollowOffset.x, Mathf.Lerp(playerCamOffset, 0, playerBackDownTimer / 3f), transposer.m_FollowOffset.z);
                break;
        
            case LookUpDownStates.down:
                //Add trigger for crouch here
                playerDownTimer += Time.deltaTime;
                transposer.m_FollowOffset = new Vector3(transposer.m_FollowOffset.x, sigmoid(-5f, 2f,  3f, playerDownTimer), transposer.m_FollowOffset.z);
                break;

            case LookUpDownStates.backUp:
                //Add trigger for returning from crouch here
                playerBackUpTimer += Time.deltaTime;
                transposer.m_FollowOffset = new Vector3(transposer.m_FollowOffset.x, Mathf.Lerp(playerCamOffset, 0, playerBackUpTimer / 3f), transposer.m_FollowOffset.z);
                break;
            
            default:
                //this state shouldn't be reached unless something breaks, leaving in for debugging purposes
                Debug.Log("LookUpDownStates is broken");
                break;
        }

        //trying to add these to the switch statement breaks stuff, I think it's better if these are outside of it anyway
        if(playerBackDownTimer > 3f){
            playerBackDownTimer = 0f;
            playerCamOffset = 0;
            transposer.m_FollowOffset.y = 0;
            lookStates = LookUpDownStates.idle;
        }  

        if(playerBackUpTimer > 3f){
            playerBackUpTimer = 0f;
            playerCamOffset = 0;
            transposer.m_FollowOffset.y = 0;
            lookStates = LookUpDownStates.idle;
        }
    }

    /*
    Sigmoid function for custom tweeners

    max: maximum value the sigmoid will asymptotically approach
    kGrowth: how aggressively the sigmoid approaches the asymptote
    valOffset: offset of the sigmoid horizontally
    val: an instance val to compute sigmoid calculations on, THIS NEEDS TO BE ITERATED SOMEWHERE ELSE BESIDES THE SIGMOID FUNCTION

    this will most likely be refactored to a Utils file
    https://www.desmos.com/calculator/mkj9jysjwr

    Note: make a method that generates a sigmoid given a kGrowth, max, and time wanted to approach the asymptote
    */
    private float sigmoid(float max, float kGrowth, float valOffset, float val){
        //Debug.Log("exp: " + -kGrowth * (val - valOffset));
        float denominator = 1f + Mathf.Exp(-kGrowth*(val - valOffset));

        return max / denominator;
    }
   
    #endregion

}
