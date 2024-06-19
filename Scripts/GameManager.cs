using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Playroom;
using AOT;
using System;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using System.Xml.Serialization;
using Unity.VisualScripting;
using System.Data.Common;
using Cinemachine;
using System.Net.NetworkInformation;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public bool MiniBossDead;
    public bool tutorialBossDead;
    public bool anglerDead;
    public bool multiplayerUnlocked;
    [SerializeField]public bool isMultiplayer;
    [SerializeField]GameObject player;
    List<Vector2> campfireSpawnPoints;
    private float instantiatePlayerTimer;
    private bool timerLive;
    public int currRoomIndex;
    public bool serratedUnlocked;
    GameObject hud;
    [SerializeField]AudioClip introMain;
    [SerializeField]AudioClip loopMain;
    [SerializeField]AudioClip introSwordfish;
    [SerializeField]AudioClip loopSwordfish;
     [SerializeField]AudioClip introMerman;
    [SerializeField]AudioClip loopMerman;
     [SerializeField]AudioClip introAngler;
    [SerializeField]AudioClip loopAngler;


   void Awake(){
        campfireSpawnPoints = new List<Vector2>();
        campfireSpawnPoints.Add(new Vector2(-2.5f,-0.4f));
        campfireSpawnPoints.Add(new Vector2(20.144f, -3.443012f));
        
   }
   void OnEnable(){
        SceneManager.sceneLoaded += onSceneLoad;
   }
   void OnDisable(){
     SceneManager.sceneLoaded -= onSceneLoad;
   }
   private void onSceneLoad(Scene scene, LoadSceneMode mode){
        //legacy code needs to be refactored
     //    if(scene.name.Equals("BaseSinglePlayer")){
          // hud = GameObject.Find("HUD");
          // hud.SetActive(false);
     //      CinemachineVirtualCamera[] cams = GameObject.FindObjectsOfType<CinemachineVirtualCamera>();
     //      for(int i = 0; i < cams.Length; i++){
     //        if(!cams[i].gameObject.name.Equals(PlayerPrefs.GetString("CurrVCam", "VC1"))){
     //           cams[i].enabled = false;
     //        }
     //      } 
     //      Debug.Log(player);
     //      Debug.Log(campfireSpawnPoints);
     //      GameObject playerRef = Instantiate(player, campfireSpawnPoints[PlayerPrefs.GetInt("CampfireID", 0)], quaternion.identity);
     //      GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");
     //      //CinemachineVirtualCamera cam = GameObject.Find("masterCam").GetComponent<CinemachineVirtualCamera>();
     //      //cam.Follow = playerRef.transform;
     //      for(int i = 0; i < rooms.Length; i++){
     //           CinemachineVirtualCamera cam = rooms[i].transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
     //           cam.Follow = playerRef.transform;
     //      }

     //      instantiatePlayerTimer = 0f;
     //      timerLive = true;
     //    }else if(scene.name.Equals("PropHuntPlayroom")){
     //      isMultiplayer = true;
     //    }
   }

   public void disableHud(){
     
     hud.SetActive(false);
     GameObject.Find("Player(Clone)").GetComponent<PlayerController>().enableControls();
          
   }
   public void enableHud(){
     hud.SetActive(true);
   }

   void Update(){
     //Debug.Log(currRoomIndex);
     if(timerLive){
          instantiatePlayerTimer += Time.deltaTime;
          if(instantiatePlayerTimer >= 1.0){
               timerLive = false;
          }
     }
     if(anglerDead){
          SceneManager.LoadScene("TitleScreen");
     }
   }



   public int getMiniBossDead(){
        return boolToInt(MiniBossDead);
   }
    public int getTutorialBossDead(){
        return boolToInt(tutorialBossDead);
   }
   public int boolToInt(bool b){
        if(b){
            return 1;
        }else{
            return 0;
        }
   }
   public int getMultiplayerUnlocked(){
    return boolToInt(multiplayerUnlocked);
   }
   public int getAnglerDead(){
    return boolToInt(anglerDead);
   }

}


