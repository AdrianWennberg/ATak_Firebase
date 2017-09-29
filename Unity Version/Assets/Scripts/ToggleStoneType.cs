using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleStoneType : MonoBehaviour
{
    [SerializeField] private GameObject[] StoneTypes;
    [SerializeField] private ToggleStoneType otherColor;
    [SerializeField] private bool isWhite;

    void Update()
    {
        // If the current player is a different color, display the other color of stones.
        if (isWhite != BoardManager.Instance.IsWhite)
            SwitchColor();

        // Sets the current stone type chosen by the player to the active stone type.
        GameObject currentType = StoneTypes[(int)BoardManager.Instance.currentStone];

        // Rotates the active stone type by a small amount.
        if (BoardManager.Instance.CurrentGameState == GameState.OPPONENTS_TURN)
            return;

        currentType.transform.Rotate(new Vector3(0, Time.deltaTime * 180, 0), Space.World);

        // If the player has clicked check if they hit a stone type, if they did, set that stone type to the active stone type.
        if (Input.GetMouseButtonDown(0) && BoardManager.Instance.FirstTurn == false)
        {
            RaycastHit hit;
            if (Physics.Raycast(GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition), out hit, 25, LayerMask.GetMask("StoneTypeDisplay")))
            {
                currentType = hit.collider.gameObject;
                BoardManager.Instance.currentStone = (StoneType)Array.IndexOf(StoneTypes, currentType);
            }
        }
    }

    // Deactivates this game object and sets the opposite color one to active.
    private void SwitchColor()
    {
        otherColor.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
