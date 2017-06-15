#ifndef VECTOR2D_H
#define VECTOR2D_H
#include <vector>

template <class T>
class Vector2d
{
    public:
    Vector2d(int pRows, int pColumns);
    void setPosition(int pRow, int pColumn, int value);
    T getPosition(int pRow, int pColumn);
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

#include "vector2d.cpp"

#endif