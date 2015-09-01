// This is the main DLL file.

#include "stdafx.h"
#include "Library2048AI.h"

namespace Library2048AI {

	int AIDLLExport AIMethods::getMove(board* pBoard)
	{
		std::fstream myfile;
		myfile.open("C:\\Users\\Alex\\SkyDrive\\Documents\\Projects\\2048\\v2\\V2.6\\DeterministicAI\\debug.txt", std::ios::app);

		//Print the board for records:
		pBoard->print(&myfile);

		bool moveFound = false;
		bool availableDirections[] = { false, false, false, false };
		getValidDirections(pBoard, availableDirections);
		myfile << "Possible directions are L:" << availableDirections[0];
		myfile << ", U:" << availableDirections[1] << " R:" << availableDirections[2];
		myfile << ", D:" << availableDirections[3] << '\n';

		//Represents the direction we have elected to move in
		int direction = 0;

		//If there is only one valid direction, move this way
		direction = getOnlyValidMove(availableDirections, &moveFound);
		if (moveFound)
		{
			myfile << direction << "  -  Only valid move\n";
			myfile.close();
			return direction;
		}

		// Find the corner cell and make sure this gets populated if it is not already.
		const cell* cornerCell = getPivotCorner(pBoard);
		myfile << "Corner is at (" << cornerCell->row << ", " << cornerCell->column << ")\n";

		direction = tryFillCorner(pBoard, cornerCell, availableDirections, &moveFound);
		if (moveFound)
		{
			myfile << direction << "  -  Filling corner\n";
			myfile.close();
			return direction;
		}

		//If the corner cell is already populated we set the directions to a set that is not dangerous
		bool safeDirections[] = { false, false, false, false };
		getSafeDirections(pBoard, availableDirections, safeDirections);
		myfile << "Safe directions are L:" << safeDirections[0];
		myfile << ", U:" << safeDirections[1] << " R:" << safeDirections[2];
		myfile << ", D:" << safeDirections[3] << '\n';

		//If there is only one safe direction, move this direction
		direction = getOnlyValidMove(safeDirections, &moveFound);
		if (moveFound)
		{
			myfile << direction << "  -  Only safe move\n";
			myfile.close();
			return direction;
		}

		//Now we can start to build a chain of cells.
		//This also updates the safe directions, so that the chain is not disturbed
		Chain chain = Chain(pBoard, cornerCell, safeDirections);
		printChain(&chain, &myfile);
		direction = getMoveFromChain(&chain, &moveFound);
		if (moveFound)
		{
			myfile << direction << "  -  Moving in chain\n";
			myfile.close();
			return direction;
		}

		//Retry looking for safe moves
		direction = getOnlyValidMove(safeDirections, &moveFound);
		if (moveFound)
		{
			myfile << direction << "  -  Moving to preserve chain\n";
			myfile.close();
			return direction;
		}

		//If all else has failed, return the first available move in directions
		//This stage should never be reached
		direction = getFirstPossibleDirection(availableDirections);
		myfile << direction << "  -  First possible direction\n";
		myfile.close();
		return direction;
	}

