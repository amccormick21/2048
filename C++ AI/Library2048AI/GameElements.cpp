#include "stdafx.h"
#include "GameElements.h"

/* Prints the board to the given stream */
void board::print(std::fstream * stream)
{
	int gridSize = 4;
	int arrayIndex;
	for (int row = 0; row < gridSize; row++)
	{
		for (int col = 0; col < gridSize; col++)
		{
			arrayIndex = row * gridSize + col;
			*stream << (cells + arrayIndex)->value << '\t';
		}
		*stream << '\n';
	}
}

/* Moves all cells on the grid in the given direction.
Re-analyses the grid to prepare for the next moves and analysis.
*/
void board::makeMove(int direction)
{
	//move cells on the grid in the specified direction
	int minorChannelStart = ((direction >> 1) == 0 ? 0 : 3);
	int minorChannelIncrement = ((direction >> 1) == 0 ? 1 : -1);

	int row, column, arrayIndex;
	int minorChannelLastEmptyCell;
	int minorChannelLastValueCell;
	int lastValue;
	cell* checkCell;

	for (int majorChannel = 0; majorChannel < 4; majorChannel++)
	{
		minorChannelLastEmptyCell = -1;
		minorChannelLastValueCell = -1;
		lastValue = -1;
		for (int minorChannel = minorChannelStart; minorChannel < 4 && minorChannel >= 0; minorChannel += minorChannelIncrement)
		{
			//Allocate the major and minor channels to the cell that is being checked:
			row = ((direction & 1) == 0 ? minorChannel : majorChannel);
			column = ((direction & 1) == 0 ? majorChannel : minorChannel);
			arrayIndex = column + row * 4;
			//Allocate the cell that is currently under investigation
			checkCell = cells + arrayIndex;

			if (checkCell->value == 0)
			{
				//Empty cell found
				//If the last free cell is not already defined, define it
				if (minorChannelLastEmptyCell == -1)
				{
					minorChannelLastEmptyCell = minorChannel;
				}

			}
			else if (checkCell->value == lastValue)
			{
				//Matching value found
				moveCell(checkCell, direction, minorChannelLastValueCell, true);
				//Merge occurred so set the last value to -1 again.
				lastValue = -1;
				minorChannelLastValueCell = -1;
				//The last empty cell must now be next to the mergers
				minorChannelLastEmptyCell = minorChannel + minorChannelIncrement;
			}
			else
			{
				//A value cell which does not match the last one is found
				//If last free cell is not -1, move to here
				if (minorChannelLastEmptyCell != -1)
				{
					//Move cell to minorChannelLastEmptyCell
					moveCell(checkCell, direction, minorChannelLastEmptyCell, false);
					minorChannelLastValueCell = minorChannelLastEmptyCell;
					minorChannelLastEmptyCell = minorChannel + minorChannelIncrement;
				}
				else
				{
					//The last empty cell position is -1 so this becomes the last value cell.
					minorChannelLastValueCell = minorChannel;
				}
				lastValue = checkCell->value;
			}
		}
	}

	//Analyse the grid to make sure that any subsequent analysis is predicated on the right assumptions.
	analyseGrid();
}
/* Analyses the board to allocate values to each cell:
	- Next move location
	- Next move action (i.e. double, stay constant, or zero)
	- Next move value (if the cell merges, else -1)
*/
/* Analyses the board to allocate values to each cell:
- Next move location
- Next move action (i.e. double, stay constant, or zero)
- Next move value (if the cell merges, else -1)
*/
bool* board::analyseGrid()
{
	int row, column, arrayIndex;
	int minorChannelLastEmptyCell;
	int minorChannelLastValueCell;
	int lastValue;
	cell* checkCell;
	cell* lastValueCell;
	bool canMove[4] = { false, false, false, false };

	//move cells on the grid in the specified direction
	for (int direction = 0; direction < 4; direction++)
	{
		int minorChannelStart = ((direction >> 1) == 0 ? 0 : 3);
		int minorChannelIncrement = ((direction >> 1) == 0 ? 1 : -1);

		for (int majorChannel = 0; majorChannel < 4; majorChannel++)
		{
			minorChannelLastEmptyCell = -1;
			minorChannelLastValueCell = -1;
			lastValue = -1;
			lastValueCell = &(cell());
			for (int minorChannel = minorChannelStart; minorChannel < 4 && minorChannel >= 0; minorChannel += minorChannelIncrement)
			{
				//Allocate the major and minor channels to the cell that is being checked:
				row = ((direction & 1) == 0 ? majorChannel : minorChannel);
				column = ((direction & 1) == 0 ? minorChannel : majorChannel);
				arrayIndex = column + row * 4;

				//Allocate the cell that is currently under investigation
				checkCell = cells + arrayIndex;
				checkCell->nextMoveAction[direction] = StayConstant;
				checkCell->nextMoveMergeValue[direction] = -1;
				checkCell->nextPosition[direction] = minorChannel;

				if (checkCell->value == 0)
				{
					//Empty cell found
					//If the last free cell is not already defined, define it
					if (minorChannelLastEmptyCell == -1)
					{
						minorChannelLastEmptyCell = minorChannel;
					}

				}
				else if (checkCell->value == lastValue)
				{
					//Matching value found
					checkCell->nextMoveAction[direction] = Act;
					checkCell->nextMoveMergeValue[direction] = lastValue;
					checkCell->nextPosition[direction] = minorChannelLastValueCell;
					lastValueCell->nextMoveAction[direction] = SetZero;
					canMove[direction] = true;

					//Merge occurred so set the last value to -1 again.
					lastValue = -1;
					minorChannelLastValueCell = -1;
					//The last empty cell must now be next to the mergers
					minorChannelLastEmptyCell = minorChannel + minorChannelIncrement;
				}
				else
				{
					//A value cell which does not match the last one is found
					//If last free cell is not -1, move to here
					if (minorChannelLastEmptyCell != -1)
					{
						//If it is not already in the position, the board can be moved
						if ((direction & 1 ? checkCell->row : checkCell->column) != minorChannelLastEmptyCell)
							canMove[direction] = true;

						//Move cell to minorChannelLastEmptyCell
						checkCell->nextPosition[direction] = minorChannelLastEmptyCell;
						minorChannelLastValueCell = minorChannelLastEmptyCell;
						minorChannelLastEmptyCell = minorChannel + minorChannelIncrement;
					}
					else
					{
						//The last empty cell position is -1 so this becomes the last value cell.
						minorChannelLastValueCell = minorChannel;
					}
					checkCell->nextMoveAction[direction] = StayConstant;
					lastValue = checkCell->value;
					lastValueCell = checkCell;
				}
			}
		}
	}
	return canMove;
}
/* Moves a cell to the specified new position in the specified direction.
Boolean determines whether to double the end result or not; simulating a merge or not.
*/
void board::moveCell(cell* originalCell, int direction, int newPosition, bool doubleResult)
{
	int originalRow = originalCell->row;
	int originalColumn = originalCell->column;
	int newRow = ((direction & 1) == 0 ? newPosition : originalColumn);
	int newColumn = ((direction & 1) == 0 ? originalRow : newPosition);

	cell* newCell = cells + newColumn + newRow * 4;

	// Swap properties of old and new cells
	swapProperties(originalCell, newCell);
	if (doubleResult)
	{
		newCell->value *= 2;
	}
	// Set old cell to zero.
	originalCell->value = 0;
}
/* Swaps the properties of two cells and then sets the original cell to zero.
This simulates a move being made which moves the new cell to the location of the original cell
*/
void board::swapProperties(cell* originalCell, cell* newCell)
{
	int newValue = newCell->value;
	newCell->value = originalCell->value;
	originalCell->value = newValue;
}