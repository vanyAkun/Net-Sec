using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Reflection;
using TMPro;

public class MultiplayerLevelManager : MonoBehaviourPunCallbacks
{
    //public int maxKills = 3; game finishes after the timer ends
    public GameObject gameOverPopup;
    public Text winnerText;
    public MultiplayerTimer multiplayerTimer;
    public TextMeshProUGUI notificationText;

    void Start()
    {
        PhotonNetwork.Instantiate("Multiplayer Player", Vector3.zero, Quaternion.identity);
    }
    public void OnTimerEnd()
    {
        Photon.Realtime.Player winner = DetermineWinner();

        if (winner != null && winner.GetScore() > 0)
        {
            // If there's a winner with at least one kill
            winnerText.text = winner.NickName + " wins!";
        }
        else
        {
            // If no one has any kills
            winnerText.text = "Nobody won!";
        }

        gameOverPopup.SetActive(true);
    }

    [PunRPC]
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        ShowNotification($"{otherPlayer.NickName} has left the game");

    } 
       // photonView.RPC("LoadMainMenuScene", RpcTarget.All); this would send everybody to the main memu }

    [PunRPC]
    /*void LoadMainMenuScene()
    {
        PhotonNetwork.Disconnect();
        //SceneManager.LoadScene("MenuScene_Main"); //   not needed as it calls the disconnect function.
    }*/
    private Photon.Realtime.Player DetermineWinner()
    {
        int highestScore = 0;
        Photon.Realtime.Player winner = null;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.GetScore() > highestScore)
            {
                highestScore = player.GetScore();
                winner = player;
            }
        }
        return highestScore > 0 ? winner : null;
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("GameEnded") && (bool)propertiesThatChanged["GameEnded"])
        {
            OnTimerEnd();
        }
    }
    /* public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
     {
         if (targetPlayer.GetScore() > maxKills)
         {
             winnerText.text = targetPlayer.NickName;
             gameOverPopup.SetActive(true);
         }
     }*/
    [PunRPC]
    public void EndGame()
    {
        OnTimerEnd();
    }
    public void OnPlayAgainClicked()
    {
        // Call the RPC on all clients to reset the game
        photonView.RPC("ResetGame", RpcTarget.All);
    }
    [PunRPC]
    void ResetGame()
    {
        // Reset timer, scores, and any other game states here
        ResetTimer();
        ResetScores();

        // Optionally, you can also hide the game over popup
        gameOverPopup.SetActive(false);
    }
    void ResetTimer()
    {
        if (multiplayerTimer != null)
        {
            multiplayerTimer.StartTimer(); // Reset and start the timer
        }
    }
    void ResetScores()
    {
        // Reset player scores here
        // Example: Loop through players and reset their scores
        foreach (var player in PhotonNetwork.PlayerList)
        {
            player.SetScore(0);
        }
    }
    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();

    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        string message = cause == DisconnectCause.DisconnectByClientLogic ?
                         "You have left the game" : "Connection lost";
        ShowNotification(message);
        Invoke("ClearNotification", 2.0f);
        SceneManager.LoadScene("MenuScene_Main"); // Redirect to main menu
        
    }
    private void ShowNotification(string message)
    {
        if (notificationText != null)
            notificationText.text = message;
        Invoke("ClearNotification", 2.0f);
        // Additional logic for displaying the notification
    }
    private void ClearNotification()
    {
        if (notificationText != null)
            notificationText.text = ""; // Clear the notification text
    }
}

