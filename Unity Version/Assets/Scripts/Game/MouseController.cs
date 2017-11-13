using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoardPosition
{
    public int x;
    public int y;

    public BoardPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public bool Equals(BoardPosition other)
    {
        return other.x == x && other.y == y;
    }
}

public static class MouseController
{
    static BoardPosition invalidPosition = new BoardPosition(-1, -1);
    public static BoardPosition CurrentPosition;
    
    public static BoardPosition GetInvalidPosition()
    {
        return invalidPosition;
    }

    public static void UpdateMousePosition()
    {

        if (!Camera.main)
        {
            Debug.LogError("No main Camera. This should never happen.");
            CurrentPosition = invalidPosition;
            return;
        }


        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("TakBoard")))
            CurrentPosition = new BoardPosition((int)(hit.point.x), (int)(hit.point.z));
        else
            CurrentPosition = invalidPosition;
    }

    public static bool IsValidPosition()
    {
        return CurrentPosition.Equals(invalidPosition) == false;
    }
}
