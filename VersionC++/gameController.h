#ifndef GAME_CONTROLLER_H
#define GAME_CONTROLLER_H

#include <vector>
#include "takheader.h"
#include "player.h"
#include "board.h"


class GameController
{
    public:
    GameController();
    
    private:
    Board gameBoard;
    std::vector<Player> gamePlayers;
    CurrentPlayer winner;
    int round;
    bool gameOver;
    
    int getBoardSize();
    void setPlayerNames();
    CurrentPlayer flatWin();
    
    CurrentPlayer roadWin(CurrentPlayer turnPlayer);
    bool searchForRoads(int row, int column, std::vector<bool> *explored, CurrentPlayer roadOwner, Direction searchDirection, int boardSize);
    
};


#endif