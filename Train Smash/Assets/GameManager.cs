using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool gameStarted = false;
    public GameObject FailedScreen;
    public GameObject CompleteScreen;
    public GameObject SettingScreen;
    public static GameManager gameMan;

    private void Awake()
    {
        gameMan = this;
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartGame()
    {
        gameStarted = true;
    }

    public void CompleteLevel()
    {
        CompleteScreen.SetActive(true);
    }
    public void FailLevel()
    {
       FailedScreen.SetActive(true);
    }
}


