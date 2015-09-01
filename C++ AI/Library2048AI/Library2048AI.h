// Library2048AI.h

#pragma once

#include "GameElements.h"
#include "Chain.h"

#ifndef __cplusplus
extern "C" {
#endif

#define AIDLLExport _declspec(dllexport)

	using namespace System;

	namespace Library2048AI {

		public class AIMethods
		{
		public:
			int AIDLLExport getMove(board*);

		private:
			int getMove(board*, bool );
			void printChain(const Chain *, std::fstream *);
			int getMoveFromChain(const Chain *, bool *);
			int getMoveFromChain(const board*, bool*);
			int getMoveFromChain(const board*, const cell*, bool*, bool*);
			int getOnlyValidMove(bool*, bool*);
			int getAnyValidMove(bool*, bool*);
			int tryFillCorner(const board*, const cell*, bool*, bool*);
			int getFirstPossibleDirection(bool*);

			void getValidDirections(const board*, bool*);
			bool moveDangerous(const board*, int);
			void getSafeDirections(const board*, bool*, bool*);
			const cell* getPivotCorner(const board*);
			int getCornerWeight(const board*, int[]);
			int getCellWeight(const cell*, int[]);
			int getMoveDirectionFromCells(cell*, cell*);
			// TODO: Add your methods for this class here.
		};
	}
#ifndef __cplusplus
}
#endif