	int AIMethods::getMove(board* pBoard, bool print)
	{
		std::fstream myfile;
		if (print)
		{
			myfile.open("C:\\Users\\Alex\\SkyDrive\\Documents\\Projects\\2048\\v2\\V2.6\\DeterministicAI\\debug.txt", std::ios::app);
			print = myfile.good();
		}
		if (!print)
		{
			myfile.close();
		}

		if (print)
			myfile << '\n';

		//Print the board for records
		if (print)
			pBoard->print(&myfile);

		bool moveFound = false;
		bool availableDirections[] = { false, false, false, false };
		getValidDirections(pBoard, availableDirections);

		if (print)
		{
			myfile << "Possible directions are L:" << availableDirections[0];
			myfile << ", U:" << availableDirections[1] << " R:" << availableDirections[2];
			myfile << ", D:" << availableDirections[3] << '\n';
		}

		//Represents the direction we have elected to move in
		int direction = 0;

		//If there is only one valid direction, move this way
		direction = getOnlyValidMove(availableDirections, &moveFound);
		if (moveFound)
		{
			if (print)
			{
				myfile << direction << "  -  Only valid move\n";
				myfile.close();
			}
			return direction;
		}

		// Find the corner cell and make sure this gets populated if it is not already.
		const cell* cornerCell = getPivotCorner(pBoard);
		if (print)
			myfile << "Corner is at (" << cornerCell->row << ", " << cornerCell->column << ")\n";

		direction = tryFillCorner(pBoard, cornerCell, availableDirections, &moveFound);
		if (moveFound)
		{
			if (print)
			{
				myfile << direction << "  -  Filling corner\n";
				myfile.close();
			}
			return direction;
		}

		//If the corner cell is already populated we set the directions to a set that is not dangerous
		bool safeDirections[] = { false, false, false, false };
		getSafeDirections(pBoard, availableDirections, safeDirections);
		if (print)
		{
			myfile << "Safe directions are L:" << safeDirections[0];
			myfile << ", U:" << safeDirections[1] << " R:" << safeDirections[2];
			myfile << ", D:" << safeDirections[3] << '\n';
		}

		//If there is only one safe direction, move this direction
		direction = getOnlyValidMove(safeDirections, &moveFound);
		if (moveFound)
		{
			if (print)
			{
				myfile << direction << "  -  Only safe move\n";
				myfile.close();
			}
			return direction;
		}

		//Now we can start to build a chain of cells.
		//This also updates the safe directions, so that the chain is not disturbed
		Chain chain = Chain(pBoard, cornerCell, safeDirections);
		if (print)
			printChain(&chain, &myfile);
		direction = getMoveFromChain(&chain, &moveFound);
		if (moveFound)
		{
			if (print)
			{
				myfile << direction << "  -  Moving in chain\n";
				myfile.close();
			}
			return direction;
		}

		//Retry looking for safe moves
		direction = getAnyValidMove(safeDirections, &moveFound);
		if (moveFound)
		{
			myfile << direction << "  -  Moving to preserve chain\n";
			myfile.close();
			return direction;
		}

		//If all else has failed, return the first available move in directions
		//This stage should never be reached
		//TODO: sometimes the system returns a direction which should not be possible. Trace this error.
		direction = getFirstPossibleDirection(availableDirections);
		if (print)
		{
			myfile << direction << "  -  First possible direction\n";
			myfile.close();
		}
		return direction;
	}

#pragma region Chain
	void AIMethods::printChain(const Chain * chain, std::fstream * myfile)
	{
		*myfile << "Chain: ";
		int chainLength = chain->cellChain.size();
		for (int i = 0; i < chainLength; i++)
		{
			*myfile << chain->cellChain[i].value << ", ";
		}
		*myfile << "\n";
	}

	int AIMethods::getMoveFromChain(const board * pBoard, bool * moveFound)
	{
		bool availableDirections[] = { false, false, false, false };
		getValidDirections(pBoard, availableDirections);

		bool safeDirections[] = { false, false, false, false };
		getSafeDirections(pBoard, availableDirections, safeDirections);

		const cell * cornerCell = getPivotCorner(pBoard);

		return getMoveFromChain(pBoard, cornerCell, safeDirections, moveFound);
	}

	int AIMethods::getMoveFromChain(const board * pBoard, const cell * cornerCell, bool * safeDirections, bool * moveFound)
	{
		Chain cellChain = Chain(pBoard, cornerCell, safeDirections);
		int moveToMake = 0;

		//First, check if a move can be made in the chain
		//Initially, this means finding equal cells
		if (cellChain.chainEndType == 2 || cellChain.chainEndType == 3)
		{
			//Find the end cells and work out the move that needs to be made
			int cellsInChain = cellChain.cellChain.size();
			cell endCell = cellChain.cellChain.at(cellsInChain - 1);
			cell penultimateCell = cellChain.cellChain.at(cellsInChain - 2);
			moveToMake = getMoveDirectionFromCells(&penultimateCell, &endCell);
			*moveFound = true;
		}
		
		return moveToMake;
	}

