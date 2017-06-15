#ifndef TAKHEADER_H
#define TAKHEADER_H

#define CLEAR_SCREEN for (int n = 0; n < 10; n++) cout << "\n\n\n\n\n\n\n\n\n\n"
#define START_SLOT_SIZE 10

enum StoneType { flat, standing, capstone };
enum CurrentPlayer { none, first, second };
enum Direction { upDir, rightDir, downDir, leftDir };
struct Position { int row; int column; };



#endif