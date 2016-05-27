using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DisplayController : MonoBehaviour
{

    Text mineralsText;
    private int totalPoints;

    public void Awake()
    {
        mineralsText = gameObject.GetComponentInChildren<Text>();
        totalPoints = 0;
        MineralPointsUp(0);
    }

    public void ClearPoints()
    {
        totalPoints = 0;
    }

    public void MineralPointsUp(int points)
    {
        totalPoints += points;
        mineralsText.text = "Minerals : \n" + totalPoints.ToString();
    }
}