	int AIMethods::getMoveFromChain(const Chain * chain, bool * moveFound)
	{
		int moveToMake = 0;

		//First, check if a move can be made in the chain
		//Initially, this means finding equal cells
		if (chain->chainEndType == 2 || chain->chainEndType == 3)
		{
			//Find the end cells and work out the move that needs to be made
			int cellsInChain = chain->cellChain.size();
			cell endCell = chain->cellChain.at(cellsInChain - 1);
			cell penultimateCell = chain->cellChain.at(cellsInChain - 2);
			moveToMake = getMoveDirectionFromCells(&penultimateCell, &endCell);
			*moveFound = true;
		}

		return moveToMake;
	}
#pragma endregion

#pragma region Finding Moves
	int AIMethods::getOnlyValidMove(bool * directions, bool * moveFound)
	{
		int chosenDirection = 0;
		int movesFound = 0;
		for (int direction = 0; direction < 4; direction++)
		{
			if (directions[direction])
			{
				movesFound++;
				chosenDirection = direction;
			}
		}

		*moveFound = (movesFound == 1);
		return chosenDirection;
	}

	int AIMethods::getAnyValidMove(bool * directions, bool * moveFound)
	{
		int chosenDirection = 0;
		int movesFound = 0;
		for (int direction = 0; direction < 4; direction++)
		{
			//TODO: More ways of evaluating which way is best.
			if (directions[direction])
			{
				movesFound++;
				chosenDirection = direction;
			}
		}

		*moveFound = (movesFound >= 1);
		return chosenDirection;
	}
	
	int AIMethods::tryFillCorner(const board* pBoard, const cell* cornerCell, bool * directions, bool * moveFound)
	{
		int bestDirection = 0;
		board* movedBoard = &board();
		if (cornerCell->value == 0)
		{
			int corner[] = { cornerCell->row, cornerCell->column };
			int weightInDirection = 0;
			int highestWeight = 0;
			//Try and fill the corner by getting the biggest weight to it.
			for (int direction = 0; direction < 4; direction++)
			{
				if (directions[direction])
				{
					//Move the board
					*movedBoard = board(*pBoard);
					movedBoard->makeMove(direction);

					//Get the weight of the corner after the move
					weightInDirection = getCornerWeight(movedBoard, corner);

					//Check if this is better than we have found before
					if ((direction == 0) || weightInDirection > highestWeight)
					{
						highestWeight = weightInDirection;
						bestDirection = direction;
					}
				}
			}
			*moveFound = true;
		}
		return bestDirection;
	}

	int AIMethods::getFirstPossibleDirection(bool* directions)
	{
		for (int direction = 0; direction < 4; direction++)
		{
			if (directions[direction])
			{
				return direction;
			}
		}
		//No moves available
		return 0;
	}
#pragma endregion

#pragma region Utility Methods
	/* Gets the directions in which the board can make a valid move*/
	void AIMethods::getValidDirections(const board* pBoard, bool* validDirections)
	{
		board * newBoard = new board(*pBoard);
		bool * results = newBoard->analyseGrid();
		for (int i = 0; i < 4; i++)
		{
			validDirections[i] = results[i];
		}
		delete newBoard;
	}

	/* Determines if the board will become dangerous with a move in a particular direction.
	Board is dangerous if:
	- One row has a single gap
	- All remaining rows and columns are either full or empty
	- No move can be made within the blocks
	- This means it is possible for a block to complete a full rectangle leaving only one possible move.
	*/
	bool AIMethods::moveDangerous(const board* pBoard, int direction)
	{
		int channelsWithOneSpace = 0;
		int partiallyFullChannels = 0;
		int cellsInChannel;
		board * newBoard = new board(*pBoard);
		newBoard->makeMove(direction);
		int cellArrayIndex = 0;

		//If we can merge any cells on the grid then the move is not dangerous:
		for (int row = 0; row < 4; row++)
		{
			for (int column = 0; column < 4; column++)
			{
				cellArrayIndex = column + row * 4;
				for (int nextDirection = 0; nextDirection < 4; nextDirection++)
				{
					// The cell can merge if the next move action is set to act.
					if (newBoard->cells[cellArrayIndex].nextMoveAction[nextDirection] == Act)
					{
						return false;
					}
				}
			}
		}

		//Check along rows first.
		for (int row = 0; row < 4; row++)
		{
			cellsInChannel = 0;
			for (int column = 0; column < 4; column++)
			{
				cellArrayIndex = column + row * 4;
				if (newBoard->cells[cellArrayIndex].value)
				{
					cellsInChannel++;
				}
			}
			if (cellsInChannel == 3)
			{
				channelsWithOneSpace++;
			}
			else if (cellsInChannel != 0 && cellsInChannel != 4)
			{
				//If any row has multiple spaces then the grid is not at risk
				partiallyFullChannels++;
			}
		}
		if (channelsWithOneSpace == 1 && partiallyFullChannels == 0)
		{
			return true;
		}

		channelsWithOneSpace = 0;
		partiallyFullChannels = 0;
		//Check along columns next.
		for (int column = 0; column < 4; column++)
		{
			cellsInChannel = 0;
			for (int row = 0; row < 4; row++)
			{
				cellArrayIndex = column + row * 4;
				if (newBoard->cells[cellArrayIndex].value)
				{
					cellsInChannel++;
				}
			}
			if (cellsInChannel == 3)
			{
				channelsWithOneSpace++;
			}
			else if (cellsInChannel != 0 && cellsInChannel != 4)
			{
				//If any row has multiple spaces then the grid is not at risk
				partiallyFullChannels++;
			}
		}
		if (channelsWithOneSpace == 1 && partiallyFullChannels == 0)
		{
			return true;
		}

		//Neither sweep has revealed a dangerous row so we can continue.
		return false;
	}

