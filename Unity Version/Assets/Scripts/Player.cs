using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player{
    
    private string playerName;
    private int stonesLeft;
    private int capstonesLeft;

    private Dictionary<int, int> stoneNumbers = new Dictionary<int, int>
    {
        { 3, 10 },
        { 4, 15 },
        { 5, 21 },
        { 6, 30 },
        { 8, 50 }
    };

    public Player(string pName, int boardSize)
    {
        playerName = pName;
        stonesLeft = stoneNumbers[boardSize];
        capstonesLeft = (boardSize - 3) / 2;
    }

    public bool TakeStone()
    {
        return (--stonesLeft > 0);
    }

    public bool HasCapstone()
    {
        return capstonesLeft > 0;
    }

    public void TakeCapstone()
    {
        capstonesLeft--;
    }

    public int GetStonesLeft()
    {
        return stonesLeft;
    }

    public int GetCapstonesLeft()
    {
        return capstonesLeft;
    }

    public string GetName()
    {
        return playerName;
    }
}
