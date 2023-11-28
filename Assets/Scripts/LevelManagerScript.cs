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
    public float timer = 2f; //  countdown
    public Text YouWinText;
    public Text YouLoseText;
    public Player player;
    public Text KillcountText;
    public Text deathText;

    public GameObject playAgainButton;
    public GameObject mainMenuButton;
    private void Awake()
    {
        Player.OnPlayerKilled += UpdateDeathCountDisplay;
        enemiesText.text = enemies.ToString();

        Enemy.OnEnemyKilled += OnEnemyKilled;
      
        UpdateTimerDisplay();
        if (timerText != null)
        {
            UpdateTimerDisplay();
        }
        playAgainButton.SetActive(false);

    }
    private void UpdateDeathCountDisplay()
    {
        if (player != null && deathText != null) // Updated reference
        {
            deathText.text = player.respawnCount.ToString();
            Debug.Log("[LevelManager] Updated death count: " + player.respawnCount);
        }
        else
        {
            Debug.LogError("[LevelManager] Player or deathText is null");
        }
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
        else if (!playAgainButton.activeSelf) // Additional check to prevent multiple calls
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
            CheckGameEndCondition();
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
        timer = 0; // Stop the timer in any end game condition
        UpdateTimerDisplay(); // Update the timer display

        int enemiesKilled = 5 - enemies; // Calculate enemies killed
        KillcountText.text = "Enemies Killed: " + enemiesKilled.ToString();
        KillcountText.gameObject.SetActive(true); // Make sure the text is visible

        if (enemies > 0)
        {
            YouLoseText.gameObject.SetActive(true);
        }
        else
        {
            YouWinText.gameObject.SetActive(true);
        }

        playAgainButton.SetActive(true);
        mainMenuButton.SetActive(true);
    }
  
    public void PlayAgain()
    {
        playAgainButton.SetActive(false); // Hide the button when starting a new game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
