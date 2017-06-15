#ifndef BOARDSLOT_H
#define BOARDSLOT_H

#include <vector>
#include <string>

#include "takheader.h"

class BoardSlot
{
    public:
    
    BoardSlot();
    int addStone(char pStone);
    std::vector<char> removeStones(int pAmount);
    
    int size();
    bool isPassable();
    bool isRoad(CurrentPlayer pOwner);
    bool isCapstone();
    std::string getStones();
    CurrentPlayer getPlayer();
    
    private: 
    std::vector<char> stones;
    char *topStone;
    CurrentPlayer mOwner;
    int mSize;
    
    void flatten();
};


#endif