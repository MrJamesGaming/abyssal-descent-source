using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerAttack : MonoBehaviour
{
    Camera cam;
    GameObject reticle;
    GameObject weapon;
    CapsuleCollider2D hitbox;
    Animator weaponAnimator;
    GameObject rotationPoint;

    public float angleDegs;
    float elapsedShootTime;

    private float animationTime; //depends on harpoon type

    private float retractTimer;

    private float reticleMagnitude = 1.5f;
    private float length;
    private float prevAngle;
    private float delta;
    private float mousePosX;
    private float mousePosY;
    private float angle;
    private Vector2 position;
    private Vector2 playerPos;
    private Vector2 mousePos;
    private Vector2 mousePosRel;
    PlayerMovement playerMovement;

    public AttackAnimationState animationState;
    public enum AttackAnimationState{
        idle,
        hit,
        shootTimer,
        retractTimer,
    }


    //private HarpoonType harpoon, future enum

    // Start is called before the first frame update
    void Start()
    {
        animationTime = 0.6f; // replace with switch case statement when enumeration is added
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        reticle = transform.Find("Reticle").gameObject;
        weapon = transform.Find("WeaponAnimation").gameObject;

        hitbox = weapon.GetComponent<CapsuleCollider2D>();
        weaponAnimator = weapon.GetComponent<Animator>();
        playerMovement = this.GetComponent<PlayerMovement>();
        rotationPoint = weapon.transform.GetChild(0).gameObject;

        animationState = AttackAnimationState.idle;
    }

    // Update is called once per frame
    void Update()
    {
        attack();
        aim();
        animationChecks();
    }

    private void animationChecks(){
     
        switch(animationState){

            case AttackAnimationState.idle:
                weaponAnimator.Play(("IdleWeapon"));
                break;

            case AttackAnimationState.hit:
                length =  elapsedShootTime / animationTime;
                weaponAnimator.Play("Retract", 0, 1 - length);
                retractTimer = animationTime * (1 - length);//this wont work if retract and extend are asymmetrical
                hitbox.enabled = false;
                animationState = AttackAnimationState.retractTimer;
                break;

            case AttackAnimationState.shootTimer:
                elapsedShootTime += Time.deltaTime;
                if(elapsedShootTime > animationTime){
                    weaponAnimator.Play("Retract", 0, 0);
                    retractTimer = animationTime;
                    animationState = AttackAnimationState.retractTimer;
                }
                break;

            case AttackAnimationState.retractTimer:
                retractTimer -=  Time.deltaTime;
                if(retractTimer <= 0)
                    animationState = AttackAnimationState.idle;
                break;

            default:
                //this state shouldn't happen unless something breaks, keeping it for debugging purposes
                Debug.Log("AttackAnimationState's Broken");
                break;

        }
    }


    private void attack(){
        if(InputManager.attackWasPressed){
            //Debug.Log("Attack was pressed");

            // rotate just weapon, not player
            prevAngle = angleWrapper(playerMovement.isFacingRight ? weapon.transform.eulerAngles.z : -weapon.transform.eulerAngles.z);
            delta = angleDegs - prevAngle; 
            position = rotationPoint.transform.position;
            delta = angleWrapper(delta);
            
            if(playerMovement.isFacingRight){
                weapon.transform.RotateAround(position, Vector3.forward, delta);
            }
            else{
                delta += 180;
                weapon.transform.RotateAround(position, Vector3.back, -delta);
            }

            hitbox.enabled = true;
            weaponAnimator.Play("Shoot",0,0);
            elapsedShootTime = 0;
            animationState = AttackAnimationState.shootTimer;
        }
            
            
    }

    private float angleWrapper(float angle){
        while(angle >= 180){
            angle -= 360;
        }
        while(angle <= -180){
            angle += 360;
        }
        return angle;
    }

    
    private void aim(){
        playerPos = gameObject.transform.position;
	    mousePos = InputManager.aim;
	    mousePos = cam.ScreenToWorldPoint(mousePos);
	    mousePosX = mousePos.x - playerPos.x;//gets the distance between object and mouse position for x
	    mousePosY = mousePos.y - playerPos.y;
        mousePosRel = new Vector2(mousePosX, mousePosY);
        angle = Mathf.Atan2(mousePosY, mousePosX);

        angleDegs = (180f / Mathf.PI) * angle;

        reticle.transform.position = gameObject.transform.position + (new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0)) * reticleMagnitude;

        // writes mousePosRelative to an asset

        // make whole player rotate versus just weapon
        // Debug.Log(angleDegs);
        // if(Math.Abs(angleDegs) >= 90){
        //       transform.Rotate(0f, -180f, 0f);
        // }else{
        //      transform.Rotate(0f, 180f, 0f);
        // }
        // transform.eulerAngles = Vector3.forward * angleDegs;
    }
}
