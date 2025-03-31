using TMPro;
using UnityEngine;

public class TurnUIController : MonoBehaviour
{
    public TextMeshProUGUI turnInfoText;
    public TextMeshProUGUI playerSideText;

    public void SetPlayerSide(string side)
    {
        playerSideText.text = $"You are playing as {side}";
    }

    public void SetTurnInfo(string side)
    {
        turnInfoText.text = $"{side}'s Turn";
    }
}
