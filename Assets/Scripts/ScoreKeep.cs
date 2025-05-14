using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeep : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    // Start is called before the first frame update

    // Update is called once per frame
    public void UpdateScore()
    {
        int totalRatings = GameManager.Instance.GetTotalRatings();
        int ratingValue = GameManager.Instance.GetRatingTotal();
        string outText = "Total Ratings:" + totalRatings + ", Rating Value: " + ratingValue;
        scoreText.text = outText;    //.ToString();
    }
}
