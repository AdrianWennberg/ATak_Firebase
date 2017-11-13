using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Image bgImage;
    private Color bgColor;

    private int stonesLeft;
    private int capstonesLeft;
    [SerializeField] private bool IsWhite;
    private string playerName;

    static Dictionary<int, int> stoneNumbers = new Dictionary<int, int>
    {
        { 3, 10 },
        { 4, 15 },
        { 5, 21 },
        { 6, 30 },
        { 8, 50 }
    };

    private void Start()
    {
        if (Client.Instance != null)
        {
            IsWhite = IsWhite == Client.Instance.isWhite;
            playerName = Client.Instance.players[IsWhite ? 0 : 1].name;
        }
        else
            playerName = IsWhite ? "White" : "Black";

        bgImage = transform.Find("Background").GetComponent<Image>();

        // White player set to green
        // black player set to red.
        bgColor = new Color(IsWhite ? 0 : 255, IsWhite ? 255 : 0, 0);
        bgImage.color = bgColor;

        int boardSize = GameManager.Instance.BoardSize;
        stonesLeft = stoneNumbers[boardSize];
        capstonesLeft = (boardSize - 3) / 2;


        transform.Find("Stones").GetComponent<Text>().text = "Stones : " + stonesLeft.ToString();
        transform.Find("Capstones").GetComponent<Text>().text = "Capstones : " + capstonesLeft.ToString();
        transform.Find("PlayerName").GetComponent<Text>().text = playerName;
    }
    
    // Returns true if the player has more than one stone left.
    public bool NoStonesLeft()
    {
        return stonesLeft == 0;
    }

    // Takes a stone from the player and returns true if the player has any stones left.
    public void TakeStone()
    {
        stonesLeft--;
        transform.Find("Stones").GetComponent<Text>().text = "Stones : " + stonesLeft.ToString();
    }

    // Returns true if the player has any capstones left.
    public bool HasCapstone()
    {
        return capstonesLeft > 0;
    }

    // Takes a capstone from the player.
    public void TakeCapstone()
    {
        capstonesLeft--;
        transform.Find("Capstones").GetComponent<Text>().text = "Capstones : " + capstonesLeft.ToString();
    }


    // Sawps the color of the player display between red and green.
    public void SwapColor()
    {
        if (bgColor == new Color(255, 0, 0))
            bgColor = new Color(0, 255, 0);
        else
            bgColor = new Color(255, 0, 0);

        bgImage.color = bgColor;
    }
}
