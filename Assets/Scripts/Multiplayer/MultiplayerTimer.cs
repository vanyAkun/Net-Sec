using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon; // Add this line
using TMPro;

public class MultiplayerTimer : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI timerText;
    private float remainingTime;
    public bool timerIsRunning = false;
    private const float TimerDuration = 10f;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartTimer();
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && timerIsRunning)
        {
            remainingTime -= Time.deltaTime;
            UpdateRoomTimerProperty(remainingTime);

            if (remainingTime <= 0)
            {
                timerIsRunning = false;
                OnTimerFinished(); // Call this method when the timer ends
            }
        }
    }

    public void StartTimer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            remainingTime = TimerDuration;
            timerIsRunning = true;
            UpdateRoomTimerProperty(remainingTime);
            UpdateTimerDisplay(); // Ensure the timer display is updated when the timer starts
        }
    }
    void OnTimerFinished()
    {
        var props = new ExitGames.Client.Photon.Hashtable
    {
        { "GameEnded", true }
    };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }




    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("Timer"))
        {
            remainingTime = (float)propertiesThatChanged["Timer"];
            UpdateTimerDisplay();

            if (remainingTime <= 0)
            {
                timerIsRunning = false;
                // Handle timer end logic
            }
        }
    }
    void UpdateTimerDisplay()
    {
        timerText.text = FormatTime(remainingTime);
    }
    string FormatTime(float time)
    {
        return string.Format("{0:00}", (int)time);
    }
    void UpdateRoomTimerProperty(float time)
    {
        var props = new ExitGames.Client.Photon.Hashtable
        {
            { "Timer", time }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }
}