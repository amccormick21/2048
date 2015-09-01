#pragma once

//Move directions
enum Direction {
	Left = 0,
	Up = 1,
	Right = 2,
	Down = 3
};

//Move actions
enum MoveAction {
	SetZero = 0,
	StayConstant = 1,
	Act = 2
};

public struct cell {
public:
	int value;
	int row;
	int column;
	int nextPosition[4];
	MoveAction nextMoveAction[4];
	int nextMoveMergeValue[4];
};


public struct board {
public:
	int gridSize;
	cell cells[16];
	void makeMove(int);
	void print(std::fstream *);
	bool* analyseGrid();

private:
	void moveCell(cell*, int, int, bool);
	void swapProperties(cell*, cell*);
};


