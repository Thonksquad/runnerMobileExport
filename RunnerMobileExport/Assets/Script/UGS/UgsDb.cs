using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using UnityEngine;


public class UgsDb : MonoBehaviour
{

    const string leaderboardId = "leaderboard";
    const string coinsId = "coins";

    public static UgsDb Instance;


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

    public async void addCoins()
    {
        var playerEntry = await LeaderboardsService.Instance
            .AddPlayerScoreAsync(coinsId, GameManager.coins
            );
        Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }

    public async Task<string> getCoins()
    {
        var scoreResponse = await LeaderboardsService.Instance
        .GetPlayerScoreAsync(coinsId);
        string res = JsonConvert.SerializeObject(scoreResponse).ToString();
        coinsResult coinRes = JsonConvert.DeserializeObject<coinsResult>(res);

        return coinRes.score.ToString();
    }

}


public class coinsResult
{
    public string playerId { get; set; }
    public string playerName { get; set; }
    public int rank { get; set; }
    public double score { get; set; }
    public DateTime updatedTime { get; set; }
}
