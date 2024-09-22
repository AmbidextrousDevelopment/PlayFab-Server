using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentFriendsMenuScript : MonoBehaviour
{
    [SerializeField] private YourFriendPrefab friendsPrefab;
    [SerializeField] private GameObject yourFriendsContent;
    [SerializeField] private TextMeshProUGUI currentFriendsMenuTitleText;

    private void DestroyAllChildrenInScrollViewContent()
    {
        if (yourFriendsContent.transform.childCount == 0) return;

        for (int i = 0; i < yourFriendsContent.transform.childCount; i++)
        {
            Destroy(yourFriendsContent.transform.GetChild(i).gameObject);
        }
    }

    private void GetFriendsAndLoadThemOnScrollView()
    {
        var getFriendRequest = new GetFriendsListRequest();

        PlayFabClientAPI.GetFriendsList
        (
            getFriendRequest,
            result =>
            {
                //Loading all of the friends
                foreach (FriendInfo friendInfo in result.Friends)
                {
                    //Handling requestee friends
                    if (friendInfo.Tags[0] == "requestee")
                    {
                        var yourFriendPrefab = Instantiate(this.friendsPrefab,
                            yourFriendsContent.transform).gameObject;

                        //Getting yourFriendPrefab Script
                        var yourFriendPrefabScript = yourFriendPrefab.GetComponent<YourFriendPrefab>();

                        //Setting yourFriendPrefab Prefab Script Variables
                        yourFriendPrefabScript.SetFriendAndPlayfabIdAndStatusName
                        (friendInfo.Username, friendInfo.FriendPlayFabId, "(Pending)");
                    }

                    if (friendInfo.Tags[0] == "confirmed")
                    {
                        var yourFriendPrefab = Instantiate(this.friendsPrefab,
                            yourFriendsContent.transform).gameObject;

                        //Getting yourFriendPrefab Script
                        var yourFriendPrefabScript = yourFriendPrefab.GetComponent<YourFriendPrefab>();

                        //Setting yourFriendPrefab Prefab Script Variables
                        yourFriendPrefabScript.SetFriendAndPlayfabIdAndStatusName
                        (friendInfo.Username, friendInfo.FriendPlayFabId, "(Confirmed)");
                    }
                }
            },
            error =>
            {
                currentFriendsMenuTitleText.text = error.ErrorMessage;
            }
        );
    }

    private void OnEnable()
    {
        currentFriendsMenuTitleText.text = "Your Friends";
        DestroyAllChildrenInScrollViewContent();
        GetFriendsAndLoadThemOnScrollView();
    }
}
