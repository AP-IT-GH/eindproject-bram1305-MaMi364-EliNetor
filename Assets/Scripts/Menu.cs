using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Called when we click the "Play" button.
    public void Easy()
    {
        LoadScene(1);
    }
    public void Normal()
    {
        LoadScene(2);
    }
    public void Hard()
    {
        LoadScene(3);
    }
    // Called when we click the "Quit" button.
    public void OnQuitButton()
    {
        Application.Quit();
    }

    private void LoadScene(int id)
    {
        SceneManager.LoadScene(id);
    }
}
