using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2048Game.Board;

namespace _2048Game.AI
{
    public struct TreeNode : IEquatable<TreeNode>
    {
        Cell cell;
        DragDirection? longestChainDirection;
        Dictionary<DragDirection, TreeNode> pointer;
        Dictionary<DragDirection, int> chainLength;
        bool inTree;

        public TreeNode(Cell cell)
        {
            this.cell = cell;
            this.cell.InTree = true;
            pointer = new Dictionary<DragDirection, TreeNode>();
            chainLength = new Dictionary<DragDirection, int>();
            foreach (var direction in DragDirections.AllDragDirections)
            {
                pointer[direction] = new TreeNode();
                chainLength[direction] = 0;
            }
            longestChainDirection = null;
            inTree = false;
        }

        public int GetLargestLengthFromCell(out DragDirection DirectionOfLargest)
        {
            int longestLengthHolder = -1;
            DirectionOfLargest = DragDirection.Left;

            foreach (DragDirection direction in DragDirections.AllDragDirections)
            {
                if (chainLength[direction] > longestLengthHolder)
                {
                    longestLengthHolder = chainLength[direction];
                    DirectionOfLargest = direction;
                }
            }

            return longestLengthHolder;
        }

        public bool Equals(TreeNode other)
        {
            if (this.Cell.Equals(other.Cell)
                && (this.InTree == other.InTree))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public Dictionary<DragDirection, TreeNode> Pointer
        {
            get { return pointer; }
            set { pointer = value; }
        }
        public Dictionary<DragDirection, int> ChainLength
        {
            get { return chainLength; }
            set { chainLength = value; }
        }
        public DragDirection? LongestChainDirection
        {
            get { return longestChainDirection; }
            set { longestChainDirection = value; }
        }
        public PositionOnBoard Position
        {
            get { return cell.Position; }
        }
        public int Value
        {
            get { return cell.Value; }
        }
        public bool InTree
        {
            get { return inTree; }
            set { inTree = value; }
        }

        public Cell Cell
        {
            get { return cell; }
        }


    }
}
