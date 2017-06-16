#ifndef VECTOR2D_H
#define VECTOR2D_H
#include <vector>

#pragma once

template <class T>
class Vector2d
{
    public:
    Vector2d(int pRows, int pColumns);
    void setPosition(int pRow, int pColumn, int value);
    T *getPositionPointer(int pRow, int pColumn);
    int getFieldSize();
    
    private:
    int mRows;
    int mColumns;
    std::vector <T> mField;
};

template <class T>
class SquareVector: public Vector2d<T>
{
    public:
    SquareVector(int size);
};

template<typename T>
Vector2d<T>::Vector2d(int pRows, int pColumns) : mRows(pRows), mColumns(pColumns), mField(pRows * pColumns) { };

template<typename T>
void Vector2d<T>::setPosition(int pRow, int pColumn, int value)
{
	if (pRow < mRows && pColumn < mColumns)
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
SquareVector<T>::SquareVector(int size) : Vector2d<T>(size, size) { };

#endif
