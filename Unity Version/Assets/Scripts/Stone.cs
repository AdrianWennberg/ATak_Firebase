using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public BoardPosition Position { get; protected set; }
    public int Height { get; protected set; }
    public bool isWhite;
    public StoneType StoneType { get; protected set;  }

    // Adding a stone object to a game object.
    public static Stone AddStone(GameObject obj, BoardPosition pos, bool isWhite, StoneType type = StoneType.Flat)
    {
        Stone stone = obj.AddComponent<Stone>();
        stone.SetPosition(pos);
        stone.isWhite = isWhite;
        stone.StoneType = type;

        if (type == StoneType.Standing)
            stone.transform.Rotate(new Vector3(90, 45, 0));

        return stone;
    }

    // Sets the position of a stone.
    public void SetPosition(BoardPosition pos, int height = 0)
    {
        this.Position = pos;
        this.Height = height;

        Vector3 newPos = Vector3.zero;
        newPos.x = pos.x + 0.5f;
        newPos.z = pos.y + 0.5f;
        newPos.y = BoardManager.Instance.Pieces[0].gameObject.transform.localScale.y * height;
        newPos += new Vector3(0, GetComponent<Renderer>().bounds.size.y / 2, 0);

        transform.position = newPos;
    }

    // Sets the position of a stone that is being moved and is currently in the air.
    public void SetPositionFloating(BoardPosition pos, int groundHeight, int airHeight)
    {
        SetPosition(pos, groundHeight + airHeight + 4);
        this.Height = -1;
    }

    // Flattens a standing stone.
    public void Flatten()
    {
        transform.rotation = new Quaternion();
        StoneType = StoneType.Flat;
        SetPosition(this.Position);
    }
}
