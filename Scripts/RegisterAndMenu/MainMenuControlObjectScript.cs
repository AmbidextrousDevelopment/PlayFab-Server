using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuControlObjectScript : MonoBehaviour
{
    #region Main Menu container functions
    private void OnMainMenuLeaderboardButtonClicked()
    {
        leaderboardSelectionReference.SetActive(true);
        mainMenuReference.SetActive(false);
    }
    private void OnMainMenuFriendsMenudButtonClicked()
    {
        friendsMenuReference.SetActive(true);
        mainMenuReference.SetActive(false);
    }
    private void BackToMainMenuButtonClicked()
    {
        leaderboardWinsReference.SetActive(false);
        leaderboardRecordsReference.SetActive(false);
        friendsMenuReference.SetActive(false);
        leaderboardSelectionReference.SetActive(false);
        selectionRecordsMenuReference.SetActive(false);
        friendsCurrentMenuReference.SetActive(false);
        friendsRequestMenuReference.SetActive(false);

        mainMenuReference.SetActive(true);
    }
    private void BackToFriendsMenuButtonClicked()
    {
        searchNewFriendsMenuReference.SetActive(false);
        friendsCurrentMenuReference.SetActive(false);
        friendsRequestMenuReference.SetActive(false);

        friendsMenuReference.SetActive(true);
    }
    private void SetupContainerMenuButtonListeners()
    {
        AddListenersToButtonsThatLeadsToMainMenuOrFriendsMenu();

        leaderboardsButton.onClick.AddListener(OnMainMenuLeaderboardButtonClicked);
        friendsMenuButton.onClick.AddListener(OnMainMenuFriendsMenudButtonClicked);

        leaderboardSelectionWInsButton.onClick.AddListener(OnLeaderBoardSelectionWinsButtonClicked);
        leaderboardSelectionRecordsButton.onClick.AddListener(OnLeaderBoardSelectionTimesButtonClicked);

        selectionRecordsOneButton.onClick.AddListener(OnRecordSelectionMenuLevelOneButtonClicked);
        selectionRecordsTwoButton.onClick.AddListener(OnRecordSelectionMenuLevelTwoButtonClicked);

        //mainMenuFriendsButton.onClick.AddListener(OnMainMenuFriendsButtonClicked);
        yourCurrentFriendsButton.onClick.AddListener(OnYourFriendsButtonClicked);
        yourRequestButton.onClick.AddListener(OnYourRequestButtonClicked);
        searchNewFriendsButton.onClick.AddListener(OnSearchNewFriendsButtonClicked);
        MainMenuGoOnlineButton.onClick.AddListener(OnGoOnlineButtonClicked);
    }
    private void AddListenersToButtonsThatLeadsToMainMenuOrFriendsMenu()
    {
        leaderBoardWinsBackButton.onClick.AddListener(BackToMainMenuButtonClicked);
        leaderBoardSelectionBackButton.onClick.AddListener(BackToMainMenuButtonClicked);
        selectionRecordsBackButton.onClick.AddListener(BackToMainMenuButtonClicked);
        leaderBoardRecordsBackButton.onClick.AddListener(BackToMainMenuButtonClicked);
        friendsMenuBackButton.onClick.AddListener(BackToMainMenuButtonClicked);

        friendsRequestBackButton.onClick.AddListener(BackToFriendsMenuButtonClicked);
        friendsCurrentBackButton.onClick.AddListener(BackToFriendsMenuButtonClicked);
        searchNewFriendsBackButton.onClick.AddListener(BackToFriendsMenuButtonClicked);
    }

    private void OnRecordSelectionMenuLevelOneButtonClicked()
    {
        leaderboardRecordTimesScript.statisticNameForLoadingLeaderboard = "Level 1 Record Times";

        selectionRecordsMenuReference.SetActive(false);
        leaderboardRecordsReference.SetActive(true);
    }
    private void OnRecordSelectionMenuLevelTwoButtonClicked()
    {
        leaderboardRecordTimesScript.statisticNameForLoadingLeaderboard = "Level 2 Record Times";

        selectionRecordsMenuReference.SetActive(false);
        leaderboardRecordsReference.SetActive(true);
    }

    private void OnLeaderBoardSelectionWinsButtonClicked()
    {
        leaderboardWinsReference.SetActive(true);
        leaderboardSelectionReference.SetActive(false);
    }
    private void OnLeaderBoardSelectionTimesButtonClicked()
    {
        selectionRecordsMenuReference.SetActive(true);
        leaderboardSelectionReference.SetActive(false);
    }

    private void OnSearchNewFriendsButtonClicked()
    {
        searchNewFriendsMenuReference.SetActive(true);
        friendsMenuReference.SetActive(false);
    }
    private void OnYourRequestButtonClicked()
    {
        friendsRequestMenuReference.SetActive(true);
        friendsMenuReference.SetActive(false);
    }
    private void OnYourFriendsButtonClicked()
    {
        friendsCurrentMenuReference.SetActive(true);
        friendsMenuReference.SetActive(false);
    }

    private void OnGoOnlineButtonClicked()
    {
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    #endregion

    #region References to script containers, objects and scripts
    [Header("References to script containers, objects and scripts")]
    [SerializeField] private GameObject mainMenuReference;
    [SerializeField] private GameObject friendsMenuReference;
    [SerializeField] private GameObject friendsCurrentMenuReference;
    [SerializeField] private GameObject friendsRequestMenuReference;
    [SerializeField] private GameObject searchNewFriendsMenuReference;
    [SerializeField] private GameObject leaderboardWinsReference;
    [SerializeField] private GameObject leaderboardRecordsReference;
    [SerializeField] private GameObject leaderboardSelectionReference;
    [SerializeField] private GameObject selectionRecordsMenuReference;
    [Header("Main Menu Button References")]
    [SerializeField] private Button leaderboardsButton;
    [SerializeField] private Button friendsMenuButton;
    [SerializeField] private Button MainMenuQuitButton;
    [SerializeField] private Button MainMenuGoOnlineButton;
    [Header("Leaderboard Selection Button References")]
    [SerializeField] private Button leaderboardSelectionWInsButton;
    [SerializeField] private Button leaderboardSelectionRecordsButton;
    [Header("Selection Records Menu Button References")]
    [SerializeField] private Button selectionRecordsOneButton;
    [SerializeField] private Button selectionRecordsTwoButton;
    [SerializeField] private LeaderboardRecordTimesScript leaderboardRecordTimesScript;
    [Header("Friends Menu References Buttons")]
    [SerializeField] private Button searchNewFriendsButton;
    [SerializeField] private Button yourRequestButton;
    [SerializeField] private Button yourCurrentFriendsButton;
    [Header("Back Buttons References")]
    [SerializeField] private Button leaderBoardWinsBackButton;
    [SerializeField] private Button leaderBoardRecordsBackButton;
    [SerializeField] private Button friendsMenuBackButton;
    [SerializeField] private Button friendsRequestBackButton;
    [SerializeField] private Button friendsCurrentBackButton;
    [SerializeField] private Button searchNewFriendsBackButton;
    [SerializeField] private Button leaderBoardSelectionBackButton;
    [SerializeField] private Button selectionRecordsBackButton;

    #endregion

    #region Unity Functions
    private void Start()
    {
        SetupContainerMenuButtonListeners();
        BackToMainMenuButtonClicked();
    }
    #endregion
}
