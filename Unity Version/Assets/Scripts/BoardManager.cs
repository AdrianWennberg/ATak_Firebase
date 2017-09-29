using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StoneType { Flat, Standing, Capstone };

public enum GameState
{
    NULL_STATE = 0,
    OPPONENTS_TURN,
    TURN_START,
    PICKUP,
    PLACE_STONE,
    GAME_END,
};

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; protected set; }

    

    private Dictionary<GameState, Action> StateMachiene;

    public GameState CurrentGameState { get; protected set; }
    
    public bool IsWhite { get; protected set; }
    
    public GameObject boardPiece;
    public GameObject moveController;

    public List<GameObject> Pieces;
    public Player[] players;
    public GameObject victoryScreen;
    
    
    private bool isWhiteTurn = true;
    public bool FirstTurn { get; protected set; }
    [NonSerialized] public StoneType currentStone;

    private List<Stone>[,] activeStones;
    private List<Stone> selectedStones = new List<Stone>();
    private List<Stone> movingStones = new List<Stone>();

    private bool[,] allowedMoves;

    private BoardPosition moveStart;
    private BoardPosition currentPosition;
    private int pickedupStones = 0;

    private int selectionHeight = -1;

    private List<Char> moveString = new List<char>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {

        if (Client.Instance)
            IsWhite = Client.Instance.isWhite;
        else
            IsWhite = true;

        CurrentGameState = IsWhite ? GameState.TURN_START : GameState.OPPONENTS_TURN;
       
        allowedMoves = new bool[GameManager.Instance.BoardSize, GameManager.Instance.BoardSize];
        moveStart = MouseController.GetInvalidPosition();
        FirstTurn = true;

        currentStone = StoneType.Flat;
        SetupGameStates();
        PrepareActiveStones();
        CreateBoard();
    }

    void Update()
    {
        //DrawTakBoard();
        if (CurrentGameState != GameState.GAME_END && CurrentGameState != GameState.OPPONENTS_TURN)
            StateMachiene[CurrentGameState]();
    }

    public Stone GetTopStone(BoardPosition pos)
    {
        if (activeStones[pos.x, pos.y].Count == 0)
            return null;

        return activeStones[pos.x, pos.y][activeStones[pos.x, pos.y].Count - 1];
    }

    private void AddStateAction(GameState state, Action action)
    {
        if (StateMachiene.ContainsKey(state))
            StateMachiene[state] += action;
        else
            StateMachiene.Add(state, action);
    }

    private void SetupGameStates()
    { 
        StateMachiene = new Dictionary<GameState, Action>();

        AddStateAction(GameState.NULL_STATE, () => { Debug.LogError("Null state: This should never happen"); });
        AddStateAction(GameState.TURN_START, MouseController.UpdateMousePosition);
        AddStateAction(GameState.TURN_START, HighlightController.Instance.HiglightMousePosition);
        AddStateAction(GameState.TURN_START, UserSelection);
        AddStateAction(GameState.PICKUP, UpdateStoneSelection);
        AddStateAction(GameState.PICKUP, PickUpStones);
        AddStateAction(GameState.PLACE_STONE, MouseController.UpdateMousePosition);
        AddStateAction(GameState.PLACE_STONE, PlaceStone);
    }
    private void PrepareActiveStones()
    {
        activeStones = new List<Stone>[GameManager.Instance.BoardSize, GameManager.Instance.BoardSize];
        for (int i = 0; i < GameManager.Instance.BoardSize; i++)
        {
            for (int j = 0; j < GameManager.Instance.BoardSize; j++)
            {
                activeStones[i, j] = new List<Stone>();
            }
        }
    }
    private void CreateBoard()
    {
        GameObject board = Instantiate(new GameObject(), new Vector3(), Quaternion.identity, transform);

        for (int i = 0; i < GameManager.Instance.BoardSize; i++)
        {
            for (int j = 0; j < GameManager.Instance.BoardSize; j++)
            {
                Instantiate(boardPiece, new Vector3(i + 0.5f, 0, j + 0.5f), Quaternion.identity, board.transform);
            }
        }
    }
    private void UpdateStoneSelection()
    {
        if (!Camera.main)
            return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("TakStone")))
        {
            Stone go = hit.transform.gameObject.GetComponent<Stone>();
            if (go.Position.Equals(moveStart))
            {
                Stone topStone = GetTopStone(moveStart).GetComponent<Stone>();
                selectionHeight = topStone.Height - go.Height;
                if (selectionHeight >= GameManager.Instance.BoardSize)
                    selectionHeight = GameManager.Instance.BoardSize - 1;
            }
        }
        else
        {
            selectionHeight = -1;
        }
    }

    private void UserSelection()
    {

        if (MouseController.IsValidPosition() == false)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (GetTopStone(MouseController.CurrentPosition) == null)
            {
                if (FirstTurn)
                {
                    SpawnFirstStone();
                    FirstTurn = FirstTurn && isWhiteTurn;
                }
                else
                    SpawnStone();
                if(Client.Instance)
                    SendSpawnStone();
                NextTurn();
            }
            else
            {
                SetMovePosition();
            }
        }
    }

    private void SpawnStone()
    {
        int index = isWhiteTurn ? 0 : 1;
        int capstoneOffset = (currentStone == StoneType.Capstone ? 2 : 0);

        GameObject go = Instantiate(Pieces[index + capstoneOffset],
            transform.position,
            Quaternion.identity, transform) as GameObject;

        activeStones[MouseController.CurrentPosition.x, MouseController.CurrentPosition.y].Add(
            Stone.AddStone(go, MouseController.CurrentPosition, isWhiteTurn, currentStone));

        if (currentStone == StoneType.Capstone)
            players[index].TakeCapstone();
        else
            players[index].TakeStone();
    }
    private void SpawnFirstStone()
    {
        int index = isWhiteTurn ? 1 : 0;

        GameObject go = Instantiate(Pieces[index],
            transform.position,
            Quaternion.identity, transform) as GameObject;

        activeStones[MouseController.CurrentPosition.x, MouseController.CurrentPosition.y].Add(
            Stone.AddStone(go, MouseController.CurrentPosition, !isWhiteTurn));

        players[index].TakeStone();
    }
    
    private void SetMovePosition()
    {
        if (FirstTurn || GetTopStone(MouseController.CurrentPosition).isWhite != IsWhite)
            return;

        moveStart = MouseController.CurrentPosition;
        currentPosition = moveStart;
        moveController.SetActive(true);
        CurrentGameState = GameState.PICKUP;
    }
    private void ResetMovePosition()
    {
        pickedupStones = 0;
        selectedStones.Clear();
        movingStones.Clear();
        moveStart = MouseController.GetInvalidPosition();
        currentPosition = moveStart;
        moveController.SetActive(false);
    }
    private void PickUpStones()
    {
        if (activeStones[moveStart.x, moveStart.y].Count == 1)
        {
            // moving one stone
            selectedStones = new List<Stone>(activeStones[moveStart.x, moveStart.y]);
            activeStones[moveStart.x, moveStart.y].Clear();

            selectedStones[0].transform.localPosition += new Vector3(0, 0.4f, 0) * 4;
            pickedupStones = 1;

            movingStones = new List<Stone>(selectedStones);

            SetAllowedMoves();
            HighlightController.Instance.HighlightAllowedMoves(allowedMoves, GameManager.Instance.BoardSize);

            CurrentGameState = GameState.PLACE_STONE;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            int stackHeight = activeStones[moveStart.x, moveStart.y].Count;
            // moving multiple stones
            if (selectionHeight != -1)
            {
                selectedStones = activeStones[moveStart.x, moveStart.y].GetRange(stackHeight - selectionHeight - 1, selectionHeight + 1);
                movingStones = new List<Stone>(selectedStones);
                pickedupStones = 0;
                foreach (Stone selected in selectedStones)
                {
                    activeStones[moveStart.x, moveStart.y].Remove(selected);
                    selectedStones[pickedupStones].transform.localPosition += new Vector3(0, 0.4f, 0) * 4;
                    pickedupStones++;
                }
                SetAllowedMoves();
                HighlightController.Instance.HighlightAllowedMoves(allowedMoves, GameManager.Instance.BoardSize);

                moveString.Add((char)(selectionHeight + '1'));

                CurrentGameState = GameState.PLACE_STONE;
            }
        }

        if (Client.Instance && pickedupStones > 0)
        {
            movingStones = new List<Stone>(selectedStones);
            moveString.Add((char)(moveStart.x + 'a'));
            moveString.Add((char)(moveStart.y + '1'));
        }
    }
    public void CancelMove()
    {
        foreach (Stone selected in movingStones)
        {
            for (int i = 0; i < GameManager.Instance.BoardSize; i++)
            {
                for (int j = 0; j < GameManager.Instance.BoardSize; j++)
                {
                    activeStones[i, j].Remove(selected);
                }
            }
            selected.SetPosition(moveStart, activeStones[moveStart.x, moveStart.y].Count);
            activeStones[moveStart.x, moveStart.y].Add(selected);
        }
        ResetMovePosition();

        HighlightController.Instance.HideHighlights();

        moveStart = MouseController.GetInvalidPosition();
        currentPosition = moveStart;
        moveString = new List<char>();
        
        CurrentGameState = GameState.TURN_START;
    }
    private void SetAllowedMoves()
    {
        allowedMoves = new bool[GameManager.Instance.BoardSize, GameManager.Instance.BoardSize];
        if (currentPosition.Equals(moveStart) || !GameManager.Instance.OfficialRules)
        {
            if (currentPosition.x > 0)
                allowedMoves[currentPosition.x - 1, currentPosition.y] = CheckStoneTypes(currentPosition.x - 1, currentPosition.y);

            if (currentPosition.y > 0)
                allowedMoves[currentPosition.x, currentPosition.y - 1] = CheckStoneTypes(currentPosition.x, currentPosition.y - 1);

            if (currentPosition.x < GameManager.Instance.BoardSize - 1)
                allowedMoves[currentPosition.x + 1, currentPosition.y] = CheckStoneTypes(currentPosition.x + 1, currentPosition.y);

            if (currentPosition.y < GameManager.Instance.BoardSize - 1)
                allowedMoves[currentPosition.x, currentPosition.y + 1] = CheckStoneTypes(currentPosition.x, currentPosition.y + 1);

            if (!(selectedStones.Count == movingStones.Count))
                allowedMoves[currentPosition.x, currentPosition.y] = true;
        }
        else
        {
            allowedMoves[currentPosition.x, currentPosition.y] = true;

            if (moveStart.x > currentPosition.x && currentPosition.x > 0) // Left
            {
                allowedMoves[currentPosition.x - 1, currentPosition.y] = CheckStoneTypes(currentPosition.x - 1, currentPosition.y);
            }
            else if (moveStart.x < currentPosition.x && currentPosition.x < GameManager.Instance.BoardSize - 1) // Right
            {
                allowedMoves[currentPosition.x + 1, currentPosition.y] = CheckStoneTypes(currentPosition.x + 1, currentPosition.y);
            }
            else if (moveStart.y > currentPosition.y && currentPosition.y > 0) // Down
            {
                allowedMoves[currentPosition.x, currentPosition.y - 1] = CheckStoneTypes(currentPosition.x, currentPosition.y - 1);
            }
            else if (moveStart.y < currentPosition.y && currentPosition.y < GameManager.Instance.BoardSize - 1) // Up
            {
                allowedMoves[currentPosition.x, currentPosition.y + 1] = CheckStoneTypes(currentPosition.x, currentPosition.y + 1);
            }
        }
    }
    private bool CheckStoneTypes(int x, int y)
    {
        return (activeStones[x, y].Count == 0 ||
            activeStones[x, y][activeStones[x, y].Count - 1].StoneType == StoneType.Flat ||
            (activeStones[x, y][activeStones[x, y].Count - 1].StoneType == StoneType.Standing &&
             selectedStones[0].StoneType == StoneType.Capstone));
    }
    private void PlaceStone()
    {
        if (Input.GetMouseButtonDown(0) &&
            MouseController.IsValidPosition() &&
            allowedMoves[MouseController.CurrentPosition.x, MouseController.CurrentPosition.y])
        {
            BoardPosition pos = MouseController.CurrentPosition;

            if (Client.Instance != null)
            {
                if (movingStones.Count == pickedupStones)
                {
                    char next;
                    if (pos.x != moveStart.x)
                        next = pos.x > moveStart.x ? '>' : '<';
                    else
                        next = pos.y > moveStart.y ? '+' : '-';

                    moveString.Add(next);
                }

                if (MouseController.CurrentPosition.Equals(currentPosition))
                    moveString[moveString.Count - 1] = (char)(moveString[moveString.Count - 1] + 1);
                else if (movingStones.Count != 1)
                    moveString.Add('1');
            }

            currentPosition = pos;

            selectedStones[0].SetPosition(pos, activeStones[pos.x, pos.y].Count);
            Flatten(pos);

            activeStones[pos.x, pos.y].Add(selectedStones[0]);
            selectedStones.RemoveAt(0);
            pickedupStones--;


            if (pickedupStones > 0)
            {
                for(int i = 0; i < selectedStones.Count; i++)
                    selectedStones[i].SetPositionFloating(pos, activeStones[pos.x, pos.y].Count, i);

                SetAllowedMoves();
                HighlightController.Instance.HighlightAllowedMoves(allowedMoves, GameManager.Instance.BoardSize);
            }
            else
            {
                if(Client.Instance)
                    SendMove();

                NextTurn();
            }
        }
    }
    private void Flatten(BoardPosition pos)
    {
        Stone Standing = GetTopStone(pos);
        if (Standing != null && selectedStones.Count == 1)
        {
            if (selectedStones[0].StoneType == StoneType.Capstone &&
                Standing.StoneType == StoneType.Standing)
            {
                Standing.Flatten();
            }
        }
    }


    private void NextTurn()
    {
        ResetMovePosition();
        CheckWin();
        if (CurrentGameState == GameState.GAME_END)
            return;

        isWhiteTurn = !isWhiteTurn;
        currentStone = StoneType.Flat;
        HighlightController.Instance.HideHighlights();

        foreach (Player p in players)
            p.SwapColor();

        if (Client.Instance)
        {
            if (isWhiteTurn == IsWhite)
                CurrentGameState = GameState.TURN_START;
            else
                CurrentGameState = GameState.OPPONENTS_TURN;
        }
        else
        {
            IsWhite = !IsWhite;
            CurrentGameState = GameState.TURN_START;
        }
    }
    
    private void CheckWin()
    {
        bool whiteWin = false;
        bool blackWin = false;
        
        // Calls search for roads with all tiles along two adjacent edges. If a road is found,
        // a flag is set for the corresponding player.
        for (int i = 0; i < GameManager.Instance.BoardSize; i++)
        {
            bool[,] explored = new bool[GameManager.Instance.BoardSize, GameManager.Instance.BoardSize];
            
            if (activeStones[i, 0].Count > 0 && SearchForRoads(i, 0, explored,
                activeStones[i, 0][activeStones[i, 0].Count - 1].isWhite, true))
            {
                if (activeStones[i, 0][activeStones[i, 0].Count - 1].isWhite)
                    whiteWin = true;
                else
                    blackWin = true;
            }

            explored = new bool[GameManager.Instance.BoardSize, GameManager.Instance.BoardSize];
            
            if (activeStones[0, i].Count > 0 && SearchForRoads(0, i, explored,
                activeStones[0, i][activeStones[0, i].Count - 1].isWhite, false))
            {
                if (activeStones[0, i][activeStones[0, i].Count - 1].isWhite)
                    whiteWin = true;
                else
                    blackWin = true;
            }
        }

        // If a road is made for the current player, they win,
        // if a road is made for the other player, they win,
        // if someone is out of stones, FlatWin is called.
        if ((IsWhite && whiteWin) || (!IsWhite && blackWin))
        {
            ShowVictoryScreen(IsWhite);
        }
        else if (blackWin || whiteWin)
        {
            ShowVictoryScreen(!IsWhite);
        }
        else if (players[0].NoStonesLeft() || players[1].NoStonesLeft())
            FlatWin();
    }
    private void FlatWin()
    {
        int[] stoneCount = new int[2];
        for (int i = 0; i < GameManager.Instance.BoardSize; i++)
        {
            for (int j = 0; j < GameManager.Instance.BoardSize; j++)
            {
                if (activeStones[i, j].Count > 0)
                    stoneCount[activeStones[i, j][activeStones[i, j].Count - 1].isWhite ? 0 : 1]++;
            }
        }

        if (stoneCount[0] == stoneCount[1])
            ShowVictoryScreen(true, true);
        else
            ShowVictoryScreen(stoneCount[0] > stoneCount[1]);
    }
    private bool SearchForRoads(int x, int y, bool[,] explored, bool isWhiteRoad, bool verticalSearch)
    {
        if (activeStones[x, y].Count == 0 || explored[x, y])
            return false;


        Stone currentStone = activeStones[x, y][activeStones[x, y].Count - 1];
        if (currentStone.isWhite != isWhiteRoad || currentStone.StoneType == StoneType.Standing)
            return false;


        if ((verticalSearch && y == (GameManager.Instance.BoardSize - 1)) || (!verticalSearch && x == (GameManager.Instance.BoardSize - 1)))
            return true;


        explored[x, y] = true;

        return (x < GameManager.Instance.BoardSize - 1 && SearchForRoads(x + 1, y, explored, isWhiteRoad, verticalSearch))
            || (y < GameManager.Instance.BoardSize - 1 && SearchForRoads(x, y + 1, explored, isWhiteRoad, verticalSearch))
            || (x > 1 && SearchForRoads(x - 1, y, explored, isWhiteRoad, verticalSearch))
            || (y > 1 && SearchForRoads(x, y - 1, explored, isWhiteRoad, verticalSearch));
    }

    private void ShowVictoryScreen(bool whiteWinner = true, bool tie = false)
    {
        victoryScreen.SetActive(true);
        CurrentGameState = GameState.GAME_END;

        if (tie)
            victoryScreen.GetComponentInChildren<Text>().text = "It's a Tie!";
        else
        {
            string winner;
            if (Client.Instance)
                winner = Client.Instance.players[whiteWinner ? 0 : 1].name;
            else
                winner = whiteWinner ? "White" : "Black";
            victoryScreen.GetComponentInChildren<Text>().text = winner + " has won the game!";
        }
    }

    private void SendSpawnStone()
    {
        if (currentStone != StoneType.Flat && !FirstTurn)
            moveString.Add(currentStone == StoneType.Capstone ? 'C' : 'S');

        moveString.Add((char)(MouseController.CurrentPosition.x + 'a'));
        moveString.Add((char)(MouseController.CurrentPosition.y + '1'));
        SendMove();
    }
    private void SendMove()
    {
        if (Client.Instance == false)
        {
            Debug.LogError("Sending a move when there is no client. Probably in a local game.");
            return;
        }
        Client.Instance.Send("CMOV|" + new string(moveString.ToArray()));
        Client.Instance.sentMove = true;
        moveString = new List<char>();
    }
    public void TryMove(string fMoveString)
    {
        int index = 0;
        int[] movingDir = new int[2];

        if (fMoveString[index] == 'C' || fMoveString[index] == 'S')
        {
            currentStone = (fMoveString[index] == 'C') ? StoneType.Capstone : StoneType.Standing;
            index++;
        }
        else if (fMoveString[index] >= '0' && fMoveString[index] <= '9')
            pickedupStones = fMoveString[index++] - '0';
        else
            currentStone = StoneType.Flat;

        moveStart.x = fMoveString[index++] - 'a';
        moveStart.y = fMoveString[index++] - '1';

        if (fMoveString.Length == index)
        {
            MouseController.CurrentPosition = moveStart;
            

            if (FirstTurn)
            {
                SpawnFirstStone();
                FirstTurn = FirstTurn && isWhiteTurn;
            }
            else
                SpawnStone();
            NextTurn();
            return;
        }

        if (pickedupStones == 0)
            pickedupStones = 1;

        char dir = fMoveString[index++];

        movingDir[0] = (dir == '>') ? 1 : (dir == '<') ? -1 : 0;
        movingDir[1] = (dir == '+') ? 1 : (dir == '-') ? -1 : 0;

        selectedStones = activeStones[moveStart.x, moveStart.y].GetRange(
            activeStones[moveStart.x, moveStart.y].Count - pickedupStones, 
            pickedupStones);
        
        activeStones[moveStart.x, moveStart.y].RemoveRange(
            activeStones[moveStart.x, moveStart.y].Count - pickedupStones,
            pickedupStones);

        if (index == fMoveString.Length)
        {
            fMoveString += "1";
        }


        for (int i = index; i < fMoveString.Length; i++)
        {
            moveStart.x += movingDir[0];
            moveStart.y += movingDir[1];
            for (int j = fMoveString[index] - '0'; j > 0; j--)
            {
                activeStones[moveStart.x, moveStart.y].Add(selectedStones[0]);

                selectedStones[0].SetPosition(moveStart, activeStones[moveStart.x, moveStart.y].Count -1);

                Flatten(moveStart);

                selectedStones.RemoveAt(0);
            }
        }
        
        NextTurn();
    }
}