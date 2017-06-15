#include <string>

#include "player.h"
using namespace std;



Player::Player(int boardSize)
{
    int i;
    int possibleBoardSizes[] = { 3, 4, 5, 6, 8 };
    int numberOfStones[] = { 10, 15, 21, 30, 50 };
    
    for(i = 0; i < 5 && boardSize != possibleBoardSizes[i]; i++);
    
    if(i != 5)
    {
        stonesLeft = numberOfStones[i];
        capsotnesLeft = i / 2;
    }
}

string Player::getName() { return mName; }
void Player::setName(string pName) {mName = pName;}
int Player::getStonesLeft(){ return stonesLeft; }
int Player::getCapstonesLeft(){ return capsotnesLeft; }


void Player::decrementStones() { stonesLeft--; }
bool Player::takeCapstone() 
{ 
    if(capsotnesLeft)
    {
        capsotnesLeft--;
        return true;
    }
    return false;
}
