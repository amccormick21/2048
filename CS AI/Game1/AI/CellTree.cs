using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2048Game.Board;

namespace _2048Game.AI
{
    public class CellTree
    {
        List<TreeNode> Nodes;
        Dictionary<DragDirection, int> highestCellMovedInDirection;
        PlayingGrid HostGrid;
        PositionOnBoard basePosition;
        int chainLength;

        public CellTree(PlayingGrid hostGrid)
        {
            Nodes = new List<TreeNode>();
            highestCellMovedInDirection = new Dictionary<DragDirection, int>();
            HostGrid = hostGrid;
            basePosition = hostGrid.GetBasePosition();
            SetupCellTree(basePosition, hostGrid);
            GetPossibleDirections();
        }

        private void SetupCellTree(PositionOnBoard basePosition, PlayingGrid hostGrid)
        {
            foreach (DragDirection direction in DragDirections.AllDragDirections)
            {
                highestCellMovedInDirection[direction] = 0;
            }

            Nodes.Add(PopulateCell(basePosition, hostGrid, false, out chainLength));
        }

        /// <summary>
        /// Populates a tree node, and initiates the populating of all child nodes.
        /// </summary>
        /// <param name="Cell">The cell to be used as a base for the cell being populated.</param>
        /// <param name="PossibleDirections">A collection of directiosn which will not move any of the cell's parents.</param>
        /// <param name="hostGrid">The grid on which the tree is being built</param>
        /// <param name="endOfRoot">True if this is the end of a root of the tree</param>
        /// <returns>A populated cell</returns>
        private TreeNode PopulateCell(PositionOnBoard position, PlayingGrid hostGrid, bool endOfRoot, out int completeChainDepth)
        {
            PositionOnBoard newPosition;
            DragDirection actualSearchDirection;
            DragDirection longestChainDirection = DragDirection.Left;
            Cell cellAtNewPosition;
            TreeNode nodeAtNewPosition;
            bool isEqual, allHigherThan;
            int highestLessThan;
            int cellValueToMatch;
            int longestChainDepthHolder = 0;
            completeChainDepth = 0;

            TreeNode CurrentCell = new TreeNode(hostGrid[position]);
            cellValueToMatch = CurrentCell.Value;
            CurrentCell.InTree = true;

            foreach (DragDirection direction in DragDirections.AllDragDirections)
            {
                actualSearchDirection = DragDirections.Opposite(direction);
                newPosition = PlayingGrid.GetPositionInDirection(CurrentCell.Position, actualSearchDirection);

                if (!newPosition.Equals(PositionOnBoard.nullPosition))
                {
                    if (!endOfRoot)
                    {
                        cellAtNewPosition = hostGrid[newPosition];
                        cellAtNewPosition.IsSurrounded(out isEqual, out allHigherThan, out highestLessThan, hostGrid);
                        if (!allHigherThan && cellAtNewPosition.Value != 0)
                        {
                            if (cellAtNewPosition.Value < cellValueToMatch)
                            {
                                nodeAtNewPosition = PopulateCell(newPosition, hostGrid, false, out completeChainDepth);
                                Nodes.Add(nodeAtNewPosition);
                                completeChainDepth++;
                                if (completeChainDepth > longestChainDepthHolder)
                                {
                                    longestChainDepthHolder = completeChainDepth;
                                    longestChainDirection = actualSearchDirection;
                                }
                                CurrentCell.Pointer[actualSearchDirection] = nodeAtNewPosition;
                            }
                            else
                            {
                                if (cellAtNewPosition.Value == cellValueToMatch)
                                {
                                    nodeAtNewPosition = PopulateCell(newPosition, hostGrid, true, out completeChainDepth);
                                    Nodes.Add(nodeAtNewPosition);
                                    completeChainDepth++;
                                    if (completeChainDepth > longestChainDepthHolder)
                                    {
                                        longestChainDepthHolder = completeChainDepth;
                                        longestChainDirection = actualSearchDirection;
                                    }
                                    CurrentCell.Pointer[actualSearchDirection] = nodeAtNewPosition;
                                }
                            }
                        }
                    }

                    CurrentCell.ChainLength[actualSearchDirection] = longestChainDepthHolder;
                    if (longestChainDepthHolder > 0)
                    {
                        CurrentCell.LongestChainDirection = longestChainDirection;
                    }
                }
            }

            return CurrentCell;
        }

