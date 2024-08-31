
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class Leaderboard : MonoBehaviour, IPointerDownHandler
{

    const string leaderboardId = "leaderboard";

    public Color playerColor;

    public TextMeshProUGUI[] topTxtRanks;
    public TextMeshProUGUI[] topTxtNames;
    public TextMeshProUGUI[] topTxtScores;
    public TextMeshProUGUI[] aroundTxtRanks;
    public TextMeshProUGUI[] aroundTxtNames;
    public TextMeshProUGUI[] aroundTxtScores;



    private void OnEnable()
    {
        showLeaderBoard();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        //Application.OpenURL("http://leaderboard.dashbored.xyz/");
    }

    public async void showLeaderBoard()
    {
       


    }

}


public class Result
{
    public string playerId { get; set; }
    public string playerName { get; set; }
    public int rank { get; set; }
    public double score { get; set; }
}

public class Root
{
    public int limit { get; set; }
    public int total { get; set; }
    public List<Result> results { get; set; }
}

