using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string level;
    float timer;
    float maxTime = 15;

    public void LoadScene()
    {
        Application.targetFrameRate = 60;           //поставил цель в 60 фпс
        GameManager.levelName = level;
        SceneManager.LoadScene("Loading");
    }

    public void AppQuit()
    {
        Application.Quit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            /*if (SceneManager.GetActiveScene().name == "MainMenu")
                Application.Quit();
            else if (SceneManager.GetActiveScene().name == "ChosenGame" || SceneManager.GetActiveScene().name == "Stats")
            {
                level = "MainMenu";
                LoadScene();
            }*/
        }

        if (SceneManager.GetActiveScene().name == "Intro")
        {
            LoadScene();
        }

        if (SceneManager.GetActiveScene().name == "GlobalIntro")
        {
            if (timer < maxTime)
            {
                timer += Time.deltaTime * 10;
            } else
            {
                LoadScene();
            }
        }
    }
}
