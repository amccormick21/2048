using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using V22048Game.Elements;
using V22048Game.Gameplay;
using V22048Game.MoveInformation;
using V22048UI.UI;

namespace V22048UI.Animations
{
    class MoveAnimation
    {
        List<NumberLabel> Labels;
        Canvas Board;
        double AnimationSpeed;
        MoveEventArgs EventArgs;
        bool growFinished, pulseFinished;

        private Storyboard slideStoryBoard;
        private Storyboard pulseStoryBoard;
        private Storyboard growStoryBoard;

        internal event EventHandler AllAnimationsComplete;

        public MoveAnimation(List<NumberLabel> labels, double speedOfAnimations, Canvas board)
        {
            Labels = labels;
            Board = board;
            AnimationSpeed = speedOfAnimations;
            growFinished = pulseFinished = false;
            //Completed += MoveAnimation_Completed;
        }

        /*void MoveAnimation_Completed(object sender, EventArgs e)
        {
            if (AllAnimationsComplete != null)
                //Fire event with the index of the completed animation
                AllAnimationsComplete(this, e);
        }*/

        /// <summary>
        /// Starts the animations to execute in order.
        /// </summary>
        internal void StartAnimations()
        {
            slideStoryBoard.Begin();
            //Remove the cells due to be removed
            //RemoveCells(); //New

            //Update the values of doubled cells.
            //UpdateValues(EventArgs.DoubledCells); //New
        }

        /// <summary>
        /// Sets up the storyboard ready for animations to be started.
        /// Call 'StartAnimations' to start the animations.
        /// </summary>
        internal void SetupAnimations(MoveEventArgs eventArgs)
        {
            EventArgs = eventArgs;

            //Load the move storyboard with the labels in their current position.
            SetupMotionAnimations(eventArgs.Moves);

            //Arrange the grid so that the labels now contain their destination position statistics.
            ArrangeGrid(eventArgs.Moves);

            //Set up the pulse animations based on finishing positions.
            SetupPulseAnimations(eventArgs.DoubledCells);

            //Add the new cells to the grid
            UpdateCellValue(eventArgs.NewCells);

            //Set up the animation to grow in the new cells.
            SetupGrowAnimations(eventArgs.NewCells);

            slideStoryBoard.Completed += slideStoryBoard_Completed;
            pulseFinished = eventArgs.DoubledCells.Count == 0;
            growStoryBoard.Completed += growStoryBoard_Completed;
            pulseStoryBoard.Completed += pulseStoryBoard_Completed;
            //Add the final animations to this storyboard.
            //AddChild(pulseStoryBoard);
            //AddChild(growStoryBoard);
            //AddChild(slideStoryBoard);
        }

        void pulseStoryBoard_Completed(object sender, EventArgs e)
        {
            pulseFinished = true;
            FinishAnimations();
        }

        void growStoryBoard_Completed(object sender, EventArgs e)
        {
            growFinished = true;
            FinishAnimations();
        }

        void FinishAnimations()
        {
            if (pulseFinished && growFinished && (AllAnimationsComplete != null))
                AllAnimationsComplete(this, new EventArgs());
        }

        void slideStoryBoard_Completed(object sender, EventArgs e)
        {
            //Once this is done we can start the pulse and grow storyboards
            RemoveCells();
            UpdateValues(EventArgs.DoubledCells);
            growStoryBoard.Begin();
            pulseStoryBoard.Begin();
        }

        private void SetupPulseAnimations(List<Cell> ValuesChanged)
        {
            //Pulse
            double duration = 120 - AnimationSpeed * 11;

            pulseStoryBoard = new Storyboard();
            ThicknessAnimation cellPulse;
            NumberLabel LabelToAffect;

            foreach (Cell cell in ValuesChanged)
            {
                if (GetLabel(cell.Position, out LabelToAffect))
                {
                    cellPulse = new ThicknessAnimation()
                    {
                        From = LabelToAffect.StandardMargin,
                        To = new Thickness(0),
                        Duration = new Duration(TimeSpan.FromMilliseconds(duration)),
                        AutoReverse = true,
                    };

                    pulseStoryBoard.Children.Add(cellPulse);
                    Storyboard.SetTargetProperty(cellPulse, new PropertyPath(DockPanel.MarginProperty));
                    Storyboard.SetTarget(cellPulse, LabelToAffect.BackgroundColourPanel);
                }
            }
        }

