objects = main.o player.o board.o move.o boardslot.o gameController.o
CXX = g++
CPPFLAGS = -IHeaders
VPATH = Headers
CC = gcc


Tak : $(objects)
	$(CXX) -o Tak $(objects)
    
main.o: board.o gameController.o

player.o: player.h takheader.h

board.o: vector2d.h board.h takheader.h boardslot.h move.o

move.o: move.h takheader.h

boardslot.o: boardslot.h takheader.h

gameController.o: gameController.h takheader.h board.o

clean:
	rm $(objects) Tak Tak.exe.stackdump