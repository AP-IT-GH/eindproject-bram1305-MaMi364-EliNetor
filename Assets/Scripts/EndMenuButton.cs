using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenuButton : MonoBehaviour
{
    public void menu()
    {
        LoadScene(0);
    }
    private void LoadScene(int id)
    {
        SceneManager.LoadScene(id);
    }
}