        private void SetupGrowAnimations(List<Cell> cells)
        {
            double duration = 100 - AnimationSpeed * 9;
            growStoryBoard = new Storyboard();
            ThicknessAnimation GrowSize;
            NumberLabel LabelToAdd;

            foreach (Cell cell in cells)
            {
                if (GetLabel(cell.Position, out LabelToAdd))
                {
                    GrowSize = new ThicknessAnimation()
                    {
                        From = new Thickness(LabelToAdd.Width / 2),
                        To = LabelToAdd.StandardMargin,
                        Duration = new Duration(TimeSpan.FromMilliseconds(duration)),
                        AutoReverse = false
                    };
                    growStoryBoard.Children.Add(GrowSize);
                    Storyboard.SetTargetProperty(GrowSize, new PropertyPath(DockPanel.MarginProperty));
                    Storyboard.SetTarget(GrowSize, LabelToAdd.BackgroundColourPanel);
                }
            }
        }


        private void SetupMotionAnimations(List<Move> Moves)
        {
            double duration = 200 - AnimationSpeed * 19;
            slideStoryBoard = new Storyboard();
            DoubleAnimation slide;
            foreach (Move move in Moves)
            {
                if (move.StartPosition.Column == move.EndPosition.Column) //Moving vertically
                {
                    slide = new DoubleAnimation()
                    {
                        From = GetGridPosition(move.StartPosition.Row, true),
                        To = GetGridPosition(move.EndPosition.Row, true)
                    };
                    Storyboard.SetTargetProperty(slide, new PropertyPath(Canvas.TopProperty));
                }
                else
                {
                    slide = new DoubleAnimation()
                    {
                        From = GetGridPosition(move.StartPosition.Column, false),
                        To = GetGridPosition(move.EndPosition.Column, false)
                    };
                    Storyboard.SetTargetProperty(slide, new PropertyPath(Canvas.LeftProperty));
                }
                slide.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
                slide.AutoReverse = false;

                NumberLabel LabelToAnimate;
                if (GetLabel(move.StartPosition, out LabelToAnimate))
                {
                    Storyboard.SetTarget(slide, LabelToAnimate);
                    slideStoryBoard.Children.Add(slide);
                }
            }
        }

        private void ArrangeGrid(List<Move> MovesMade)
        {
            NumberLabel labelToUpdate, labelToRemove;
            foreach (Move move in MovesMade)
            {
                if (GetLabel(move.EndPosition, out labelToRemove))
                {
                    labelToRemove.Removing = true;
                }

                if (GetLabel(move.StartPosition, out labelToUpdate))
                {
                    labelToUpdate.Row = move.EndPosition.Row;
                    labelToUpdate.Column = move.EndPosition.Column;
                }
                else
                {
                     throw new InvalidOperationException("It broke again");
                }
            }
        }

        private void UpdateValues(List<Cell> ValuesChanged)
        {
            NumberLabel label;
            foreach (Cell cell in ValuesChanged)
            {
                if (GetLabel(cell.Position, out label))
                {
                    label.Update(cell.Value.ToString());
                }
            }
        }

        private void UpdateCellValue(List<Cell> cells)
        {
            NumberLabel labelToAdd;
            foreach (Cell cell in cells)
            {
                labelToAdd = new NumberLabel(cell, Board.Width / (double)GameController.GridSize, Board.Height / (double)GameController.GridSize, true);
                Labels.Add(labelToAdd);
                Board.Children.Add(labelToAdd);
                Canvas.SetLeft(labelToAdd, GetGridPosition(labelToAdd.Column, true));
                Canvas.SetTop(labelToAdd, GetGridPosition(labelToAdd.Row, false));
            }
        }

        private void RemoveCells()
        {
            List<NumberLabel> labelsToRemove = Labels.FindAll(l => l.Removing);

            foreach (NumberLabel l in labelsToRemove)
            {
                Board.Children.Remove(l);
                Labels.Remove(l);
            }
        }

        bool GetLabel(Position position, out NumberLabel label)
        {
            label = Labels.Find(l => (l.Row == position.Row) && (l.Column == position.Column) && (!l.Removing));
            return (label != null);
        }

        double GetGridPosition(int index, bool isLeftPosition)
        {
            return index * ((isLeftPosition ? Board.Width : Board.Height) / GameController.GridSize);
        }
    }
}
