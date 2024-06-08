using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Collectable : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Reference to the TextMeshPro text object
    private int score = 0; // Initial score

    // Start is called before the first frame update
    void Start()
    {
        UpdateScoreText();
    }

    // This function is called when another object enters a trigger collider attached to this object
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AddPoint();
            Destroy(gameObject);
        }
    }

    // Adds a point to the score and updates the text
    private void AddPoint()
    {
        score += 1;
        UpdateScoreText();
    }

    // Updates the TextMeshPro text with the current score
    private void UpdateScoreText()
    {
        scoreText.text = "Points: " + score;
    }
}
