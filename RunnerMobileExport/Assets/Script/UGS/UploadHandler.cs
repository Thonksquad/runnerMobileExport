using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;


public class UploadHandler : MonoBehaviour
{

    const string leaderboardId = "leaderboard";
    const string coinsId = "coins";

    public static UploadHandler Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this)
            {
                Destroy(this);
            }
        }
    }

    public async void addScore()
    {
        var metadata = new Dictionary<string, string>() {
            { "gameLength", GameManager.gameLength.ToString() } ,
            { "enemiesKilled", GameManager.enemiesKilled.ToString() },
            { "speed", CameraManager.Instance.CamSpeed.ToString() }
        };
        var playerEntry = await LeaderboardsService.Instance
            .AddPlayerScoreAsync(leaderboardId, GameManager.distance,
            new AddPlayerScoreOptions { Metadata = metadata }
            );
        Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }

    public async Task<string> getPlayerScore()
    {
        string tRes = "0";
        Debug.Log("Grabbing player score");

        try
        {
            var dbResponse = await LeaderboardsService.Instance.GetPlayerScoreAsync(leaderboardId);
            string res = JsonConvert.SerializeObject(dbResponse).ToString();
            dbResult psRes = JsonConvert.DeserializeObject<dbResult>(res);
            tRes = psRes.score.ToString();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            tRes = "0";
        }

        return tRes;
    }

    public async void addCoins()
    {
        Debug.Log("add coins to db");
        try
        {
            var playerEntry = await LeaderboardsService.Instance
            .AddPlayerScoreAsync(coinsId, GameManager.coins
            );
            Debug.Log(JsonConvert.SerializeObject(playerEntry));
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
        
    }

    public async Task<string> getCoins()
    {
        string tRes = "0";
        Debug.Log("Grabbing player coins");

        try
        {
            var dbResponse = await LeaderboardsService.Instance.GetPlayerScoreAsync(coinsId);
            string res = JsonConvert.SerializeObject(dbResponse).ToString();
            dbResult coinRes = JsonConvert.DeserializeObject<dbResult>(res);
            tRes = coinRes.score.ToString();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            tRes = "0";
        }

        return tRes;
    }

}


public class dbResult
{
    public string playerId { get; set; }
    public string playerName { get; set; }
    public int rank { get; set; }
    public double score { get; set; }
    public DateTime updatedTime { get; set; }
}
