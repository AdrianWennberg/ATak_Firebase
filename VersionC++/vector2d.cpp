#include <vector>
#include "vector2d.h"
using namespace std;

template<typename T> 
Vector2d<T>::Vector2d(int pRows, int pColumns): mRows(pRows), mColumns(pColumns), mField(pRows * pColumns) { }
    
template<typename T> 
void Vector2d<T>::setPosition(int pRow, int pColumn, int value)
{
    if(pRow < mRows && pColumn < mColumns)
    {
        mField[pRow * mColumns + pColumn] = value;
    }
};
    
template<typename T> 
T *Vector2d<T>::getPositionPointer(int pRow, int pColumn)
{
    return &(mField[pRow * mColumns + pColumn]);
};
    
template<typename T> 
int Vector2d<T>::getFieldSize()
{
    return mField.size();
};


template<typename T> 
SquareVector<T>::SquareVector(int size): Vector2d<T>(size, size) { }
