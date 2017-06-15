#include <iostream>
#include "vector2d.h"
using namespace std;

int main()
{
    int boardSize;
    cout << "Board Size: ";
    cin >> boardSize;
    
    SquareVector <int> myBoard(boardSize);
    
    for(int i = 0; i < boardSize; i++)
    {
        for(int j = 0; j < boardSize; j++)
        {
            myBoard.setPosition(i, j, i * boardSize + j);
            cout << myBoard.getPosiition(i, j) << " ";
        }
        cout << "\n";
    }
    
    cout << myBoard.getFieldSize();
}