using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHighlights : MonoBehaviour {

	public static BoardHighlights Instance { get; set; }

    public GameObject HighlightPrefab;
    private List<GameObject> hightlights;


    private void Start()
    {
        Instance = this;
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

    public void HideHighlights()
    {
        foreach(GameObject go in hightlights)
        {
            go.SetActive(false);
        }
    }


}
