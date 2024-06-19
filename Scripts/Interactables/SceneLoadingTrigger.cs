using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
trigger code for the dynamic loader
*/
public class SceneLoadingTrigger : MonoBehaviour
{
    [SerializeField] SceneField[] scenesToLoad;
    [SerializeField] SceneField[] scenesToUnload;

    // Start is called before the first frame update
    void Awake()
    {
       
    }

    void OnTriggerEnter2D(Collider2D collision){

        //Debug.Log("you have collided with the loader");
        if(collision.gameObject.tag == "Player"){
            Debug.Log("Here");
            loadScenes();
            unloadScenes();
        }
    }

    /*
    this should load the scenes on a trigger,
    the extra time complexity is so that no scenes are loaded multiple times
    due to the nature of additive loading
    */
    private void loadScenes(){
        for(int i = 0; i < scenesToLoad.Length; i++){
            bool isLoaded = false;
            for(int j = 0; j < SceneManager.sceneCount; j++){
                Scene curr = SceneManager.GetSceneAt(j);
                if(curr.name == scenesToLoad[i].SceneName){
                    isLoaded = true;
                    break;
                }
            }
            if(!isLoaded){
                SceneManager.LoadSceneAsync(scenesToLoad[i], LoadSceneMode.Additive);
            }
        }
    }

    IEnumerator waiter(){
        yield return new WaitForSeconds(3);
        for(int i = 0; i < scenesToUnload.Length; i++){
            for(int j = 0; j < SceneManager.sceneCount; j++){
                Scene curr = SceneManager.GetSceneAt(j);
                if(curr.name == scenesToUnload[i].SceneName){
                    SceneManager.UnloadSceneAsync(scenesToUnload[i]);
                }
            }
        }
    }

    /*
    same thing this should unload on a trigger,
    extra time complexity here is so we dont unload an already unloaded scene,
    the extra computation time eclipses the resources to unload potentially already unloaded scenes
    */
    private void unloadScenes(){
        StartCoroutine(waiter());
    }
}
