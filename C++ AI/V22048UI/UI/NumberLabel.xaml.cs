using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using V22048Game.Elements;
using V22048Game.Gameplay;

namespace V22048UI.UI
{
    /// <summary>
    /// Interaction logic for NumberLabel.xaml
    /// </summary>
    public partial class NumberLabel : UserControl
    {
        private int generation;
        private int row;
        private int column;
        internal bool Removing { get; set; }
        internal Thickness StandardMargin;

        static Color[] BackColours = new Color[] {
            Color.FromArgb(255, 150, 210, 255), //Light blue    2
            Color.FromArgb(255, 104, 143, 255), //Darker blue   4
            Color.FromArgb(255, 80, 120, 255),  //Blue          8
            Color.FromArgb(255, 255, 160, 40),  //Light Orange  16
            Color.FromArgb(255, 255, 130, 25),  //Dark Orange   32
            Color.FromArgb(255, 255, 100, 80),  //Salmon        64
            Color.FromArgb(255, 255, 60, 40),   //Red           128
            Color.FromArgb(255, 108, 255, 83),  //Light Green   256
            Color.FromArgb(255, 136, 255, 40),  //Green         512
            Color.FromArgb(255, 255, 255, 23),  //Yellow        1024
            Color.FromArgb(255, 255, 200, 15),  //Gold          2048
            Color.FromArgb(255, 150, 255, 0),   //Dark Green    4096
        };

        public NumberLabel(int generation, int row, int column, double width, double height)
        {
            InitializeComponent();
            this.generation = generation;
            this.Row = row;
            this.Column = column;
            this.Width = width;
            this.Height = height;
            this.Removing = false;
            StandardMargin = new Thickness(width / 20);
            ColourBorder.Margin = StandardMargin;
            if (GameController.GridSize > 20)
                NumberTextBox.Visibility = System.Windows.Visibility.Hidden;
            if (GameController.GridSize < 20)
                ColourBorder.CornerRadius = new CornerRadius(8 - Math.Sqrt(GameController.GridSize));
            else
                ColourBorder.CornerRadius = new CornerRadius(0);

            Update("");
        }

        public NumberLabel(Cell cell, double width, double height, bool toBeAnimated)
        {
            InitializeComponent();
            this.generation = cell.Generation;
            this.Row = cell.Row;
            this.Column = cell.Column;
            this.Width = width;
            this.Height = height;
            this.Removing = false;
            StandardMargin = new Thickness(width / 20);
            ColourBorder.Margin = toBeAnimated ? new Thickness(width / 2) : StandardMargin;
            if (GameController.GridSize > 20)
                NumberTextBox.Visibility = System.Windows.Visibility.Hidden;
            if (GameController.GridSize < 20)
                ColourBorder.CornerRadius = new CornerRadius(8 - Math.Sqrt(GameController.GridSize));
            else
                ColourBorder.CornerRadius = new CornerRadius(0);

            Update(cell.Value.ToString());
        }

        /// <summary>
        /// Copy constructor for number label class
        /// </summary>
        /// <param name="numberLabel">The number label to be copied</param>
        public NumberLabel(NumberLabel numberLabel)
        {
            InitializeComponent();
            this.generation = numberLabel.generation;
            this.Row = numberLabel.Row;
            this.Column = numberLabel.Column;
            this.Width = numberLabel.Width;
            this.Height = numberLabel.Height;
            this.Removing = false;
            StandardMargin = new Thickness(numberLabel.Width / 20);
            ColourBorder.Margin = StandardMargin;
            ColourBorder.CornerRadius = numberLabel.ColourBorder.CornerRadius;
            if (GameController.GridSize > 20)
                NumberTextBox.Visibility = System.Windows.Visibility.Hidden;
            Update(numberLabel.NumberTextBox.Text);
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

        public void Update(string textToShow)
        {
            SetNumber(generation++, textToShow);
        }

        private void SetNumber(int generation, string stringToDisplay)
        {
            if (generation == 0)
            {
                SetBackgrounds(Colors.White, Colors.Black);
                NumberTextBox.Text = "";
            }
            else
            {
                SetBackgroundColor();
                NumberTextBox.FontSize = GetFontSize(stringToDisplay);
                NumberTextBox.Text = stringToDisplay;
            }
        }

        private void SetBackgroundColor()
        {
            int ColourIndex = (generation - 1) % BackColours.Length;
            SetBackgrounds(BackColours[ColourIndex], Colors.Black);
        }

        private void SetBackgrounds(Color BackColour, Color ForeColour)
        {
            var BackBrush = new SolidColorBrush(BackColour);
            var ForeBrush = new SolidColorBrush(ForeColour);
            NumberTextBox.Background = BackBrush;
            NumberTextBox.Foreground = ForeBrush;
            ColourBorder.Background = BackBrush;
        }

        internal Border BackgroundColourPanel
        {
            get { return this.ColourBorder; }
            set { this.ColourBorder = value; }
        }

        static double GetFontSize(string stringToDisplay)
        {
            //double fontSize = (34 - GameController.GridSize * 2);
            //double step = (4 - (double)GameController.GridSize / 8.0);
            //return fontSize - (stringToDisplay.Length * step);
            return 15.0;
        }

        internal void FlashGreen()
        {
            this.MainGrid.Background = Brushes.Lime;
        }
        internal void GoWhite()
        {
            this.MainGrid.Background = Brushes.Transparent;
        }

        internal void FlashRed()
        {
            this.MainGrid.Background = Brushes.Red;
        }

        internal void FlashBlue()
        {
            this.MainGrid.Background = Brushes.Blue;
        }

        internal Position Position
        {
            get { return new Position(row, column); }
        }

        public override string ToString()
        {
            return NumberTextBox.Text + "; " + Position.ToString();
        }
    }
}
