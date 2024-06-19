using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

//copied this entire thing from a YouTube Tutorial
[CreateAssetMenu(menuName = "Player Movement")]
public class PlayerMovementStats : ScriptableObject
{
    [Header("Walk")]
    [Range(1f, 100f)] public float maxWalkSpeed = 12.5f;
    [Range(0.25f, 50f)] public float groundedAcceleration = 5f;
    [Range(0.25f, 50f)] public float groundedDeceleration = 20f;
    [Range(0.25f, 50f)] public float airAcceleration = 5f;
    [Range(0.25f, 50f)] public float airDeceleration = 5f;

    
    [Header("Grounded/Collision Checks")]
    public LayerMask groundLayer;
    public float groundDetectionRayLength = 0.02f;
    public float headDetectionRayLength = 0.02f;
    [Range(0f, 1f)] public float headWidth = 0.75f;

    
    [Header("Jump")]
    public float jumpHeight = 6.5f;
    [Range(1,1.1f)] public float jumpHeightCompensationFactor = 1.054f;
    public float timeTillJumpApex = 0.35f;
    [Range (0.01f, 5f)] public float gravityOnReleaseMultiplier = 2f;
    public float maxFallSpeed = 26f;
    [Range (1,5)] public int numberOfJumpsAllowed = 1;


    [Header ("Jump Cut")]
    [Range (0.02f, 0.3f)] public float timeForUpwardsCancel = 0.027f;

    
    [Header ("Jump Apex")]
    [Range (0.5f, 1.5f)] public float apexThreshold = 0.97f;
    [Range (0.01f, 1.5f)] public float apexHangTime = 0.075f;


    [Header ("Jump Buffer")]
    [Range (0f,1f)] public float jumpBufferTime = 0.125f;


    [Header ("Jump Coyote Timing")]
    [Range (0f,1f)] public float jumpCoyoteTiming = 0.1f;


    [Header ("Debug")]
    public bool debugShowIsGroundedBox;
    public bool debugShowHead;


    [Header ("Jump visualizer tool")]
    public bool showWalkJumpArc = false;
    public bool showRunJumpArc = false;
    public bool stopOnCollision = true;
    public bool drawRight = true;
    [Range (5,100)] public int arcResolution = 20;
    [Range (0,500)] public int visualizationSteps = 90;


    public float jumpGravity {get; private set;}
    public float initialJumpVelocity {get; private set;}
    public float adjustedJumpHeight {get; private set;}

    private void OnValidate(){
        calculateValues();
    }

    private void OnEnable(){
        //changing gravity for 'Underwater' feel movement, comment out this line for 'fast' movement
        //Physics2D.gravity = new Vector2(0, -1f);
        calculateValues();
    }

    private void calculateValues(){

        adjustedJumpHeight = jumpHeight * jumpHeightCompensationFactor;
        jumpGravity = -(2f * adjustedJumpHeight) / Mathf.Pow(timeTillJumpApex, 2f);
        initialJumpVelocity = Mathf.Abs(jumpGravity) * timeTillJumpApex;
    }
}