	/*Gets an array of boolean values representing whether a move in the given direction
	is safe on the given board*/
	void AIMethods::getSafeDirections(const board* pBoard, bool* availableDirections, bool * safeDirections)
	{
		for (int direction = 0; direction < 4; direction++)
		{
			if (availableDirections[direction])
			{
				safeDirections[direction] = !moveDangerous(pBoard, direction);
			}
			else
			{
				safeDirections[direction] = false;
			}
		}
	}

	/* Gets the corner of the board that the rest of the board should pivot around.
	This is based on the one which has the greatest weight of tiles both in it, and with
	the potential to move in to it.
	*/
	const cell* AIMethods::getPivotCorner(const board* pBoard)
	{
		int weights[4] = { 0, 0, 0, 0 };
		int corners[4][2] = { { 3, 0 }, { 0, 0 }, { 0, 3 }, { 3, 3 } };
		int lowestWeight = 0;
		int bestCorner = 0;
		//Corners are ranked by how many moves it takes to get large cells to them.

		for (int corner = 0; corner < 4; corner++)
		{
			weights[corner] = getCornerWeight(pBoard, corners[corner]);
			if ((weights[corner] < lowestWeight) || (corner == 0))
			{
				lowestWeight = weights[corner];
				bestCorner = corner;
			}
		}

		int cellIndex = corners[bestCorner][1] + corners[bestCorner][0] * 4;
		const cell* cornerCell = pBoard->cells + cellIndex;
		return cornerCell;
	}
	int AIMethods::getCornerWeight(const board* pBoard, int corner[])
	{
		int cornerWeight = 0;
		const cell * cell;
		for (int row = 0; row < 4; row++)
		{
			for (int col = 0; col < 4; col++)
			{
				cell = pBoard->cells + row + col * 4;
				cornerWeight += getCellWeight(cell, corner);
			}
		}
		return cornerWeight;
	}
	int AIMethods::getCellWeight(const cell* valueCell, int corner[])
	{
		int cellWeight = 0;
		int channelOffsets = 0; // Holds the number of moves required to get to the corner.
		if (valueCell->value)
		{
			if (valueCell->row != corner[0])
				channelOffsets++;
			if (valueCell->column != corner[1])
				channelOffsets++;
			
			cellWeight = channelOffsets * valueCell->value;
		}
		else
		{
			cellWeight = 0;
		}
		return cellWeight;
	}

	/*Gets the direction of a move required to merge a moving cell with a static cell*/
	int AIMethods::getMoveDirectionFromCells(cell * staticCell, cell * movingCell)
	{
		int moveDirection = 0;
		int delta = 0;
		if (staticCell->row == movingCell->row)
		{
			delta = movingCell->column - staticCell->column;
			if (delta > 0)
				moveDirection = Direction::Left;
			else
				moveDirection = Direction::Right;
		}
		else
		{
			delta = movingCell->row - staticCell->row;
			if (delta > 0)
				moveDirection = Direction::Up;
			else
				moveDirection = Direction::Down;
		}
		return moveDirection;
	}
#pragma endregion
}