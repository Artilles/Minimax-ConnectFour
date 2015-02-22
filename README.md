ConnectFour
===========

Connect Four AI using MiniMax algorithm in C#.

There's currently a bug in the code which causes the AI to ignore the top row of the board when checking for win conditions. Thus if the player will win next turn by placing a token in the top row, the AI will not stop the player from winning. Additionally, the AI will not make a winning move if it is in the top row.
