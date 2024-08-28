using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    public float score = 0;
    //reference to just the visible elements of the score board
    public GameObject scoreBoardVisuals;
    //reference to our text object
    public TextMeshProUGUI scoreTextDisplay;
    private GameManager gameManager => GameManager.Instance;
    // Start is called before the first frame update
    void Start()
    {
        gameManager.OnGameOver += OnGameOver;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.CurrentGameState == GameManager.GameState.Playing)
        {
            score += Time.deltaTime;
        }
    }

    public void OnGameOver(object sender, GameManager.OnGameOverArgs args)
    {
        scoreBoardVisuals.SetActive(true);
        updateScoreDisplay(score);
    }

    public void updateScoreDisplay(float newScore)
    {
        scoreTextDisplay.text = $"Your score was: \n {newScore.ToString()} seconds!";
    }

}
