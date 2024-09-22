using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardPrefabScrollviewScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankTextReference;
    [SerializeField] private TextMeshProUGUI userNameTextReference;
    [SerializeField] private TextMeshProUGUI scoreTextReference;


    public void SetValuesForLeaderboardPrefab(string rank, string username, string score)
    {
        rankTextReference.text = rank;
        userNameTextReference.text = username;
        scoreTextReference.text = score;
    }
}
