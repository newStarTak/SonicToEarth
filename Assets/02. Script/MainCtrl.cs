using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainCtrl : MonoBehaviour
{
    public bool isStory = false;
    public float time;

    private void Start()
    {
        if(isStory)
        {
            Invoke("startGame", time);
        }
    }

    public void startBtn()
    {
        SceneManager.LoadScene("Story");
    }

    public void startGame()
    {
        SceneManager.LoadScene("Stage_Tutorial");
    }

    public void returnBtn()
    {
        SceneManager.LoadScene("Main");
    }

    public void exitBtn()
    {
        Application.Quit();
    }
}
