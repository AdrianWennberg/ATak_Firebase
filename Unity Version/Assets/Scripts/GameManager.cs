using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public enum Menu { Main, NewGame, LocalGame, JoinGame }
    public Dictionary<String, Menu> MenuDict = new Dictionary<string, Menu>()
    {
        { "Main", Menu.Main },
        { "NewGame", Menu.NewGame },
        { "JoinGame", Menu.JoinGame },
        { "LocalGame", Menu.LocalGame }
    };
    public static GameManager Instance = null;

    public int BoardSize { get; set; }
    public bool OfficialRules = false;

    public GameObject MainMenu;
    public GameObject NewGameMenu;
    public GameObject JoinGameMenu;
    public GameObject LocalGameMenu;


    public GameObject clientPrefab;

    public InputField nameInput;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        GoToMenu("Main");
        
        //Client c = Instantiate<Client>(new GameObject().AddComponent<Client>());
        //c.ConnectToServer("127.0.0.1", 6321);
    }

    public void GoToMenu(String menu)
    {
        if (MenuDict.ContainsKey(menu) == false)
            return;

        Menu m = MenuDict[menu];

        MainMenu.SetActive(m == Menu.Main ? true : false);
        NewGameMenu.SetActive(m == Menu.NewGame ? true : false);
        JoinGameMenu.SetActive(m == Menu.JoinGame ? true : false);
        LocalGameMenu.SetActive(m == Menu.LocalGame ? true : false);
    }
    
    public void ConnectToServerButton()
    {

        try
        {

        }
        catch (Exception e)
        {
            Debug.Log("Connect error: " + e.Message);
        }
    }
    public void StartLocalGameButton()
    {
        StartLocalGameButton(true, 5);
    }

    public void StartLocalGameButton(bool officialRules = true, int boardSize = 5)
    {
        this.OfficialRules = officialRules;
        this.BoardSize = boardSize;
        //Destroy(Client.Instance);
        StartGame();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GameOver()
    {


    }
}
