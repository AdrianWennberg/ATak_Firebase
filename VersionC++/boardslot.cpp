#include <iostream>
#include <string>
#include "boardslot.h"
using namespace std;


BoardSlot::BoardSlot(): stones(), topStone(NULL), mOwner(none), mSize(0) { }

/* int addStone(char pStone);
    Adds a stone to the top of the slot and returns the current amount of stones.
    If a stone can not be added, -1 is returned. If the current top stone is a standing stone
    and the added stone is a capstone, the top stone is flattened.
*/
int BoardSlot::addStone(char pStone)
{
    if(isPassable())
    {
        stones.push_back(pStone);
        topStone = &stones.back();
        mOwner = (*topStone == 'f' || *topStone == 's' || *topStone == 'c')?first:second;
        return ++mSize;
    }
    else{
        if((*topStone == 's' || *topStone == 'S') && (pStone == 'c' || pStone == 'C'))
        {
            flatten();
            stones.push_back(pStone);
            topStone = &stones.back();
            mOwner = (*topStone == 'f' || *topStone == 's' || *topStone == 'c')?first:second;
            return ++mSize;
        }
        else
        {
            // Error message
            return -1;
        }
    }
}

/* void flatten();
    Changes a standing stone into a flat stone.
*/
void BoardSlot::flatten()
{
    if(*topStone == 's')
    {
        *topStone = 'f';
    }
    else if(*topStone == 'S')
    {
        *topStone = 'F';
    }
}



/* vector<char> removeStones(int pAmount);
    Removes pAmount stones form the slot and returns the removed elements.
    If the amount is larger than the anout of elements in the slot, 
    nothing happens and an empty vector is returned.
*/
vector<char> BoardSlot::removeStones(int pAmount)
{
    vector<char> deleted;
    if(pAmount <= mSize)
    {        
        deleted.assign(stones.end() - pAmount, stones.end());
        stones.erase(stones.end() - pAmount, stones.end());
        mSize-=pAmount;
        topStone = (mSize == 0) ?  NULL : &stones.back();
        mOwner = topStone != NULL?((*topStone == 'f' || *topStone == 's' || *topStone == 'c')?first:second):none;
    }
    else
    {
        //Error message
    }
    return deleted;
}

int BoardSlot::size() { return mSize; }
bool BoardSlot::isPassable() { return (topStone == NULL || *topStone == 'f' || *topStone == 'F'); }
bool BoardSlot::isRoad(CurrentPlayer pOwner) { return (pOwner == mOwner && (isPassable() || isCapstone())); }
bool BoardSlot::isCapstone() { return (!(topStone == NULL) && (*topStone == 'c'  || *topStone == 'C')); }
CurrentPlayer BoardSlot::getPlayer() { return mOwner; } 

string BoardSlot::getStones() { return string(stones.begin(), stones.end()); }

