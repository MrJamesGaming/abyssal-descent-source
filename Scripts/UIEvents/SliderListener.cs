using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderListener : MonoBehaviour
{
    [SerializeField]private Slider slider;
    [SerializeField]private bool sfx;
    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener((newVol)=>{
            if(sfx){
                PlayerPrefs.SetFloat("SFXVolume", newVol);
            }else{
                PlayerPrefs.SetFloat("MusicVolume", newVol);
            }
        });
    }
}
