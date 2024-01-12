using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour, IPointerDownHandler
{

    LeaderboardClass leaderboards = new LeaderboardClass();
    public Text highScores;


    private void OnEnable()
    {
        showLeaderBoard();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        Application.OpenURL("http://leaderboard.dashbored.xyz/");
    }

    public async void showLeaderBoard()
    {
        highScores.text = await leaderboards.GetScores();
    }


}
