using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class RoomCollider : MonoBehaviour
{

    //whole class is legacy code, likely to be deleted in 1-2 commits
    

    void Start(){
        

            
        // if(isStartingCam){
        //     GameObject vcam = transform.GetChild(0).gameObject;
        //     CinemachineVirtualCamera cam = vcam.GetComponent<CinemachineVirtualCamera>();
        //     cam.enabled = true;
        // }else{
        //     GameObject vcam = transform.GetChild(0).gameObject;
        //     CinemachineVirtualCamera cam = vcam.GetComponent<CinemachineVirtualCamera>();
        //     cam.enabled = false;
        // }
    }
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.tag.Equals("Player")){
            GameObject vcam = transform.GetChild(0).gameObject;
            
            CinemachineVirtualCamera cam = vcam.GetComponent<CinemachineVirtualCamera>();
            //Debug.Log("Entering: " + cam.name);
            cam.enabled = true;
            GameObject camera = GameObject.Find("Main Camera");
            //camera.GetComponent<CinemachineBrain>().ActiveVirtualCamera = cam;
            
        }
    }
    void  OnTriggerExit2D(Collider2D collision){
        if(collision.gameObject.tag.Equals("Player")){
            GameObject vcam = transform.GetChild(0).gameObject;
            CinemachineVirtualCamera cam = vcam.GetComponent<CinemachineVirtualCamera>();
            //Debug.Log("Exiting: " + cam.name);
            cam.enabled = false;
        }
    }
}
