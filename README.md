# 2048
Artificial Intelligence for the game 2048

2048 is a simple logic puzzle, originally developed by Gabriele Cirulli <a href="gabrielecirulli.github.io/2048/">here</a>.
It can be played according to simple rules and hence is an excellent test-bed for artificial intelligences.

This repo contains the C# code required to play the game, in an executable, using arrow keys to move the grid. It also contains code to play the game itself, analysing the grid at each stage and selecting a move which moves the game closer to a 2048 tile. The game analyses only the grid in the current state and does not try to search at a depth greater than 1.

Development opportunities include building stepping stones to help the game slot cells into the chains, particularly if the chain has been corrupted at any point by a lower cell. Routines to move the cells back into the corner of the grid if it has been disturbed will also prove beneficial in this instance.
