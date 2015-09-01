#pragma once

#include "stdafx.h"
#include "Chain.h"

namespace Library2048AI
{
	enum Chain::EndType {
		//The final cell in the chain is less than half of the next, with no gap between it and the next
		LowerNoGap = 0,
		//The final cell in the chain is less than half of the next, with a gap between it and the next
		LowerWithGap = 1,
		//The final cell in the chain is equal to the next, with no gap between it and the next
		EqualNoGap = 2,
		//The final cell in the chain is equal to the next, with a gap between it and the next
		EqualWithGap = 3,
		//There is only one cell in the chain
		OnlyOneCell = 4
	};

	Chain::Chain(const board * pBoard, const cell * startingCell, bool * validMoveDirections)
	{
		firstCell = startingCell;
		boardCells = pBoard;
		chainEndType = EndType::OnlyOneCell;
		bool validSearchDirections[4] = { true, true, true, true };
		cellChain = getCellChain(validMoveDirections, validSearchDirections, &chainEndType);
	}

	std::vector<cell> Chain::getCellChain(bool * validMoveDirections, bool * validSearchDirections, Chain::EndType * endType)
	{
		//Sorted list of all cells in the chain
		std::vector<cell> chain;

		const cell * endCell = firstCell;
		chain.push_back(*firstCell);

		//Cell found becomes false if there is nothing more to add to the chain.
		bool cellFound = true;

		do
		{
			//Update the end cell in the chain
			endCell = getNextCell(endCell, validMoveDirections, validSearchDirections, endType, &cellFound);
			if (cellFound)
			{
				//Add the cell to the chain
				chain.push_back(*endCell);
			}
			//The end type describes what cell combination is found at the end of the chain.
			//Stop searching if any equal cell is found, or a lower cell is found but has a gap
		} while (cellFound && (*endType == EndType::LowerNoGap || *endType == EndType::OnlyOneCell));

		//The chain is complete
		return chain;
	}

	const cell * Chain::getNextCell(const cell * endCell, bool * validMoveDirections, bool * validSearchDirections, Chain::EndType * endType, bool * cellFound)
	{
		const cell * nextEndCell;
		int cellValue = endCell->value;

		//This updates the valid directions array based on which way the board can be moved
		//It returns the search directions array, showing which directions we can search for cells
		getValidDirections(endCell, validMoveDirections, validSearchDirections);

		//Find the cells surrounding the cell.
		const cell * surroundingCells[4] = { &cell(), &cell(), &cell(), &cell() };
		bool anyCellsFound = getSurroundingCells(endCell, validSearchDirections, surroundingCells);

		*cellFound = false;

		//Pick the best one
		//If there is one which has the same value, select it
		int foundCellValue = 0;
		for (int searchDirection = 0; searchDirection < 4; searchDirection++)
		{
			if (surroundingCells[searchDirection])
			{
				foundCellValue = surroundingCells[searchDirection]->value;
				if (foundCellValue == cellValue)
				{
					*cellFound = true;
					nextEndCell = surroundingCells[searchDirection];
				}
			}
		}

		//If there is one with half the value, choose this
		if (!*cellFound)
		{
			for (int searchDirection = 0; searchDirection < 4; searchDirection++)
			{
				if (surroundingCells[searchDirection] && (surroundingCells[searchDirection]->value == cellValue / 2))
				{
					*cellFound = true;
					nextEndCell = surroundingCells[searchDirection];
				}
			}
		}

		//If there is one with a lower value, choose this
		int largestCellIndex;
		int largestCellValue = 0;
		if (!*cellFound)
		{
			for (int searchDirection = 0; searchDirection < 4; searchDirection++)
			{
				if (surroundingCells[searchDirection] && (surroundingCells[searchDirection]->value < cellValue) && (surroundingCells[searchDirection]->value > 0))
				{
					//Most wanted holder
					if (surroundingCells[searchDirection]->value > largestCellValue)
					{
						largestCellIndex = searchDirection;
						largestCellValue = surroundingCells[searchDirection]->value;
					}
				}
			}
			if (largestCellValue > 0)
			{
				*cellFound = true;
				nextEndCell = surroundingCells[largestCellIndex];
			}
		}

		//If we did not find a cell, the end type is not changed and is carried from last search
		if (*cellFound && nextEndCell)
			*endType = getEndType(endCell, nextEndCell);

		return nextEndCell;
	}

