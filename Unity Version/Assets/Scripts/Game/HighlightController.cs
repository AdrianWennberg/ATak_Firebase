using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightController : MonoBehaviour {

	public static HighlightController Instance { get; protected set; }

    public GameObject HighlightPrefab;
    private List<GameObject> hightlights;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        hightlights = new List<GameObject>();
    }

    private GameObject GetHighlightObject()
    {
        GameObject go = hightlights.Find(g => !g.activeSelf);

        if(go == null)
        {
            go = Instantiate(HighlightPrefab);
            hightlights.Add(go);
        }

        return go;
    }

    public void HighlightAllowedMoves(bool[,] moves, int boardSize)
    {
        HideHighlights();
        for (int i = 0; i < boardSize; i++)
        {
            for(int j = 0; j < boardSize; j++)
            {
                if(moves[i,j])
                {
                    GameObject go = GetHighlightObject();
                    go.transform.position = new Vector3((i + 0.5f), 0.001f, (j + 0.5f));
                    
                    go.SetActive(true);
                }
            }
        }
    }

    public void HiglightMousePosition()
    {
        HideHighlights();
        if (MouseController.IsValidPosition() == false)
            return;

        BoardPosition pos = MouseController.CurrentPosition;
        Stone posStone = BoardManager.Instance.GetTopStone(pos);

        // If there is a stone in this position and
        // it is not your stone or
        // it is the frst turn, do not highlight this slot.
        if (posStone != null &&
            (posStone.isWhite != BoardManager.Instance.IsWhite ||
            BoardManager.Instance.FirstTurn))
            return;

        GameObject go = GetHighlightObject();
        go.transform.position = new Vector3(pos.x + (0.5f), 0.001f, pos.y + (0.5f));
        go.SetActive(true);
    }

    public void HideHighlights()
    {
        foreach(GameObject go in hightlights)
        {
            go.SetActive(false);
        }
    }
}
