using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//copied from a YouTube Tutorial
public class InputManager : MonoBehaviour
{
   /*
    * Player actions:
    * Move
    * Aim
    * Attack
    * Jump
    * Interact
    * Look Up
    * Look Down
    */
    #region Variables
    public static PlayerInput playerInput;

    public static Vector2 movement;
    public static Vector2 aim;

    public static bool attackWasPressed;
    public static bool attackIsHeld;
    public static bool attackWasReleased;

    public static bool jumpWasPressed;
    public static bool jumpIsHeld;
    public static bool jumpWasReleased;

    public static bool interactWasPressed;
    public static bool interactIsHeld;
    public static bool interactWasReleased;

    public static bool lookUpWasPressed;
    public static bool lookUpWasReleased;

    public static bool lookDownWasPressed;
    public static bool lookDownWasReleased;

    private InputAction moveAction;
    private InputAction aimAction;
    private InputAction attackAction;
    private InputAction jumpAction;
    private InputAction interactAction;
    private InputAction lookUpAction;
    private InputAction lookDownAction;
    List<InputAction> digitalActions = new List<InputAction>();
    #endregion

    void Awake()
    {
        playerInput = new PlayerInput();

        moveAction = playerInput.Player.Move;
        aimAction = playerInput.Player.Aim;
        attackAction = playerInput.Player.Attack;
        jumpAction = playerInput.Player.Jump;
        interactAction = playerInput.Player.Interact;
        lookUpAction = playerInput.Player.LookUp;
        lookDownAction = playerInput.Player.CrouchLookDown;

        moveAction.Enable();
        aimAction.Enable();
        attackAction.Enable();
        jumpAction.Enable();
        interactAction.Enable();
        lookUpAction.Enable();
        lookDownAction.Enable();

        // actions.Add(moveAction); not adding analog fields
        // actions.Add(aimAction);
        
        digitalActions.Add(attackAction);
        digitalActions.Add(jumpAction);
        digitalActions.Add(interactAction);
        digitalActions.Add(lookUpAction);
        digitalActions.Add(lookDownAction);
        
    }

    // Update is called once per frame
    void Update()
    {
        movement = moveAction.ReadValue<Vector2>();
        aim = aimAction.ReadValue<Vector2>();
        
        //I tried making this more concise but that didn't work so we're stuck with this ig.
        jumpWasPressed = jumpAction.WasPressedThisFrame();
        jumpIsHeld = jumpAction.IsPressed();
        jumpWasReleased = jumpAction.WasReleasedThisFrame();

        attackWasPressed = attackAction.WasPressedThisFrame();
        attackIsHeld = attackAction.IsPressed();
        attackWasReleased = attackAction.WasReleasedThisFrame();

        interactWasPressed = interactAction.WasPressedThisFrame();
        interactIsHeld = interactAction.IsPressed();
        interactWasReleased = interactAction.WasReleasedThisFrame();

        lookUpWasPressed = lookUpAction.WasPressedThisFrame();
        lookUpWasReleased = lookUpAction.WasReleasedThisFrame();

        lookDownWasPressed = lookDownAction.WasPressedThisFrame();
        lookDownWasReleased = lookDownAction.WasReleasedThisFrame();

    }

    
}
