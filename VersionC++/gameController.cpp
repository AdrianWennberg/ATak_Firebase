#include <iostream>
#include <string>
#include <cstdlib>

#include "GameController.h"
#include "board.h"
#include "player.h"
using namespace std;



GameController::GameController(): gameBoard(getBoardSize()), gamePlayers(2, Player(gameBoard.getBoardSize())), winner(none), round(0), gameOver(false)
{
    setPlayerNames();
    
    bool moveWorked;
    string myMoveString;
    CurrentPlayer turnPlayer;
    
    CLEAR_SCREEN;
    gameBoard.printBoard();
    cout << "\n    Round 1\n    " << gamePlayers[0].getName() << " stones: " << gamePlayers[0].getStonesLeft() << " capstones: " << gamePlayers[0].getCapstonesLeft();
    moveWorked = false;
    
    while(!moveWorked)
    {
        cout << "\n    : ";
        getline(cin, myMoveString);
        Move aMove(myMoveString);
        
        moveWorked = aMove.isCorrectMove() && aMove.getPlacementStoneType() == flat && gameBoard.tryMove(aMove, second);
        if(moveWorked){ gamePlayers[1].decrementStones(); }
    }
    
    CLEAR_SCREEN;
    gameBoard.printBoard();
    cout << "\n    Round 1\n    " << gamePlayers[1].getName() << " stones: " << gamePlayers[1].getStonesLeft() << " capstones: " << gamePlayers[1].getCapstonesLeft();
    moveWorked = false;
    
    while(!moveWorked)
    {
        cout << "\n    : ";
        getline(cin, myMoveString);
        Move aMove(myMoveString);
        
        moveWorked = aMove.isCorrectMove() && aMove.getPlacementStoneType() == flat && gameBoard.tryMove(aMove, first);
        if(moveWorked){ gamePlayers[0].decrementStones(); } 
    }
    
    while(!gameOver)
    {
        CLEAR_SCREEN;
        gameBoard.printBoard();
        cout << "\n    Round " << 1 + round / 2 << "\n    " << gamePlayers[round % 2].getName()\
        << " stones: " << gamePlayers[round % 2].getStonesLeft() << " capstones: " << gamePlayers[round % 2].getCapstonesLeft();
        turnPlayer = ((1 + round) % 2?first:second);
        moveWorked = false;
        
        while(!moveWorked)
        {
            cout << "\n    : ";
            getline(cin, myMoveString);
            Move aMove(myMoveString);
            if(aMove.isPlacement() && aMove.getPlacementStoneType() == capstone && !(gamePlayers[turnPlayer - 1].getCapstonesLeft()))
            {
                moveWorked = false;
            }
            else
            {
                moveWorked = aMove.isCorrectMove() && gameBoard.tryMove(aMove, turnPlayer);
            }
            if(moveWorked && aMove.isPlacement())
            { 
                if(aMove.getPlacementStoneType() == capstone){ gamePlayers[turnPlayer - 1].takeCapstone(); }
                else { gamePlayers[turnPlayer - 1].decrementStones(); }
            }
        }
        if(gamePlayers[turnPlayer - 1].getStonesLeft() == 0)
        {
            winner = flatWin();
            gameOver = true;
        }
        else if((winner = roadWin(turnPlayer)) != none)
        {
            gameOver = true;
        }
        else{ round++; }
    }
    CLEAR_SCREEN;
    gameBoard.printBoard();
    if(winner)
    {
        cout << "\n    " << gamePlayers[winner - 1].getName() << " has won the game!";
    }
    else{
        cout << "\n    It's a draw!";
    }
    
}


int GameController::getBoardSize()
{
    string tempBoardSize;
    long int boardSize;
    const char *cInputString;
    char *endPtr;
    bool correctInput = false;
    int possibleSizes[] = {3, 4, 5, 6, 8};
    
    
    do{
        cout << "\n    Input desired board size: ";
        getline(cin, tempBoardSize);
        if((boardSize = strtol(tempBoardSize.c_str(), &endPtr, 10)) && *endPtr == '\0')
        {
            int i;
            for(i = 0; i < 5 && boardSize != possibleSizes[i] ; i++);
            if(i < 5){ correctInput = true; }
        }
        
    }while(!correctInput);
    
    return boardSize;
}

void GameController::setPlayerNames()
{
    string playerOneName;
    string playerTwoName;
    
    
    cout << "\n    Input player 1 name: ";
    getline(cin, playerOneName);
    
    cout << "\n    Input player 2 name: ";
    getline(cin, playerTwoName);
    
    gamePlayers[0].setName(playerOneName);
    gamePlayers[1].setName(playerTwoName);
}

CurrentPlayer GameController::flatWin()
{
    int stonesPerPlayer[3] = {0, 0, 0};
    int i, boardSize = gameBoard.getBoardSize();
    
    
    for(i = 0; i < boardSize * boardSize; i++)
    {
        stonesPerPlayer[gameBoard.getPositionPointer(i / 5, i % 5)->getPlayer()]++;
    }
    
    if(stonesPerPlayer[1] != stonesPerPlayer[2])
    {
        return (stonesPerPlayer[1] > stonesPerPlayer[2]? first: second);
    }
    else
    {
        return none;
    }
}

CurrentPlayer GameController::roadWin(CurrentPlayer turnPlayer)
{
    int boardSize = gameBoard.getBoardSize();
    bool hasWon[3] = {false};
    
    
    vector<bool> explored(boardSize * boardSize, false);
    CurrentPlayer roadOwner;
    
    for(int row = 0; row < gameBoard.getBoardSize(); row++)
    {
        roadOwner = gameBoard.getPositionPointer(row, 0)->getPlayer();
        if(roadOwner != none && searchForRoads(row, 0, &explored, roadOwner, rightDir, boardSize))
        {
            hasWon[roadOwner] = true;
        }
    }
    explored = vector<bool>(boardSize * boardSize, false);
    for(int column = 0; column < gameBoard.getBoardSize(); column++)
    {
        roadOwner = gameBoard.getPositionPointer(0, column)->getPlayer();
        if(roadOwner != none && searchForRoads(0, column, &explored, roadOwner, downDir, boardSize))
        {
            hasWon[roadOwner] = true;
        }
    }
    
    if(hasWon[turnPlayer]) { return turnPlayer; }
    else if(hasWon[3 - turnPlayer]) { return static_cast<CurrentPlayer>(3-turnPlayer);}
    else{ return none; }
}


bool GameController::searchForRoads(int row, int column, vector<bool> *explored, CurrentPlayer roadOwner, Direction searchDirection, int boardSize)
{
    if(gameBoard.getPositionPointer(row, column)->getPlayer() == roadOwner)
    {
        if((searchDirection == rightDir && column == boardSize - 1) || (searchDirection == downDir && row == boardSize - 1))
        {
            return true;
        }
        else if(!(*explored)[row * boardSize + column])
        {
            (*explored)[row * boardSize + column] = true;
           
            return (gameBoard.isOnBoard(row + 1, column) && searchForRoads(row + 1, column, explored, roadOwner, searchDirection, boardSize))\
            || (gameBoard.isOnBoard(row, column + 1) && searchForRoads(row, column + 1, explored, roadOwner, searchDirection, boardSize))\
            || (gameBoard.isOnBoard(row - 1, column) && searchForRoads(row - 1, column, explored, roadOwner, searchDirection, boardSize))\
            || (gameBoard.isOnBoard(row, column - 1) && searchForRoads(row, column - 1, explored, roadOwner, searchDirection, boardSize));
        }
    }
    else
    {
        return false;
    }
}

