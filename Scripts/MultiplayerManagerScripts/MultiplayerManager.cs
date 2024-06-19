using System;
using System.Collections;
using System.Collections.Generic;
using AOT;
using Playroom;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MultiplayerManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static float lastJoinTime;
    private static readonly List<PlayroomKit.Player> players = new();
    private static readonly List<GameObject> playerGameObjects = new();
    private static Dictionary<string, GameObject> PlayerDict = new();
    private static int playerCount;
    private int localPlayerIndex;
    private static bool playerJoined;
    [SerializeField] TMP_Text log;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Sprite sprite1;
    [SerializeField] Sprite sprite2;
    [SerializeField] Sprite sprite3;
    [SerializeField] Sprite sprite4;
    [SerializeField] Sprite sprite5;
    [SerializeField] Sprite sprite6;
    List<Sprite> propSprites = new List<Sprite>();

    private static int hunterIndex;

    String conglomerate;
    private static bool gameStarted;

    //String GameState
    /*
    Queueing
    Hiding
    Seeking
    Win Hunter
    Win Props
    */
    void Awake()

    {
       propSprites.Add(sprite1);
       propSprites.Add(sprite2);
       propSprites.Add(sprite3);
       propSprites.Add(sprite4);
       propSprites.Add(sprite5);
       propSprites.Add(sprite6); 
    }
    void OnEnable(){
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable(){
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        if(scene.name.Equals("PropHuntPlayroom")){
            PlayroomKit.InsertCoin(new PlayroomKit.InitOptions()
        {
            allowGamepads = false,
            maxPlayersPerRoom = 5,
            skipLobby = false,
            matchmaking = true,
        }, () =>
        {
            PlayroomKit.OnPlayerJoin(AddPlayer);
            
            // The host has launched the game, you can start your game logic here.
        });
        }
    }

    private void beginGame()
    {
        conglomerate = "The game has Begun!";
            GameObject me = playerGameObjects[localPlayerIndex];
            PlayerController pc = me.GetComponent<PlayerController>();
            if(localPlayerIndex == hunterIndex){
                conglomerate += "\n The seeker is " + localPlayerIndex;
                pc.animator.enabled = true;
                            
            }else{
                pc.sprite.sprite = propSprites[Random.Range(0,6)];
                Destroy(pc.animator);
                pc.enableControls();
            }
    }

    private void AddPlayer(PlayroomKit.Player player)
    {
        //conglomerate += playerPrefab + "\n";
        if(!PlayroomKit.GetState<string>("Phase").Equals("Queueing")){
            //player.Kick();
        }
        GameObject playerObj = Instantiate(playerPrefab, new Vector3(-1.89f + 3f * playerCount,-0.71f + 1 * playerCount,0), quaternion.identity);
        //conglomerate += playerObj.transform.position;
        playerJoined = true;
        playerGameObjects.Add(playerObj);
        players.Add(player);
        localPlayerIndex = playerCount;
        playerCount += 1;
        //conglomerate += "We got to add player";
        player.OnQuit(RemovePlayer);
        if(PlayroomKit.IsHost()){
           
        }
        PlayroomKit.SetState("Phase","Queueing");
        lastJoinTime = 0;
    }
    
    [MonoPInvokeCallback(typeof(Action<string>))]    
    private void RemovePlayer(string obj)
    {
        if(playerGameObjects[localPlayerIndex] != null){
            Destroy(playerGameObjects[localPlayerIndex]);
        }else{
            Debug.Log("Error! player not found");
        }
    }

    // Update is called once per frame
     private void Update()
    {
        
        if (playerJoined)
        {
            
            var myPlayer = PlayroomKit.MyPlayer();
            var index = players.IndexOf(myPlayer);

                

            playerGameObjects[index].GetComponent<PlayerController>().Move();
            playerGameObjects[index].GetComponent<PlayerController>().Jump();

            players[index].SetState("posX", playerGameObjects[index].GetComponent<Transform>().position.x);
            players[index].SetState("posY", playerGameObjects[index].GetComponent<Transform>().position.y);
            if(PlayroomKit.IsHost()){
                conglomerate = "LJT "  + lastJoinTime + "\n";
                //conglomerate += PlayroomKit.GetState<string>("Phase");
                lastJoinTime += Time.deltaTime;
                if(lastJoinTime >= 15 && PlayroomKit.GetState<string>("Phase").Equals("Queueing")){
                    
                    hunterIndex = Random.Range(0,playerCount);
                    
                    PlayroomKit.SetState("Phase", "Hiding");
                    
                    //change state to start of the game
                }
            }
            if(PlayroomKit.GetState<string>("Phase").Equals("Queueing")){
                conglomerate = "LJT "  + lastJoinTime + "\n";
                for(int i = 0; i < playerGameObjects.Count; i++){
                    GameObject curr = playerGameObjects[i];
                    PlayerController pc = curr.GetComponent<PlayerController>();
                    pc.disableControls();
                }
                PlayroomKit.SetState("isHiding", false);
            }else if(PlayroomKit.GetState<string>("Phase").Equals("Hiding") && !gameStarted && PlayroomKit.IsHost()){
                conglomerate += "before the RPC "  + PlayroomKit.GetState<string>("Phase") + "\n";  
                Debug.Log(conglomerate);
                var status = 1;      
                PlayroomKit.RpcCall("Begin game", status, PlayroomKit.RpcMode.ALL, ()=>{
                    conglomerate = "in the RPC" + "\n";
                    Debug.Log(conglomerate);
                    log.SetText(conglomerate);
                    beginGame();
                    gameStarted = true;
                });
            }

        }
        
        //conglomerate += "State "  + PlayroomKit.GetState<string>("Phase") + "\n";
        log.SetText(conglomerate);
        
        for(int i = 0; i < players.Count; i++){
            //conglomerate += 
        }

        for (var i = 0; i < players.Count; i++)
        {

            if (players[i] != null)
            {

                
                var posX = players[i].GetState<float>("posX");
                var posY = players[i].GetState<float>("posY");
                Vector3 newPos = new Vector3(posX, posY, 0);

                if (playerGameObjects != null)
                    playerGameObjects[i].GetComponent<Transform>().position = newPos;
                }
        }
    }
}
