using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Video;

public class CampFire : MonoBehaviour, IInteractable
{

    [SerializeField]int campfireId;
    static int currCampfireId;
    bool isOpen;
    bool isClosed = true;
    GameObject player;
    PlayerController pc;
    Animator animator;

    GameObject gm;
    GameManager manager;

    //This asset is a placeholder, replace with getting currently bound key sprite at some point
    SpriteRenderer interactKeySprite;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager");
        manager = gm.GetComponent<GameManager>();
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player");
        pc = player.GetComponent<PlayerController>();
//        interactKeySprite = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        //interactKeySprite.enabled = false;
    }

    // Update is called once per frame
    void Update(){
        player = GameObject.Find("Player");;
        if(player != null){
            pc = player.GetComponent<PlayerController>();
        
        }
    }

    public void WriteSave(){
        //PlayerPrefs.SetInt("HasSerrated");
        int isSerrated;
        if(pc.isSerrated){
            isSerrated = 1;
        }else{
            isSerrated = 0;
        }
        PlayerPrefs.SetInt("isSerrated", isSerrated);
        PlayerPrefs.SetInt("MaxHealth", pc.maxHP);
        PlayerPrefs.SetInt("MiniBossDead", manager.getMiniBossDead());
        PlayerPrefs.SetInt("TutorialBossDead", manager.getTutorialBossDead());
        PlayerPrefs.SetInt("MultiplayerUnlocked", manager.getMultiplayerUnlocked());
        PlayerPrefs.SetInt("AnglerDead", manager.getAnglerDead());
        PlayerPrefs.SetInt("CampfireID", currCampfireId);
        string currVCam = "VC1";
        if(currCampfireId == 1){
            currVCam = "VC7";
        }else if(currCampfireId == 2){
            currVCam = "VC11";
        }
        PlayerPrefs.SetString("CurrVCam", currVCam);
    }
    public static void deleteSave(){
        int multiplayer = PlayerPrefs.GetInt("MultiplayerUnlocked");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("MultiplayerUnlocked", multiplayer);
    }

    public void interact()
    {
        Debug.Log("we are interacting");
        /*set animation
        Legacy code:
        currCampfireId = campfireId;
        animator.SetTrigger("Opening");
        pc.disableControls();
        manager.enableHud();
        Debug.Log("here");*/
        
    }

    public bool canInteract(){

        if(pc.isJumping)
            return false;
        else
            return true;
    }

    public void OnTriggerEnter2D(Collider2D trigger){
        
        if(isClosed){
            animator.SetTrigger("Opening");
    
            //Using two separate bools here to prevent specific animator bugs (related to queuing I think?)
            isOpen = true;
            isClosed = false;
            //interactKeySprite.enabled = true;
        }
    }
    public void OnTriggerExit2D(Collider2D trigger){
        
        if(isOpen){
            animator.SetTrigger("Closing");
            isClosed = true;
            isOpen = false;
            //interactKeySprite.enabled = false;
        }
    }
}
