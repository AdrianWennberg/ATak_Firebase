#include <string>
#include <iostream>

#include "move.h"
#include "board.h"

using namespace std;



Move::Move(string pMoveString) : mMoveString(pMoveString), topStone('F'), correctMove(true), placeStone(findPlacement())
{
    if(!placeStone)
    {
        decodeMove();
    }
}



bool Move::findPlacement()
{
    if(mMoveString[0] == 'C' || mMoveString[0] == 'S' || mMoveString[0] == 'F')
    {
        if(mMoveString.length() == 3)
        {
            setPosition(mMoveString[1], mMoveString[2]);
            mStone = mMoveString[0] == 'C' ? capstone : (mMoveString[0] == 'S' ? standing : flat);
            return true;
        }
        
    }
    else if(mMoveString.length() == 2)
    {
        setPosition(mMoveString[0], mMoveString[1]);
        mStone = flat;
        return true;
    }
    return false;
}


void Move::setPosition(char column, char row)
{
    if(row > '0'&& row <= '9' && column >= 'a' && column <= 'z')
    {
        stonePosition.row = row - '1';
        stonePosition.column = column - 'a';
    }
    else
    {
        correctMove = false;
    }
}

void Move::decodeMove()
{
    int currentChar = 0;
    int i, sumDropStones = 0;
    if(mMoveString[currentChar] <='9' && mMoveString[currentChar] > '0')
    {
        pickupStones = mMoveString[currentChar++] - '0';
    }
    else
    {
        pickupStones = 1;
    }
    
    
    if(mMoveString.length() - currentChar  >= 2)
    {
    
        setPosition(mMoveString[currentChar], mMoveString[currentChar + 1]);
        
        currentChar += 2;
        char directionCharacters[4] = {'+', '>', '-', '<'};
        Direction moveDirections[4] = {upDir, rightDir, downDir, leftDir};
        
        for(i = 0; mMoveString[currentChar] != directionCharacters[i] && i < 5; i++);
        
        if(i == 5)
        {
            correctMove = false;
        }
        else
        {
            moveDirection = moveDirections[i];
        }
        currentChar++;
        
        while(currentChar < mMoveString.length() && mMoveString[currentChar] <='9' && mMoveString[currentChar] > '0')
        {
            dropStones.push_back(mMoveString[currentChar++] - '0');
            sumDropStones += dropStones.back();
        }
        
        if(dropStones.size() == 0)
        {
            dropStones.push_back(pickupStones);
            sumDropStones += dropStones.back();
        }
        
        
        if(currentChar < mMoveString.length())
        {
            if(!(mMoveString[currentChar] == 'C' || mMoveString[currentChar] == 'S'))
            {
                correctMove = false;
            }
            else
            {
                topStone = mMoveString[currentChar];
            }
        }
        
        if(++currentChar < mMoveString.length() || sumDropStones != pickupStones)
        {
            correctMove = false;
        }
    }
    else 
    {
        correctMove = false;
    }
}


bool Move::isPlacement() { return placeStone; }
StoneType Move::getPlacementStoneType() { return mStone; }
Position Move::getPosition() { return stonePosition; }
int Move::getPickupStones() { return pickupStones; }
Direction Move::getMoveDirection() { return moveDirection; }
vector<int> Move::getDropStones() { return dropStones; }
bool Move::isCorrectMove(){ return correctMove; }
char Move::getTopStone(){ return topStone; }
