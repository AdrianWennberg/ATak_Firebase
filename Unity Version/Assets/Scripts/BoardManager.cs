using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public static BoardHighlights Instance { get; set; }

    private bool gameOver;
    
    public enum StoneType { Flat, Standing, Capstone };

    public GameObject takPlane;
    public GameObject highlightPrefab;
    public GameObject moveController;
    public GameObject toggleStoneType;


    public List<GameObject> Pieces;
    private Player[] players;
    public GameObject[] playerDisplay;
    public GameObject victoryScreen;

    private GameObject highlightObject;


    private int selectionX = -1;
    private int selectionY = -1;
    private bool isWhiteTurn = true;
    private bool firstTurn = true;
    private StoneType currentStone;

    private List<Stone>[,] activeStones;
    private List<Stone> selectedStones = new List<Stone>();
    private List<Stone> movingStones;

    private bool[,] allowedMoves;

    private int[] moveStart;
    private int[] currentPosition;
    private int pickedupStones = 0;

    private int selectionHeight = -1;

    void Start()
    {
        PreparePlayers();
        allowedMoves = new bool[GameManager.Instance.boardSize, GameManager.Instance.boardSize];
        moveStart = new int[2];
        ResetCurrentStone();
        PrepareActiveStones();
        PlaceTakPlane();
        CreateHighlight();
    }



    // Update is called once per frame
    void Update()
    {
        DrawTakBoard();
        if (!gameOver)
        {
            if (!moveController.activeSelf)
            {
                UpdateBoardSelection();
                HighlightSlot();
                UserSelection();
            }
            else if (pickedupStones == 0)
            {
                UpdateStoneSelection();
                PickUpStones();
            }
            else
            {
                UpdateBoardSelection();
                PlaceStone();
            }
        }
    }

    private void PreparePlayers()
    {
        players = new Player[2];
        players[0] = new Player("Player 1", GameManager.Instance.boardSize);
        players[1] = new Player("Player 2", GameManager.Instance.boardSize);
        for (int i = 0; i < 2; i++)
        {
            playerDisplay[i].GetComponent<Text>().text = players[i].GetName();
            playerDisplay[i].transform.GetChild(0).GetComponent<Text>().text = "Stones : " + players[i].GetStonesLeft().ToString();
            playerDisplay[i].transform.GetChild(1).GetComponent<Text>().text = "Captones : " + players[i].GetCapstonesLeft().ToString();
        }
    }

    //
    private void PrepareActiveStones()
    {
        activeStones = new List<Stone>[GameManager.Instance.boardSize, GameManager.Instance.boardSize];
        for (int i = 0; i < GameManager.Instance.boardSize; i++)
        {
            for (int j = 0; j < GameManager.Instance.boardSize; j++)
            {
                activeStones[i, j] = new List<Stone>();
            }
        }
    }
    //
    private void PlaceTakPlane()
    {
        GameObject go = Instantiate(takPlane,
            transform.position + Vector3.forward * (GameManager.Instance.boardSize * GameManager.Instance.slotSize / 2.0f) + Vector3.right * (GameManager.Instance.boardSize * GameManager.Instance.slotSize / 2.0f),
            Quaternion.identity, transform);
        go.transform.localScale *= GameManager.Instance.slotSize;
    }
    //
    private void CreateHighlight()
    {
        highlightObject = Instantiate(highlightPrefab, transform.position, Quaternion.identity, transform) as GameObject;
        highlightObject.transform.localScale = highlightPrefab.transform.localScale * GameManager.Instance.slotSize;
        highlightObject.SetActive(false);
    }

    //
    private void UpdateBoardSelection()
    {
        if (!Camera.main)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("TakBoard")))
        {
            selectionX = (int)(hit.point.x / GameManager.Instance.slotSize);
            selectionY = (int)(hit.point.z / GameManager.Instance.slotSize);
        }
        else
        {
            selectionY = -1;
            selectionX = -1;
        }
    }
    //
    private void UpdateStoneSelection()
    {
        if (!Camera.main)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("TakStone")))
        {
            Stone go = hit.transform.gameObject.GetComponent<Stone>();
            if(go.CurrentX == moveStart[0] && go.CurrentY == moveStart[1])
            {
                Stone topStone = activeStones[moveStart[0], moveStart[1]][activeStones[moveStart[0], moveStart[1]].Count - 1].GetComponent<Stone>();
                selectionHeight = topStone.Height - go.Height;
                if (selectionHeight >= GameManager.Instance.boardSize)
                    selectionHeight = GameManager.Instance.boardSize - 1;
            }
        }
        else
        {
            selectionHeight = -1;
        }
    }
    //
    private void DrawTakBoard()
    {

        Vector3 widthLine = Vector3.right * GameManager.Instance.boardSize * GameManager.Instance.slotSize;
        Vector3 heightLine = Vector3.forward * GameManager.Instance.boardSize * GameManager.Instance.slotSize;

        for (int i = 0; i <= GameManager.Instance.boardSize; i++)
        {
            Vector3 start = Vector3.forward * i * GameManager.Instance.slotSize;
            Debug.DrawLine(start, start + widthLine);

            start = Vector3.right * i * GameManager.Instance.slotSize;
            Debug.DrawLine(start, start + heightLine);
        }
    }
    //
    private void HighlightSlot()
    {
        
        if (selectionX >= 0 && selectionY >= 0)
        {
            List<Stone> positionStones = new List<Stone>(activeStones[selectionX, selectionY]);
            if (positionStones.Count == 0 || 
                positionStones[positionStones.Count - 1].isWhite == isWhiteTurn)
            {
                highlightObject.transform.position = new Vector3(
                    selectionX * GameManager.Instance.slotSize + (GameManager.Instance.slotSize / 2.0f), 0,
                    selectionY * GameManager.Instance.slotSize + (GameManager.Instance.slotSize / 2.0f));

                highlightObject.SetActive(true);
            }
        }
        else
        {
            highlightObject.SetActive(false);
        }
    }
    

    //
    private void UserSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectionX >= 0 && selectionY >= 0)
            {
                if (activeStones[selectionX, selectionY].Count == 0)
                {
                    if (!(currentStone == StoneType.Capstone) || players[isWhiteTurn ? 0 : 1].HasCapstone())
                    {
                        if (firstTurn)
                            SpawnFirstStone();
                        else
                            SpawnStone();
                        NextTurn();
                    }
                }
                else if(!firstTurn)
                {
                    SelectStones();
                }
            }
        }
    }
    //
    private void SelectStones()
    {
        if (activeStones[selectionX, selectionY][activeStones[selectionX, selectionY].Count - 1].isWhite != isWhiteTurn)
        {
            selectedStones.Clear();
            return;
        }

        SetMovePosition(selectionX, selectionY);

    }

    //
    public void SetMovePosition(int x, int y)
    {
        moveStart[0] = x;
        moveStart[1] = y;
        currentPosition = new int[] { moveStart[0], moveStart[1] };
        moveController.SetActive(true);
    }

    //
    public void ResetMovePosition()
    {
        moveStart[0] = -1;
        moveStart[1] = -1;
        currentPosition = new int[] { moveStart[0], moveStart[1] };
        moveController.SetActive(false);
    }

    //
    private void PickUpStones()
    {
        if (activeStones[moveStart[0], moveStart[1]].Count == 1)
        {
            // moving one stone
            selectedStones = new List<Stone>(activeStones[moveStart[0], moveStart[1]]);
            activeStones[moveStart[0], moveStart[1]].Clear();

            selectedStones[0].transform.localPosition += new Vector3(0, 0.4f, 0) * 4;
            pickedupStones = 1;
            
            SetAllowedMoves();
            BoardHighlights.Instance.HighlightAllowedMoves(allowedMoves, GameManager.Instance.boardSize, GameManager.Instance.slotSize);

            highlightObject.SetActive(false);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            int stackHeight = activeStones[moveStart[0], moveStart[1]].Count;
            //moving multiple stones
            if (selectionHeight != -1)
            {
                selectedStones = activeStones[moveStart[0], moveStart[1]].GetRange(stackHeight - selectionHeight - 1, selectionHeight + 1);

                pickedupStones = 0;
                foreach (Stone selected in selectedStones)
                {
                    activeStones[moveStart[0], moveStart[1]].Remove(selected);
                    selectedStones[pickedupStones].transform.localPosition += new Vector3(0, 0.4f, 0) *  4;
                    pickedupStones++;
                }
                SetAllowedMoves();
                BoardHighlights.Instance.HighlightAllowedMoves(allowedMoves, GameManager.Instance.boardSize, GameManager.Instance.slotSize);
                highlightObject.SetActive(false);
            }
        }

        if(pickedupStones > 0)
        {
            movingStones = new List<Stone>(selectedStones);
        }
    }
    //
    public void CancelMove()
    {
        foreach (Stone selected in movingStones)
        {
            for (int i = 0; i < GameManager.Instance.boardSize; i++)
            {
                for (int j = 0; j < GameManager.Instance.boardSize; j++)
                {
                    activeStones[i, j].Remove(selected);
                }
            }
            selected.transform.position = GetSlotCenter(moveStart[0], moveStart[1]) +
                    new Vector3(0, 0.2f, 0) * (2 * activeStones[moveStart[0], moveStart[1]].Count);
            OffsetStonePosition(selected.gameObject);
            activeStones[moveStart[0], moveStart[1]].Add(selected);

        }
        pickedupStones = 0;
        movingStones.Clear();
        selectedStones.Clear();
        ResetMovePosition();

        BoardHighlights.Instance.HideHighlights();

        moveStart[0] = -1;
        moveStart[1] = -1;
        currentPosition = new int[] { moveStart[0], moveStart[1] };
    }
    //
    private void SetAllowedMoves()
    {
        allowedMoves = new bool[GameManager.Instance.boardSize, GameManager.Instance.boardSize];
        if(currentPosition[0] == moveStart[0] && currentPosition[1] == moveStart[1])
        {
            if(moveStart[0] - 1 >= 0)
                allowedMoves[moveStart[0] - 1, moveStart[1]] = CheckStoneTypes(currentPosition[0] - 1, currentPosition[1]);

            if (moveStart[1] - 1 >= 0)
                allowedMoves[moveStart[0], moveStart[1] - 1] = CheckStoneTypes(currentPosition[0], currentPosition[1] - 1);

            if (moveStart[0] + 1 < GameManager.Instance.boardSize)
                allowedMoves[moveStart[0] + 1, moveStart[1]] = CheckStoneTypes(currentPosition[0] + 1, currentPosition[1]);

            if (moveStart[1] + 1 < GameManager.Instance.boardSize)
                allowedMoves[moveStart[0], moveStart[1] + 1] = CheckStoneTypes(currentPosition[0], currentPosition[1] + 1);
        }
        else
        {
            allowedMoves[currentPosition[0], currentPosition[1]] = true;

            if (moveStart[0] > currentPosition[0] && currentPosition[0] > 0) // Left
            {
                allowedMoves[currentPosition[0] - 1, currentPosition[1]] = CheckStoneTypes(currentPosition[0] - 1, currentPosition[1]);
            }
            else if (moveStart[0] < currentPosition[0] && currentPosition[0] < GameManager.Instance.boardSize - 1) // Right
            {
                allowedMoves[currentPosition[0] + 1, currentPosition[1]] = CheckStoneTypes(currentPosition[0] + 1, currentPosition[1]);
            }
            else if (moveStart[1] > currentPosition[1] && currentPosition[1] > 0) // Down
            {
                allowedMoves[currentPosition[0], currentPosition[1] - 1] = CheckStoneTypes(currentPosition[0], currentPosition[1] - 1);
            }
            else if (moveStart[1] < currentPosition[1] && currentPosition[1] < GameManager.Instance.boardSize - 1) // Up
            {
                allowedMoves[currentPosition[0], currentPosition[1] + 1] = CheckStoneTypes(currentPosition[0], currentPosition[1] + 1);
            }
        }
    }

    private bool CheckStoneTypes(int x, int y)
    {
        return (activeStones[x, y].Count == 0 ||
            activeStones[x, y][activeStones[x, y].Count - 1].stoneType == StoneType.Flat ||
            (activeStones[x, y][activeStones[x, y].Count - 1].stoneType == StoneType.Standing &&
             selectedStones[0].stoneType == StoneType.Capstone));
    }

    //
    private void PlaceStone()
    {
        if (Input.GetMouseButtonDown(0) && 
            selectionX >= 0 && selectionY >= 0 && 
            allowedMoves[selectionX, selectionY])
        {
            currentPosition[0] = selectionX;
            currentPosition[1] = selectionY;

            selectedStones[0].transform.position = GetSlotCenter(selectionX, selectionY) +
                new Vector3(0, 0.4f, 0) * activeStones[selectionX, selectionY].Count;
            OffsetStonePosition(selectedStones[0].gameObject);

            Flatten();

            selectedStones[0].SetPosition(selectionX, selectionY);
            selectedStones[0].Height = activeStones[selectionX, selectionY].Count;

            activeStones[selectionX, selectionY].Add(selectedStones[0]);
            selectedStones.RemoveAt(0);
            pickedupStones--;

            BoardHighlights.Instance.HideHighlights();


            if (pickedupStones > 0)
            {
                int i = 0;
                foreach (Stone selected in selectedStones)
                {
                    selected.transform.position = GetSlotCenter(selectionX, selectionY) +
                    new Vector3(0, 0.4f, 0) * (i++ + activeStones[selectionX, selectionY].Count + 4);
                    OffsetStonePosition(selected.gameObject);
                }

                SetAllowedMoves();
                BoardHighlights.Instance.HighlightAllowedMoves(allowedMoves, GameManager.Instance.boardSize, GameManager.Instance.slotSize);
            }
            else
            {
                CheckRoadWin();
                NextTurn();
                ResetMovePosition();
            }
        }
    }

    private void Flatten()
    {
        if (activeStones[selectionX, selectionY].Count > 0)
        {
            GameObject Capstone = selectedStones[0].gameObject;
            GameObject Standing = activeStones[selectionX, selectionY][activeStones[selectionX, selectionY].Count - 1].gameObject;
            if (Capstone.GetComponent<Stone>().stoneType == StoneType.Capstone &&
                Standing.GetComponent<Stone>().stoneType == StoneType.Standing)
            {
                Standing.transform.position -= new Vector3(0, Standing.GetComponent<Renderer>().bounds.size.y / 2, 0);
                Standing.transform.rotation = new Quaternion();
                OffsetStonePosition(Standing);
                Standing.GetComponent<Stone>().stoneType = StoneType.Flat;
            }
        }
    }

    //
    private void SpawnStone()
    {
        int index = isWhiteTurn ? 0 : 1;
        GameObject go;
        int capstoneOffset = (currentStone == StoneType.Capstone ? 2 : 0);
        
        go = Instantiate(Pieces[index + capstoneOffset],
            transform.position,
            Quaternion.identity, transform) as GameObject;
        
        go.transform.position = GetSlotCenter(selectionX, selectionY);

        if(currentStone == StoneType.Standing)
            go.transform.Rotate(new Vector3(90, 45, 0));

        activeStones[selectionX, selectionY].Add(go.GetComponent<Stone>());

        activeStones[selectionX, selectionY][0].stoneType = (StoneType)currentStone;
        activeStones[selectionX, selectionY][0].SetPosition(selectionX, selectionY);
        activeStones[selectionX, selectionY][0].Height = 0;

        OffsetStonePosition(go);
        highlightObject.SetActive(false);

        if (currentStone == StoneType.Capstone)
        {
            players[index].TakeCapstone();
            playerDisplay[index].transform.GetChild(1).GetComponent<Text>().text = "Captones : " + players[index].GetCapstonesLeft().ToString();

        }
        else
        {
            if (!players[index].TakeStone())
            {
                FlatWin();
            }
            playerDisplay[index].transform.GetChild(0).GetComponent<Text>().text = "Stones : " + players[index].GetStonesLeft().ToString();
        }
        CheckRoadWin();
    }   

    private void SpawnFirstStone()
    {
        GameObject go;
        int index = isWhiteTurn ? 1 : 0;

        go = Instantiate(Pieces[index],
            transform.position,
            Quaternion.identity, transform) as GameObject;

        go.transform.position = GetSlotCenter(selectionX, selectionY);
        
        activeStones[selectionX, selectionY].Add(go.GetComponent<Stone>());

        activeStones[selectionX, selectionY][0].stoneType = StoneType.Flat;
        activeStones[selectionX, selectionY][0].SetPosition(selectionX, selectionY);
        activeStones[selectionX, selectionY][0].Height = 0;

        OffsetStonePosition(go);
        highlightObject.SetActive(false);

        players[index].TakeStone();
        playerDisplay[index].transform.GetChild(0).GetComponent<Text>().text = "Stones : " + players[index].GetStonesLeft().ToString();

        if (!isWhiteTurn)
            firstTurn = false;
    }

    private void OffsetStonePosition(GameObject go)
    {
        go.transform.position += new Vector3(0, go.GetComponent<Renderer>().bounds.size.y / 2, 0);
    }

    //
    private Vector3 GetSlotCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += x * GameManager.Instance.slotSize + (GameManager.Instance.slotSize / 2.0f);
        origin.z += y * GameManager.Instance.slotSize + (GameManager.Instance.slotSize / 2.0f);
        return origin;
    }

    //
    private void NextTurn()
    {
        isWhiteTurn = !isWhiteTurn;
        ResetCurrentStone();
    }

    //
    public void ToggleCurrentStone()
    {
        if(!firstTurn)
            currentStone = (StoneType)((int)currentStone + 1);

        if((int)currentStone == 3)
        {
            currentStone = StoneType.Flat;
        }

        toggleStoneType.GetComponentInChildren<Text>().text = currentStone.ToString();  
    }

    private void ResetCurrentStone()
    {
        currentStone = StoneType.Flat;
        toggleStoneType.GetComponentInChildren<Text>().text = currentStone.ToString();
    }


    private void FlatWin()
    {
        int[] stoneCount = new int[2];
        for(int i = 0; i < GameManager.Instance.boardSize; i++)
        {
            for(int j = 0; j < GameManager.Instance.boardSize; j++)
            {
                if(activeStones[i,j].Count > 0)
                {
                    stoneCount[activeStones[i, j][activeStones[i, j].Count - 1].isWhite ? 0 : 1]++;
                }
            }
        }
        gameOver = true;
        if(stoneCount[0] == stoneCount[1])
        {
            victoryScreen.GetComponent<Text>().text = "It's a Tie!";
        }
        else
        {
            victoryScreen.GetComponent<Text>().text = players[stoneCount[0] > stoneCount[1] ? 0:1].GetName() + " has won the game!";
        }
    }

    private void CheckRoadWin()
    {
        for(int i = 0; i < GameManager.Instance.boardSize; i++)
        {
            if((activeStones[i, 0].Count > 0 &&
                SearchForRoads(i, 0, new bool[GameManager.Instance.boardSize, GameManager.Instance.boardSize], activeStones[i, 0][activeStones[i, 0].Count - 1].isWhite, true)))
            {
                gameOver = true;
                victoryScreen.GetComponent<Text>().text = players[activeStones[i, 0][activeStones[i, 0].Count - 1].isWhite ? 0 : 1].GetName() + " has won the game!";
            }
            else if((activeStones[0, i].Count > 0 &&
                SearchForRoads(0, i, new bool[GameManager.Instance.boardSize, GameManager.Instance.boardSize], activeStones[0, i][activeStones[0, i].Count - 1].isWhite, false)))
            {
                gameOver = true;
                victoryScreen.GetComponent<Text>().text = players[activeStones[i, 0][activeStones[0, i].Count - 1].isWhite ? 0 : 1].GetName() + " has won the game!";
            }
        }
    }


    private bool SearchForRoads(int x, int y, bool[,] explored, bool isWhiteRoad, bool verticalSearch)
    {
        if (activeStones[x, y].Count > 0)
        {
            Stone currentStone = activeStones[x, y][activeStones[x, y].Count - 1];
            if (currentStone.isWhite == isWhiteRoad && currentStone.stoneType != StoneType.Standing)
            {
                if ((verticalSearch && y == 4) || (!verticalSearch && x == 4))
                {
                    return true;
                }
                else if (!explored[x, y])
                {
                    explored[x, y] = true;

                    return (x < GameManager.Instance.boardSize - 1 && SearchForRoads(x + 1, y, explored, isWhiteRoad, verticalSearch))
                        || (y < GameManager.Instance.boardSize - 1 && SearchForRoads(x, y + 1, explored, isWhiteRoad, verticalSearch))
                        || (x > 1 && SearchForRoads(x - 1, y, explored, isWhiteRoad, verticalSearch))
                        || (y > 1 && SearchForRoads(x, y - 1, explored, isWhiteRoad, verticalSearch));
                }
            }
        }
        return false;
    }
}
