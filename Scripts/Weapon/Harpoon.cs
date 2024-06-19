using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Harpoon : MonoBehaviour
{
    // Start is called before the first frame update
    float speed = 2.2f;
    float start;
    static bool isClone = false;
    bool isOriginal;

    public float harpoonDir;
    GameObject harpoonAnimationContainer;
    HarpoonAnimationManager manager;

    private float previousHarpoonDir;

    GameObject player;

    BoxCollider2D collide;
    bool isFirstFrame;
    void Start()
    {
        start = Time.time;
        isOriginal = !isClone;
        isClone = true;
        player = transform.parent.gameObject;
        Transform[] allKids = player.GetComponentsInChildren<Transform>();
        
        for(int i = 0; i < allKids.Length; i++){
            if(allKids[i].name.Contains("WeaponAnimation")){
                harpoonAnimationContainer = allKids[i].gameObject;
            }
        }
        manager = harpoonAnimationContainer.GetComponent<HarpoonAnimationManager>();
        if(!isOriginal){
            manager.shoot = true;
        }
        isFirstFrame = true;
        
        collide = gameObject.GetComponent<BoxCollider2D>();
    }
   
    void OnEnable(){
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable(){
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(arg0.name.Equals("BaseSinglePlayer")){
            isClone = false;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if(!isOriginal){
            transform.Translate( speed * Time.deltaTime, 0, 0);
            transform.position = new Vector3(transform.position.x,player.transform.position.y - 0.42f ,0);
            if(Time.time - start > 0.6){
                manager.hit = true;
                Destroy(gameObject);
            }
            //Debug.Log(harpoonDir);
            if(isFirstFrame){
                isFirstFrame = false;
                //collide.offset = new Vector2(collide.offset.x * -harpoonDir, collide.offset.y);
            }

            if(previousHarpoonDir != harpoonDir){
                //collide.offset = new Vector2(collide.offset.x * -1, collide.offset.y);
            }
            previousHarpoonDir = harpoonDir;
            

        }
        
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Enemy"){
            GameObject enemy = collision.gameObject;
            manager.hit = true;
            enemy.GetComponent<Enemy>().takeDamage();
            Destroy(gameObject);
        }else if(collision.gameObject.tag == "Boss"){
            manager.hit = true;
            Destroy(gameObject);
        }
    }
}
