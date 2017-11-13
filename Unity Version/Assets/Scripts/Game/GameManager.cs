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

        
    }

    private void Start()
    {
        Client c = null;
        try
        {
            c = Instantiate(new GameObject().AddComponent<Client>());
            c.ConnectToServer("127.0.0.1", 6321);
        }
        catch (Exception e)
        {
            Debug.Log("Connect error: " + e.Message);
        }

        if (c == null || c.SocketReady == false)
        {
            DisableOnline();
        }
        else
        {
            GameObject.Find("ConnectToServerButton").SetActive(false);
        }
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

    private void DisableOnline()
    {
        GameObject.Find("JoinGameButton").GetComponent<Button>().interactable = false;
        GameObject.Find("NewGameButton").GetComponent<Button>().interactable = false;
    }

    public void RefreshOnline()
    {

        Client c = null;
        if (Client.Instance == null)
        {
            c = Instantiate<Client>(new GameObject().AddComponent<Client>());
        }
        else
        {
            c = Client.Instance;
        }

        if (c != null && c.SocketReady == false)
        {
            try
            {
                c.ConnectToServer("127.0.0.1", 6321);
            }
            catch (Exception e)
            {
                Debug.Log("Connect error: " + e.Message);
            }
        }
        if (c == null || c.SocketReady == false)
            return;


        GameObject.Find("JoinGameButton").GetComponent<Button>().interactable = true;
        GameObject.Find("NewGameButton").GetComponent<Button>().interactable = true;
        GameObject.Find("ConnectToServerButton").SetActive(false);
    }

    public void ConnectToServerButton()
    {

        
    }

    public void StartLocalGame(int boardSize, bool officialRules)
    {
        this.OfficialRules = officialRules;
        this.BoardSize = boardSize;
        Destroy(Client.Instance);
        SceneManager.LoadScene("GameScene");
    }

    public void CreateNewGame(int boardSize, bool officialRules, bool whiteStart)
    {


        this.OfficialRules = officialRules;
        this.BoardSize = boardSize;
    }

    public void GameOver()
    {


    }
}
