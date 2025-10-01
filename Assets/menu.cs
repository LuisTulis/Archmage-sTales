using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject howToPlay;
    public GameObject credits;

    private void Start()
    {
   
    }

    public void OnHowToPlay()
    {
        mainMenu.SetActive(false);
        howToPlay.SetActive(true);
        credits.SetActive(false);
    }

    public void OnCredits()
    {
        mainMenu.SetActive(false);
        howToPlay.SetActive(false);
        credits.SetActive(true);
    }

    public void Back()
    {
        mainMenu.SetActive(true);
        howToPlay.SetActive(false);
        credits.SetActive(false);
    }

    public void OnNewGame()
    {
        SceneManager.LoadScene("Game_Scene_01");
        print("clicked");
    }

    public void OnContinue()
    {
        //we don't have it yet HEHE
        SceneManager.LoadScene("Game_Scene_01");
    }
}
