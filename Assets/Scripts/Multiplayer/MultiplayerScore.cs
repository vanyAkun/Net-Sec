using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Photon.Pun;
using UnityEngine.UI;

// callback == piece of code(usually a function) that is provided to another piece of code to be executed later under certain conditions??okay
public class MultiplayerScore : MonoBehaviourPunCallbacks
{
    public GameObject playerScorePrefab;
    public Transform panel;

    Dictionary<int, GameObject> playerScore = new Dictionary<int, GameObject>();//dictyonary that matches the players ID with the scoregameobject
    void Start()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            player.SetScore(0);
            var playerScoreObject = Instantiate(playerScorePrefab, panel);
            var playerScoreObjectText = playerScoreObject.GetComponent<Text>();
            playerScoreObjectText.text = string.Format("{0} Kills: {1}", player.NickName, player.GetScore());//sets nickname and score

            playerScore[player.ActorNumber] = playerScoreObject;//Adds players score GameObject to the playerScore dictionary (ActorNumber==ID basically)
        }
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)//callback==piece of code (usually a function) that is provided to another piece of code to be executed later under certain conditions??okay
    {
        if (playerScore.ContainsKey(otherPlayer.ActorNumber))//checks actornumber(id,nick) is in the playerscore dictionary
        {
            // If the player who left is in the dictionary, remove their score display
            Destroy(playerScore[otherPlayer.ActorNumber]);
            playerScore.Remove(otherPlayer.ActorNumber);
        }
    }
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)//called when the properties of a player change (like the score.
    {
        var playerScoreObject = playerScore[targetPlayer.ActorNumber];
        var playerScoreObjectText = playerScoreObject.GetComponent<Text>();
        playerScoreObjectText.text = string.Format ("{0} Kills: {1}", targetPlayer.NickName, targetPlayer.GetScore());
    }
}
//override changes specific functionality, fr example, integrate custom code
