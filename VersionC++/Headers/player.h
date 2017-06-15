#ifndef PLAYER_H
#define PLAYER_H


#include <string>

#include "takheader.h"

class Player
{
    public:
        Player(int boardSize);
        
        std::string getName();
        void setName(std::string pName);
        int getStonesLeft();
        int getCapstonesLeft();
        void decrementStones();
        bool takeCapstone();
        
    private:
        std::string mName;
        int stonesLeft;
        int capsotnesLeft;
};

#endif