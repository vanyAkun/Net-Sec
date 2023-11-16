using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerLevelManager : MonoBehaviourPunCallbacks
{
    //public int maxKills = 3; game finishes after the timer ends
    public GameObject gameOverPopup;
    public Text winnerText;
    public MultiplayerTimer multiplayerTimer;

    void Start()
    {
        PhotonNetwork.Instantiate("Multiplayer Player", Vector3.zero, Quaternion.identity);
    }
    public void OnTimerEnd()
    {
        Photon.Realtime.Player winner = DetermineWinner();
        winnerText.text = winner.NickName + " wins!";
        gameOverPopup.SetActive(true);
    }
    [PunRPC]
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        // Call the RPC on all clients to load the main menu
        photonView.RPC("LoadMainMenuScene", RpcTarget.All);
    }

    [PunRPC]
    void LoadMainMenuScene()
    {
        PhotonNetwork.Disconnect();
        //SceneManager.LoadScene("MenuScene_Main"); //   not needed as it calls the disconnect function.
    }
    private Photon.Realtime.Player DetermineWinner()
    {
        // Your logic to determine the winner
        // Example: player with the highest score
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
        return winner;
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
        SceneManager.LoadScene("MenuScene_Main");
    }
}

