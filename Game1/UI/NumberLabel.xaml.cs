using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using _2048Game.Board;

namespace _2048Game.UI
{
    /// <summary>
    /// Interaction logic for NumberLabel.xaml
    /// </summary>
    public partial class NumberLabel : UserControl
    {
        private int number;
        private int row;
        private int column;
        static string[] BackColours = new string[] {"LightSteelBlue", "PaleGoldenrod", "Coral", "Orange", "OrangeRed", "Red", "Yellow", "Gold", "CornflowerBlue", "SlateBlue", "BlueViolet"};

        public NumberLabel(int value, int row, int column, double width, double height)
        {
            InitializeComponent();
            this.Number = value;
            this.Row = row;
            this.Column = column;
            this.Width = width;
            this.Height = height;
            MainGrid.Margin = new Thickness(8);
        }

        public NumberLabel(Cell cell, double width, double height)
        {
            InitializeComponent();
            this.Number = cell.Value;
            this.Row = cell.Row;
            this.Column = cell.Column;
            this.Width = width;
            this.Height = height;
            MainGrid.Margin = new Thickness(6);
        }

        /// <summary>
        /// Copy constructor for number label class
        /// </summary>
        /// <param name="numberLabbel">The number label to be copied</param>
        public NumberLabel(NumberLabel numberLabel)
        {
            InitializeComponent();
            this.Number = numberLabel.Number;
            this.Row = numberLabel.Row;
            this.Column = numberLabel.Column;
            this.Width = numberLabel.Width;
            this.Height = numberLabel.Height;
            MainGrid.Margin = new Thickness(6);
        }

        public int Number
        {
            get { return number; }
            set
            {
                number = value;

                if (number == 0)
                {
                    SetBackgrounds("White", "Black");
                    NumberTextBox.Text = "";
                }
                else
                {
                    SetBackgroundColor();
                    NumberTextBox.Text = Convert.ToString(number);
                }
            }
        }

        public int Row
        {
            get { return row; }
            set { row = value; }
        }
        public int Column
        {
            get { return column; }
            set { column = value; }
        }

        private void SetBackgroundColor()
        {
            int ColourIndex = ((int)System.Math.Log(Number, 2) - 1) % 11;
            SetBackgrounds(BackColours[ColourIndex], "Black");
        }

        private void SetBackgrounds(string BackColour, string ForeColour)
        {
            var BackBrush = (Brush)((new BrushConverter()).ConvertFromString(BackColour));
            var ForeBrush = (Brush)((new BrushConverter()).ConvertFromString(ForeColour));
            NumberTextBox.Background = BackBrush;
            NumberTextBox.Foreground = ForeBrush;
            BackgroundGrid.Background = BackBrush;
            this.BorderBrush = BackBrush;
        }

        internal DockPanel BackgroundColourPanel
        {
            get { return this.BackgroundGrid; }
            set { this.BackgroundGrid = value; }
        }

        internal void FlashGreen()
        {
            this.MainGrid.Background = (Brush)((new BrushConverter()).ConvertFromString("Lime"));
        }
        internal void GoWhite()
        {
            this.MainGrid.Background = (Brush)((new BrushConverter()).ConvertFromString("White"));
        }

        internal void FlashRed()
        {
            this.MainGrid.Background = (Brush)((new BrushConverter()).ConvertFromString("Red"));
        }

        internal void FlashBlue()
        {
            this.MainGrid.Background = (Brush)((new BrushConverter()).ConvertFromString("Blue"));
        }
    }
}