	/* Modifies the valid directions array to limit search directions*/
	void Chain::getValidDirections(const cell * endCell, bool * validMoveDirections, bool * validSearchDirections)
	{
		//Search directions are directions opposite to those where a move cannot be made
		//i.e. if none of the chain moves when a move down is made, up is a valid search direction
		//if the chain moves when a move right is made, left is not a valid direction
		//If a cell can move in a particular direction, the opposite direction is set to false
		for (int direction = 0; direction < 4; direction++)
		{
			//For each of the current search directions, eliminate if the cell moves the opposite way
			if (validSearchDirections[direction])
			{
				if (endCell->nextPosition[direction ^ (1 << 1)] != (direction & 1 ? endCell->row : endCell->column))
					validSearchDirections[direction] = false;
			}

			//For each of the current move directions, eliminate if the cell moves the same way
			if (validMoveDirections[direction])
			{
				if (endCell->nextPosition[direction] != (direction & 1 ? endCell->row : endCell->column))
					validMoveDirections[direction] = false;
			}
		}
	}

	/* Finds the cells surrounding a given cell at the end of a chain */
	bool Chain::getSurroundingCells(const cell * endCell, bool * validSearchDirections, const cell** surroundingCells)
	{
		const cell * nextCellInDirection;
		bool cellInDirection = false;
		bool anyCellsFound = false;
		for (int searchDirection = 0; searchDirection < 4; searchDirection++)
		{
			if (validSearchDirections[searchDirection])
			{
				//This is a valid direction in which to search
				nextCellInDirection = getCellInDirection(boardCells, searchDirection, endCell->row, endCell->column, &cellInDirection);
				if (cellInDirection)
				{
					anyCellsFound = true;
					surroundingCells[searchDirection] = nextCellInDirection;
				}
			}
		}
		return anyCellsFound;
	}

	const cell * Chain::getCellInDirection(const board * pBoard, int searchDirection, int row, int column, bool * cellInDirection)
	{
		const cell * nextCellInDirection;
		int rowStep = 0;
		int columnStep = 0;
		//Left and right give zero. Up gives -1, down +1
		if (searchDirection & 1)
		{
			//Up or down
			rowStep = -2 + searchDirection;
		}
		else
		{
			columnStep = searchDirection - 1;
		}

		bool cellFound = false;
		//Keep adding steps while still within range
		while ((row + rowStep >= 0) && (row + rowStep < 4) && (column + columnStep >= 0) && (column + columnStep < 4) && !cellFound)
		{
			row += rowStep;
			column += columnStep;

			if (pBoard->cells[row * 4 + column].value != 0)
				cellFound = true;
			nextCellInDirection = pBoard->cells + row * 4 + column;
		}
		*cellInDirection = cellFound;
		return nextCellInDirection;
	}

	/* Determines if there is a gap and if the values are equal or different, and
	returns the appropriate chain end type */
	Chain::EndType Chain::getEndType(const cell * endCell, const cell * nextCell)
	{
		EndType endType;
		if (std::abs(nextCell->row - endCell->row + nextCell->column - endCell->column) > 1)
		{
			//There is a gap
			endType = (endCell->value == nextCell->value ? EndType::EqualWithGap : EndType::LowerWithGap);
		}
		else
		{
			endType = (endCell->value == nextCell->value ? EndType::EqualNoGap : EndType::LowerNoGap);
		}
		return endType;
	}
}