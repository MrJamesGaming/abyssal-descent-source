using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ToSinglePlayer(){
        SceneManager.LoadSceneAsync("StartingRoom");
    }
    public void ToTitleScreen(){
        SceneManager.LoadScene("TitleScreen");
    }
    public void ToControlsScreen(){
        SceneManager.LoadScene("Controls");
    }
    public void ToOptions(){
        SceneManager.LoadScene("Options");
    }
    public void ToMultiplayer(){
        SceneManager.LoadScene("PropHuntPlayroom");
    }
    

}
