#ifndef MOVE_H
#define MOVE_H

#include <string>
#include <vector>

#include "takheader.h"


class Move
{
    public:
    Move(std::string pMoveString);
    
    bool isPlacement();
    StoneType getPlacementStoneType();
    Position getPosition();
    int getPickupStones();
    Direction getMoveDirection();
    std::vector<int> getDropStones();
    bool isCorrectMove();
    char getTopStone();
    
    
    private:
    std::string mMoveString;;
    char topStone;
    bool correctMove;
    bool placeStone;
    StoneType mStone;
    Position stonePosition;
    
    int pickupStones;
    Direction moveDirection;
    std::vector<int> dropStones;
    
    
    bool findPlacement();
    void setPosition(char column, char row);
    void decodeMove();
};

#endif