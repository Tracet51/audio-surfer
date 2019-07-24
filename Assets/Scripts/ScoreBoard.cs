using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    int CurrentScore;

    TextMeshProUGUI ScoreText;


    // Start is called before the first frame update
    void Start()
    {
        ScoreText = GetComponent<TextMeshProUGUI>();
        ScoreText.text = CurrentScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ScoreHit(int scorePerHit)
    {
        CurrentScore += scorePerHit;
        ScoreText.text = CurrentScore.ToString();
    }
}
