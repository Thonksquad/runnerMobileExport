const { LeaderboardsApi } = require("@unity-services/leaderboards-2.0");

module.exports = async ({ params, context, logger }) => {
  const { projectId } = context;
  const { playerId } = context;
  const leaderboardId = "leaderboard";
  const { score } = params;
  
  // Initialize the LeaderboardsApi using the context allows
  // for admin-level access to Leaderboards endpoints.
  const leaderboardsApi = new LeaderboardsApi(context);

  const addScoreResult = await leaderboardsApi.addLeaderboardPlayerScore(projectId, leaderboardId, playerId, { score: score });


  return { addScoreResultStatus: addScoreResult.status };
};