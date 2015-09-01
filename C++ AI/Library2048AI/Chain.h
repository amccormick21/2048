#pragma once

#include "GameElements.h"
#include <vector>

namespace Library2048AI {
	public class Chain
	{
	public:
		enum EndType;

		Chain(const board*, const cell*, bool*);

		EndType chainEndType;
		std::vector<cell> cellChain;

	private:
		std::vector<cell> getCellChain(bool*, bool*, EndType*);
		const cell * getNextCell(const cell*, bool*, bool*, EndType*, bool*);
		void getValidDirections(const cell*, bool*, bool*);
		bool getSurroundingCells(const cell*, bool*, const cell**);
		const cell * getCellInDirection(const board*, int, int, int, bool*);
		EndType getEndType(const cell*, const cell*);

		const cell * firstCell;
		const board * boardCells;
	};
}