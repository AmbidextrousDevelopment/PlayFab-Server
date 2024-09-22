using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class LeaderboardRecordTimesScript : MonoBehaviour
{
    #region Functions For Loading Leaderboards
    public string statisticNameForLoadingLeaderboard = "";
    private void LoadGlobalRankLeaderboard()
    {
        //DestroyAllPreviousLeaderboardEntries();
        DisableInteractionComponentFromAllButtons();
        //Request to fetch Leaderboard from Playfab server
        var leaderboardFetchRequest = new GetLeaderboardRequest
        {
            StatisticName = statisticNameForLoadingLeaderboard,
            MaxResultsCount = 10,
        };

        PlayFabClientAPI.GetLeaderboard(
            leaderboardFetchRequest,
            result =>
            {
                var leaderboardResultList = result.Leaderboard;
                leaderboardResultList.Reverse();

                //PlayerLeaderboardEntry is Playfab class
                foreach (PlayerLeaderboardEntry playerLeaderboardEntry in leaderboardResultList)
                {
                    var spawnedLeaderboardEntry = Instantiate(leaderboardsPrefabReference,
                        leaderboardsScrollviewContentReference.transform);

                    LeaderboardPrefabScrollviewScript spawnedLeaderboardEntryScript =
                    spawnedLeaderboardEntry.GetComponent<LeaderboardPrefabScrollviewScript>();

                    //Processing Seconds and Converting to Minutes + Seconds
                    var statValue = playerLeaderboardEntry.StatValue;
                    var minutesInStats = statValue / 60;
                    var secondsInStats = statValue % 60;

                    //Processing the rank of the player
                    var currentPlayersPosition = leaderboardResultList.Count - playerLeaderboardEntry.Position;

                    //Position, DisplayName and StatValue are variables from PlayerLeaderboardEntry Playfab class
                    spawnedLeaderboardEntryScript.SetValuesForLeaderboardPrefab(
                        currentPlayersPosition.ToString(),
                        playerLeaderboardEntry.DisplayName,
                         $"{minutesInStats:00} : {secondsInStats:00}"
                        //playerLeaderboardEntry.StatValue.ToString()
                        );
                }

                EnableInteractionComponentFromAllButtons();
                leaderboardTitleTextMeshProReference.text = "Global Ranks";
            },
            error =>
            {
                leaderboardTitleTextMeshProReference.text = "Global Leaderboard failed to load";
                print(error.ErrorMessage);
                EnableInteractionComponentFromAllButtons();
            }
                                  );
    }

    private void LoadPlayerRankLeaderboard()
    {
        //DestroyAllPreviousLeaderboardEntries();
        DisableInteractionComponentFromAllButtons();
        //Request to fetch Leaderboard from Playfab server
        var leaderboardFetchRequest = new GetLeaderboardAroundPlayerRequest()
        {
            StatisticName = statisticNameForLoadingLeaderboard,
            MaxResultsCount = 10,
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(
            leaderboardFetchRequest,
            result =>
            {
                var leaderboardResultList = result.Leaderboard;
                leaderboardResultList.Reverse();

                //PlayerLeaderboardEntry is Playfab class
                foreach (PlayerLeaderboardEntry playerLeaderboardEntry in leaderboardResultList)
                {
                    var spawnedLeaderboardEntry = Instantiate(leaderboardsPrefabReference,
                        leaderboardsScrollviewContentReference.transform);

                    LeaderboardPrefabScrollviewScript spawnedLeaderboardEntryScript =
                    spawnedLeaderboardEntry.GetComponent<LeaderboardPrefabScrollviewScript>();

                    //Processing Seconds and Converting to Minutes + Seconds
                    var statValue = playerLeaderboardEntry.StatValue;
                    var minutesInStats = statValue / 60;
                    var secondsInStats = statValue % 60;

                    //Processing the rank of the player
                    var currentPlayersPosition = leaderboardResultList.Count - playerLeaderboardEntry.Position;

                    //Position, DisplayName and StatValue are variables from PlayerLeaderboardEntry Playfab class
                    spawnedLeaderboardEntryScript.SetValuesForLeaderboardPrefab(
                        currentPlayersPosition.ToString(),
                        playerLeaderboardEntry.DisplayName,
                        $"{minutesInStats:00} : {secondsInStats:00}"
                        //playerLeaderboardEntry.StatValue.ToString()
                        );
                }

                EnableInteractionComponentFromAllButtons();
                leaderboardTitleTextMeshProReference.text = "Player Ranks";
            },
            error =>
            {
                leaderboardTitleTextMeshProReference.text = "Global Leaderboard failed to load";
                print(error.ErrorMessage);
                EnableInteractionComponentFromAllButtons();
            }
                                  );
    }

    private void SetupAllListenersForButtons()
    {
        globalRankButton.onClick.AddListener(GlobalRankButtonClicked);
        currentPlayerRankButton.onClick.AddListener(PlayerRankButtonClicked);
    }

    ///<summary>
    ///This function is to destroy prefabs, that might be already inside content
    ///</summary>
    private void DestroyAllPreviousLeaderboardEntries()
    {
        if (leaderboardsScrollviewContentReference.transform.childCount == 0) return;
        for (int i = 0; i < leaderboardsScrollviewContentReference.transform.childCount; i++)
        {
            Destroy(leaderboardsScrollviewContentReference.transform.GetChild(i).gameObject);
        }

    }

    private void GlobalRankButtonClicked()
    {
        DestroyAllPreviousLeaderboardEntries();
        LoadGlobalRankLeaderboard();
    }
    private void PlayerRankButtonClicked()
    {
        DestroyAllPreviousLeaderboardEntries();
        LoadPlayerRankLeaderboard();
    }

    #endregion

    #region References To Objects and Components
    [Header("References To Objects and Components")]
    [SerializeField] private TextMeshProUGUI leaderboardTitleTextMeshProReference;
    [SerializeField] private GameObject leaderboardsScrollviewContentReference;
    [SerializeField] private GameObject leaderboardsPrefabReference;

    #endregion

    #region Button References
    [SerializeField] private Button globalRankButton;
    [SerializeField] private Button currentPlayerRankButton;
    [SerializeField] private Button friendsRankButton;

    [SerializeField] private Button recordWinsButton;
    [SerializeField] private Button recordTimesButton;
    [SerializeField] private Button backButton;

    private void DisableInteractionComponentFromAllButtons()
    {
        globalRankButton.interactable = false;
        currentPlayerRankButton.interactable = false;
        friendsRankButton.interactable = false;
        recordWinsButton.interactable = false;
        recordTimesButton.interactable = false;
        backButton.interactable = false;

    }
    private void EnableInteractionComponentFromAllButtons()
    {
        globalRankButton.interactable = true;
        currentPlayerRankButton.interactable = true;
        friendsRankButton.interactable = true;
        recordWinsButton.interactable = true;
        recordTimesButton.interactable = true;
        backButton.interactable = true;

    }

    #endregion

    #region Unity Functions
    private void OnEnable()
    {
        leaderboardTitleTextMeshProReference.text = "Leaderboards";
        DestroyAllPreviousLeaderboardEntries();
        LoadGlobalRankLeaderboard();
    }
    private void OnDisable()
    {
        DestroyAllPreviousLeaderboardEntries();
    }
    private void Start()
    {
        SetupAllListenersForButtons();
    }
    #endregion
}
