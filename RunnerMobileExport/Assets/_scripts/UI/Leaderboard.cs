using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Exceptions;
using UnityEngine;
using UnityEngine.EventSystems;



public class Leaderboard : MonoBehaviour, IPointerDownHandler
{

    const string leaderboardId = "leaderboard";

    public Color playerColor;
    public Color defaultColor;

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

        Debug.Log("Accessing LB");
        try
        {
            var scoresResponse1t5 = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, new GetScoresOptions { Limit = 5 });
            var rangeLimit = 2;
            var scoresResponse6t10 = await LeaderboardsService.Instance.GetPlayerRangeAsync(leaderboardId, new GetPlayerRangeOptions { RangeLimit = rangeLimit });

            string top5 = JsonConvert.SerializeObject(scoresResponse1t5);
            string around5 = JsonConvert.SerializeObject(scoresResponse6t10);

            Root first5 = JsonConvert.DeserializeObject<Root>(top5);
            Root second5 = JsonConvert.DeserializeObject<Root>(around5);

            int i = 0;

            foreach (Result res in first5.results)
            {
                topTxtRanks[i].text = (res.rank + 1).ToString();
                topTxtNames[i].text = res.playerName;
                topTxtScores[i].text = res.score.ToString();
                if (res.playerId == PlayerPrefs.GetString("ugsPlayerIds"))
                {
                    topTxtRanks[i].color = playerColor;
                    topTxtNames[i].color = playerColor;
                    topTxtScores[i].color = playerColor;
                }
                else
                {
                    topTxtRanks[i].color = defaultColor;
                    topTxtNames[i].color = defaultColor;
                    topTxtScores[i].color = defaultColor;
                }
                Debug.Log("" + (res.rank + 1).ToString() + " -- " + res.playerName + " -- " + res.score.ToString());
                i++;
            }

            i = 0;
            foreach (Result res in second5.results)
            {
                aroundTxtRanks[i].text = (res.rank + 1).ToString();
                aroundTxtNames[i].text = res.playerName;
                aroundTxtScores[i].text = res.score.ToString();
                if (res.playerId == PlayerPrefs.GetString("ugsPlayerIds"))
                {
                    aroundTxtRanks[i].color = playerColor;
                    aroundTxtNames[i].color = playerColor;
                    aroundTxtScores[i].color = playerColor;
                }
                else
                {
                    aroundTxtRanks[i].color = defaultColor;
                    aroundTxtNames[i].color = defaultColor;
                    aroundTxtScores[i].color = defaultColor;
                }

                if( res.rank < 10 && res.playerId == PlayerPrefs.GetString("ugsPlayerIds"))
                {
                    show10results();
                    break;
                }

                Debug.Log("" + (res.rank + 1).ToString() + " -- " + res.playerName + " -- " + res.score.ToString());
                i++;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            //Print out the top 10 scores
            var responseTop10 = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, new GetScoresOptions { Limit = 10 });
            string top10 = JsonConvert.SerializeObject(responseTop10);
            Root first10 = JsonConvert.DeserializeObject<Root>(top10);
            
            int i = 0;
            
            foreach (Result res in first10.results)
            {
                if( i <= 4 )
                {
                    topTxtRanks[i].text = (res.rank + 1).ToString();
                    topTxtNames[i].text = res.playerName;
                    topTxtScores[i].text = res.score.ToString();
                }
                else
                {
                    aroundTxtRanks[i - 5].text = (res.rank + 1).ToString();
                    aroundTxtNames[i - 5].text = res.playerName;
                    aroundTxtScores[i - 5].text = res.score.ToString();
                }
                Debug.Log("" + (res.rank + 1).ToString() + " -- " + res.playerName + " -- " + res.score.ToString());
                i++;
            }
        }
    }

    public async void show10results()
    {

        try
        {
            var scoresResponse1t5 = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId,
                new GetScoresOptions { Offset = 5, Limit = 5 });
            
            string top5 = JsonConvert.SerializeObject(scoresResponse1t5);


            Root first5 = JsonConvert.DeserializeObject<Root>(top5);


            int i = 0;

            foreach (Result res in first5.results)
            {
                aroundTxtRanks[i].text = (res.rank + 1).ToString();
                aroundTxtNames[i].text = res.playerName;
                aroundTxtScores[i].text = res.score.ToString();
                if (res.playerId == PlayerPrefs.GetString("ugsPlayerIds"))
                {
                    aroundTxtRanks[i].color = playerColor;
                    aroundTxtNames[i].color = playerColor;
                    aroundTxtScores[i].color = playerColor;
                }
                else
                {
                    aroundTxtRanks[i].color = defaultColor;
                    aroundTxtNames[i].color = defaultColor;
                    aroundTxtScores[i].color = defaultColor;
                }
                Debug.Log("" + (res.rank + 1).ToString() + " -- " + res.playerName + " -- " + res.score.ToString());
                i++;
            }

        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
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

