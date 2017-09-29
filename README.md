# ATak
An attempt at implementing the TAK board game. Starting of as a text based C implementation, 
then moving on to unity or another such engine.

The game of Tak is a beautiful game with very simple rules, but you need a few different board 
pieces to be able to play it. That is why I'm making this implementation, mostly for myself, 
but we'll see where it goes in the future.

I know that the game has been implemented before, most notable PlayTak.com. This is mainly a 
personal challenge, not a commercial or even that public version of the game. (yet)

The rules can be found here: http://cheapass.com/wp-content/uploads/2016/05/TakWebRules.pdf
Reddit: https://www.reddit.com/r/Tak/
PTN: https://www.reddit.com/r/Tak/wiki/portable_tak_notation

 Unity Version
 Currently works as a local game with a 5x5 board and the official rules. A server
 will (hopefully) be up soon so that the game can be run online!
 
 Here are some pictures!
 [Beginning of game image](\Screenshots\StartGameScreen.PNG)
 [Middle of game image](\Screenshots\OngoingGameScreen.PNG)
 [End of game image](\Screenshots\EndGameScreen.PNG)

 Version C++ 
 Currently finished, but there might be errors that need fixing, or things that get ironed 
 out in the future.
 
 You might need to change the makefile to suit your compiler.
 
 Currently supported:
  Placing and moving of stones.
  Official rules with all official board sizes.
  
 Might be added later:
  Scoring
  More detailed victory screen. ?
  saving your game as a text file that can be viewed using online tools.
 
 This version uses the Portable Tak Notation (PTN), to take input as to what move to make and 
 displays the current board state in text after each move.