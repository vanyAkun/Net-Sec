using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static Player;

public class LevelManagerScript : MonoBehaviour
{
    public int enemies = 5;
    public Text enemiesText;
    public Text timerText; // Text to display the timer
    private float timer = 5f; //  countdown
    public Text YouWinText;
    public Text YouLoseText;
    public Player player;

    public GameObject playAgainButton;
    public GameObject mainMenuButton;
    private void Awake()
    {
        enemiesText.text = enemies.ToString();

        Enemy.OnEnemyKilled += OnEnemyKilled;
       // Player.OnPlayerKilled += OnPlayerKilled;
        UpdateTimerDisplay();
        if (timerText != null)
        {
            UpdateTimerDisplay();
        }
        playAgainButton.SetActive(false);

    }
    /*private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        Player.OnPlayerKilled -= OnPlayerKilled;
    }
    private void OnPlayerKilled()
    {
       
    }*/
    private void Update()
    {

        if (timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay();
        }
        else
        {
            CheckGameEndCondition();
        }
    }

    

    void OnEnemyKilled()
    {
        enemies--;
        enemiesText.text = enemies.ToString();

        if (enemies <= 0)
        {
            // Call the method to handle victory
            YouWin();
        }

    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MenuScene_Main"); // Replace with your main menu scene name
        mainMenuButton.SetActive(false);
    }
    private void UpdateTimerDisplay()
    {
        if (timerText != null) // Check for null to prevent NullReferenceException
        {
            timerText.text = Mathf.Ceil(timer).ToString() + "s";
        }
    }

    private void CheckGameEndCondition()
    {
        if (timer <= 0 && enemies > 0)
        {
            YouLoseText.gameObject.SetActive(true);
            playAgainButton.SetActive(true);
            mainMenuButton.SetActive(true);
        }
    }

    private void YouWin()
    {
        YouWinText.gameObject.SetActive(true);
        playAgainButton.SetActive(true);
        mainMenuButton.SetActive(true);
    }
   
    public void PlayAgain()
    {
        playAgainButton.SetActive(false); // Hide the button when starting a new game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
