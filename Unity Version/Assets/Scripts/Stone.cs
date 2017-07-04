using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public int CurrentX { get; set; }
    public int CurrentY { get; set; }
    public int Height { get; set; }
    public bool isWhite;
    public BoardManager.StoneType stoneType;

    public void SetPosition(int x, int y)
    {
        CurrentX = x;
        CurrentY = y;
    }
}