        private void GetPossibleDirections()
        {
            TreeNode nodeInDirection;
            int highestCellMoved;

            foreach (DragDirection direction in DragDirections.AllDragDirections)
            {
                highestCellMoved = 0;

                foreach (TreeNode node in Nodes)
                {
                    if (node.Cell.MaxScaleInDirection[direction] != 0)
                    {
                        if (GetNodeInDirection(node, direction, out nodeInDirection))
                        {
                            if (nodeInDirection.Value != node.Value)
                            {
                                if (node.Value > highestCellMoved)
                                {
                                    highestCellMoved = node.Value;
                                }
                            }
                        }
                        else
                        {
                            if (node.Value > highestCellMoved)
                            {
                                highestCellMoved = node.Value;
                            }
                        }
                    }
                }

                if (highestCellMoved > highestCellMovedInDirection[direction])
                {
                    highestCellMovedInDirection[direction] = highestCellMoved;
                }
            }
        }

        internal TreeNode GetLastCellInTree(TreeNode currentNode)
        {
            TreeNode nodeBeingChecked = new TreeNode();

            if (!currentNode.Equals(new TreeNode()))
            {
                DragDirection directionOfLongestChain;
                if (nodeBeingChecked.LongestChainDirection.HasValue)
                {
                    directionOfLongestChain = (DragDirection)nodeBeingChecked.LongestChainDirection.Value;
                }
                else
                {
                    return nodeBeingChecked;
                }

                if (nodeBeingChecked.Pointer[directionOfLongestChain].Equals(new TreeNode()))
                {
                    return nodeBeingChecked;
                }
                else
                {
                    TreeNode cellInLongestDirection = nodeBeingChecked.Pointer[directionOfLongestChain];

                    if (!cellInLongestDirection.Equals(new TreeNode()))
                    {
                        if (!cellInLongestDirection.InTree)
                        {
                            return nodeBeingChecked;
                        }
                        else
                        {
                            return GetLastCellInTree(nodeBeingChecked.Pointer[directionOfLongestChain]);
                        }
                    }
                }
            }

            return nodeBeingChecked;
        }

        internal TreeNode GetLastCellInTree()
        {
            TreeNode baseNode;
            GetNodeAtPosition(basePosition, out baseNode);
            return GetLastCellInTree(baseNode);
        }

        private bool GetNodeInDirection(TreeNode node, DragDirection direction, out TreeNode nodeInDirection)
        {
            nodeInDirection = new TreeNode();

            foreach (TreeNode checkNode in Nodes)
            {
                if (checkNode.Position.Equals(PlayingGrid.GetPositionInDirection(node.Position, direction)))
                {
                    nodeInDirection = checkNode;
                    return true;
                }
            }

            return false;
        }

        private bool GetNodeAtPosition(PositionOnBoard position, out TreeNode nodeAtPosition)
        {
            nodeAtPosition = new TreeNode();
            foreach (TreeNode checkNode in Nodes)
            {
                if (checkNode.Position.Equals(position))
                {
                    nodeAtPosition = checkNode;
                    return true;
                }
            }
            return false;
        }

        internal TreeNode LastNode
        {
            get
            {
                TreeNode baseNode;
                GetNodeAtPosition(basePosition, out baseNode);
                return baseNode;
            }
        }

        internal List<TreeNode> NodesInChain
        {
            get { return Nodes; }
            set { Nodes = value; }
        }

        internal Dictionary<DragDirection, int> HighestCellMovedInDirection
        {
            get { return highestCellMovedInDirection; }
            set { highestCellMovedInDirection = value; }
        }
    }
}
