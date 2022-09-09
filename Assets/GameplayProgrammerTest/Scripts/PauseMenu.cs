using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public GameObject PauseMenuCanvas;
    private bool paused = false;

    public void Pause()
    {
        PauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        paused = true;
    }

    public void Resume()
    {
        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
    }

    public bool IsPaused()
    {
        return paused;
    }

    void Start()
    {
        Resume();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("joystick button 7"))
        {
            if (IsPaused())
                Resume();
            else
                Pause();
        }
    }
  

    public void Exit()
    {
        Debug.Log("Quit");    
        Application.Quit();
    }

}
