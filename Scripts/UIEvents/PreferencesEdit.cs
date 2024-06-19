using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreferencesEdit : MonoBehaviour
{
    public void adjustVolumeSFX(float newVol){
        
    }
    public void adjustVolumeMusic(float newVol){
        PlayerPrefs.SetFloat("MusicVolume", newVol);
    }
    public void wipeSave(){
        CampFire.deleteSave();
    }
}
