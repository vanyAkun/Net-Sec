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
    //public int maxKills = 3;
    public GameObject gameOverPopup;
    public Text winnerText;
    // Start is called before the first frame update
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

