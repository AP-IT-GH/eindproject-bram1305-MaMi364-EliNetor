using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    public Canvas canvas;
    private TextMeshPro textMeshPro;

    private void Start()
    {
        textMeshPro = canvas.gameObject.GetComponent<TextMeshPro>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Speler"))
        {
            canvas.gameObject.SetActive(true);
            textMeshPro.text = "You win!!!";
        }
        else
        {
            canvas.gameObject.SetActive(true);
            textMeshPro.text = "You lose ;'(";
        }
    }
}
