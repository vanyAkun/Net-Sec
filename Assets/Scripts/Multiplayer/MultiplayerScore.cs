using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Photon.Pun;
using UnityEngine.UI;


public class MultiplayerScore : MonoBehaviourPunCallbacks
{
    public GameObject playerScorePrefab;
    public Transform panel;

    Dictionary<int, GameObject> playerScore = new Dictionary<int, GameObject>();
    void Start()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            player.SetScore(0);
            var playerScoreObject = Instantiate(playerScorePrefab, panel);
            var playerScoreObjectText = playerScoreObject.GetComponent<Text>();
            playerScoreObjectText.text = string.Format("{0} Kills: {1}", player.NickName, player.GetScore());

            playerScore[player.ActorNumber] = playerScoreObject;
        }
    }
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        var playerScoreObject = playerScore[targetPlayer.ActorNumber];
        var playerScoreObjectText = playerScoreObject.GetComponent<Text>();
        playerScoreObjectText.text = string.Format ("{0} Kills: {1}", targetPlayer.NickName, targetPlayer.GetScore());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
