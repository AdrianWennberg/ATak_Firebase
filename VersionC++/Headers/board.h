#ifndef BOARD_H
#define BOARD_H

#include "takheader.h"
#include "vector2d.h"
#include "boardslot.h"
#include "move.h"


class Board
{
    public:
    Board(int pBoardSize);
    void printBoard();
    int getBoardSize();
    BoardSlot *getPositionPointer(int row, int column);
    bool isOnBoard(int row, int column);
    
    bool tryMove(Move playerMove, CurrentPlayer player);
    
    private: 
    int mBoardSize;
    int mSlotSize;
    SquareVector<BoardSlot> mBoard;
    
    void printBoardLine();
    void printSlotRow(int rowNumber);
    void printColumnLetters();
    
    bool tryPlacement(Move playerMove, CurrentPlayer player);
};

#endif