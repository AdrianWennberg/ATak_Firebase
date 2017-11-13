using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewGameMenu : MonoBehaviour {

    public Dropdown boardSizeDD;
    public Toggle isWhiteToggle;
    public Toggle officialRulesToggle;
	// Use this for initialization
	void OnEnable () {
        boardSizeDD.value = 2;
        officialRulesToggle.isOn = true;

        if(isWhiteToggle)
            isWhiteToggle.isOn = true;
    }

    public void StartGameButton()
    {
        int boardSize = boardSizeDD.value + 3;
        if (boardSize == 7) boardSize++;

        if (isWhiteToggle)
            GameManager.Instance.CreateNewGame(boardSize, officialRulesToggle.isOn, isWhiteToggle.isOn);
        else
            GameManager.Instance.StartLocalGame(boardSize, officialRulesToggle.isOn);
    }
}
