#include <iostream>
#include <string>
#include <vector>
#include "board.h"
#include "move.h"
using namespace std;



Board::Board(int pBoardSize): mBoardSize(pBoardSize), mSlotSize(START_SLOT_SIZE), mBoard(pBoardSize){} 
int Board::getBoardSize(){ return mBoardSize; }
BoardSlot *Board::getPositionPointer(int row, int column){ return mBoard.getPositionPointer(row, column); }


void Board::printBoard()
{
    printBoardLine();
    for(int row = mBoardSize - 1; row >= 0; row--)
    {
        printSlotRow(row);
        printBoardLine();
    }
    printColumnLetters();
}


void Board::printBoardLine()
{
    cout << "\n    |" << string(mSlotSize * mBoardSize + (mBoardSize - 1), '-') << "|";
}

void Board::printSlotRow(int rowNumber)
{
    int offset = rowNumber * mBoardSize;
    int slotSize, rightSide, leftSide;
    cout << "\n  " << rowNumber + 1 << " |";
    
    for(int i = 0; i < mBoardSize; i++)
    {
        slotSize = mBoard.getPositionPointer(rowNumber, i)->size();
        rightSide = (mSlotSize - slotSize) / 2;
        leftSide = (mSlotSize - slotSize + 1) / 2;
        
        cout << string(rightSide, ' ') << mBoard.getPositionPointer(rowNumber, i)->getStones() << string(leftSide, ' ') << '|'; 
    }
}

void Board::printColumnLetters()
{
    cout << "\n     " << string(mSlotSize / 2, ' ');
    for(int i = 0; i < mBoardSize; i++)
    {
        cout << (char)('a' + i) << string(mSlotSize, ' ');
    }
}

bool Board::tryMove(Move playerMove, CurrentPlayer player)
{
    if(playerMove.isPlacement())
    {
        return tryPlacement(playerMove, player);
    }
    
    
    Position startPosition = playerMove.getPosition();
    BoardSlot *startSlot = mBoard.getPositionPointer(startPosition.row, startPosition.column);
    
    if(player != startSlot->getPlayer())
    {
        cout << "\n    That's not your stone.";
        return false;
    }
    
    
    vector<BoardSlot*> moveToSlots;
    vector<char> moveStones;
    int moveLength = playerMove.getDropStones().size();
    int amountOfStones = playerMove.getPickupStones();
    
    for(int i = 1; i <=moveLength; i++)
    {
        switch(playerMove.getMoveDirection())
        {
            case upDir:
            if(isOnBoard(startPosition.row + i, startPosition.column))
            {
                moveToSlots.push_back(mBoard.getPositionPointer(startPosition.row + i, startPosition.column));
            }
            else 
            {
                cout << "\n    That would move you off the board";
                return false; 
            }
            break;
            case downDir:
            if(isOnBoard(startPosition.row - i, startPosition.column))
            {
                moveToSlots.push_back(mBoard.getPositionPointer(startPosition.row - i, startPosition.column));
            }
            else 
            {
                cout << "\n    That would move you off the board";
                return false; 
            }
            break;
            case leftDir:
            if(isOnBoard(startPosition.row, startPosition.column - i))
            {
                moveToSlots.push_back(mBoard.getPositionPointer(startPosition.row, startPosition.column - i));
            }
            else 
            {
                cout << "\n    That would move you off the board";
                return false; 
            }
            break;
            case rightDir:
            if(isOnBoard(startPosition.row, startPosition.column + i))
            {
                moveToSlots.push_back(mBoard.getPositionPointer(startPosition.row, startPosition.column + i));
            }
            else 
            {
                cout << "\n    That would move you off the board";
                return false; 
            }
            break;
            default:
            return false;
            break;
        }
        
        
    }
    
    for(int i = 0; i < moveLength - 1; i++)
    {
        if(!(moveToSlots[i]->isPassable()))
        { 
            cout << "\n    There is a " << (moveToSlots[i]->isCapstone()?"capstone":"standing stone")<< " in the way.";
            return false; 
        }
    }
    
    if(moveToSlots.back()->isCapstone())
    {
        cout << "\n    There is a capstone in the way.";
        return false;
    }
    
    if(startSlot->isCapstone())
    {
        if(playerMove.getTopStone() != 'C')
        { 
            cout << "\n    Add C to the end to move a capstone.";
            return false; 
        }
        if(!(moveToSlots.back()->isPassable()) && playerMove.getDropStones().back() != 1)
        { 
            cout << "\n    The capstone needs to be alone to flatten a standing stone.";
            return false; 
        }
    }
    else if(!(startSlot->isPassable()))
    { 
        if(playerMove.getTopStone() != 'S')
        { 
            cout << "\n    Add S to the end to move a standing stone.";
            return false; 
        }
        else if(!(moveToSlots.back()->isPassable()))
        {
            cout << "\n    There is a standing stone in the way.";
            return false; 
        }
    }
    else if(playerMove.getTopStone() != 'F') 
    { 
        cout << "\n    This is a flat stone.";
        return false; 
    }
    else if(!(moveToSlots.back()->isPassable()))
    {
        cout << "\n    There is a standing stone in the way.";
        return false; 
    }
    
    
    
    if(amountOfStones <= mBoardSize && startSlot->size() >= amountOfStones)
    {
        moveStones = startSlot->removeStones(amountOfStones);
    }
    else
    {
        if(amountOfStones <= mBoardSize)
        {
            cout << "\n    You can only move up to " << mBoardSize << " stones at a time.";
        }
        else
        {
            cout << "\n    There" << ((startSlot->size() > 1)?" are":" is") << " only " << startSlot->size() << ((startSlot->size() > 1)?" stones":" stone") << " in that slot.";
        }
        return false;
    }
    
    for(int currentPosition = 0; currentPosition < moveLength; currentPosition++)
    {
        for(int stoneNumber = 0; stoneNumber < playerMove.getDropStones()[currentPosition]; stoneNumber++)
        {            
            if(moveToSlots[currentPosition]->addStone(moveStones.front()) > mSlotSize - 2)
            {
                mSlotSize++;
            }
            moveStones.erase(moveStones.begin());
        }
    }
    return true;
}

bool Board::isOnBoard(int row, int column)
{ 
    if(row >= 0 && column >= 0 && row < mBoardSize && column< mBoardSize)
    {
        return true;
    }
    else
    {
        return false;
    }
}


bool Board::tryPlacement(Move playerMove, CurrentPlayer player)
{
    Position placementPosition = playerMove.getPosition();
    BoardSlot *placementSlot = mBoard.getPositionPointer(placementPosition.row, placementPosition.column); 
    
    if(placementPosition.row < mBoardSize && placementPosition.column < mBoardSize && placementSlot->size() == 0)
    {
        
        char stoneTypes[6] = {'f', 's', 'c', 'F', 'S', 'C'};
        placementSlot->addStone(stoneTypes[playerMove.getPlacementStoneType() + 3 * (player - 1)]);
        return true;
    }
    else
    {
    
        cout << "\n    You cannot place a stone there.";
        return false;
    }
}