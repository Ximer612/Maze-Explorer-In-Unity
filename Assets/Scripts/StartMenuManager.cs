using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void LoadMainLevel()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
