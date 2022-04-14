using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    private bool paused = false;

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (!paused)
            {

                Time.timeScale = 0;
                paused = true;

            }
            else{ 

                Time.timeScale = 1;
                paused = false;

            }

        }

    }

}
