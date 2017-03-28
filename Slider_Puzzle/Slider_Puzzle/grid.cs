using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace Slider_Puzzle
{
    public class grid : ContentPage
    {
        private Dictionary<GridPosition, GridItem> _gridItems;
        private string solved;
        private AbsoluteLayout _absoluteLayout;
        public grid()
        {
            int squareSize = 75;
            _gridItems = new Dictionary<GridPosition, GridItem>();
            _absoluteLayout = new AbsoluteLayout
            {
                BackgroundColor = Color.FromRgb(0, 0, 255),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            int counter = 1;
            string source = "";
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    counter++;
                    source = counter.ToString() + ".jpeg";
                    GridItem item = new GridItem(new GridPosition(row, col), source);

                    var tapRecognizer = new TapGestureRecognizer();
                    tapRecognizer.Tapped += OnLabelTapped;
                    item.GestureRecognizers.Add(tapRecognizer);

                    _gridItems.Add(item.Position, item);
                    Rectangle rect = new Rectangle(col * squareSize, row * squareSize, squareSize, squareSize);
                    _absoluteLayout.Children.Add(item, rect);
                }
            }
            solved = _gridItems.ToString();
                          
            ContentView contentView = new ContentView
            {
                Content = _absoluteLayout
            };
            contentView.SizeChanged += OnContentViewSizeChanged;
            this.Padding = new Thickness(5, Device.OnPlatform(25, 5, 5), 5, 5);
            this.Content = contentView;

            GridPosition emptySquare = new GridPosition(3, 3);
            for (int i = 0; i < 100000; i++)
            {
                // 0, 1, 2, 3 = up right down left
                //adjust for corners and edges
                Random rand = new Random();
                int move;
                if (emptySquare.Row == 0)
                {
                    if (emptySquare.Column == 0)
                    {
                        move = rand.Next(0, 1);
                        if (move == 0)
                        {
                            move = 2;
                        }
                    }
                    else if (emptySquare.Column == 3)
                    {
                        move = rand.Next(2, 3);
                    }
                    else
                    {
                        move = rand.Next(1, 3);
                    }
                }
                else if (emptySquare.Row == 3)
                {
                    if (emptySquare.Column == 0)
                    {
                        move = rand.Next(0, 1);
                    }
                    else if (emptySquare.Column == 3)
                    {
                        move = rand.Next(0, 1);
                        if (move == 1)
                        {
                            move = 3;
                        }
                    }
                    else
                    {
                        move = rand.Next(0, 3);
                        if (move == 2)
                        {
                            move = 3;
                        }
                    }
                }
                else if (emptySquare.Column == 0)
                {
                    move = rand.Next(0, 2);
                }
                else if (emptySquare.Column == 3)
                {
                    move = rand.Next(0, 2);
                    if (move == 1)
                    {
                        move = 3;
                    }
                }
                else
                {
                    move = rand.Next(0, 3);
                }

                if (move == 0)
                {
                    Scrambler(_gridItems[new GridPosition(emptySquare.Row - 1, emptySquare.Column)]);
                    emptySquare.Row = emptySquare.Row - 1;
                }
                else if (move == 1)
                {
                    Scrambler(_gridItems[new GridPosition(emptySquare.Row, emptySquare.Column + 1)]);
                    emptySquare.Column = emptySquare.Column + 1;
                }
                else if (move == 2)
                {
                    Scrambler(_gridItems[new GridPosition(emptySquare.Row + 1, emptySquare.Column)]);
                    emptySquare.Row = emptySquare.Row + 1;
                }
                else
                {
                    Scrambler(_gridItems[new GridPosition(emptySquare.Row, emptySquare.Column - 1)]);
                    emptySquare.Column = emptySquare.Column - 1;
                }
            }
        }
        void OnContentViewSizeChanged(object sender, EventArgs args)
        {
            ContentView contentView = (ContentView)sender;
            double squareSize = Math.Min(contentView.Width, contentView.Height) / 4;

            for (var row = 0; row < 4; row++)
            {
                for (var col = 0; col < 4; col++)
                {
                    GridItem item = _gridItems[new GridPosition(row, col)];
                    Rectangle rect = new Rectangle(col * squareSize, row * squareSize, squareSize, squareSize);
                    AbsoluteLayout.SetLayoutBounds(item, rect); 
                }
            }
        }
        void OnLabelTapped(object sender, EventArgs args)
        {
            Scrambler((GridItem)sender);            
        }

        void Scrambler(GridItem item)
        {
            Random rand = new Random();
            int row = item.Position.Row;
            int col = item.Position.Column;
            bool swapable = false;
            GridPosition correctSquare = new GridPosition(row, col);
            if (row - 1 > -1)
            {
                GridPosition adjacentSquare = new GridPosition(row - 1, col);

                if (_gridItems[adjacentSquare].Text == "17.jpeg")
                {
                    correctSquare = adjacentSquare;
                    swapable = true;
                }
            }
            if (row + 1 < 4)
            {
                GridPosition adjacentSquare = new GridPosition(row + 1, col);

                if (_gridItems[adjacentSquare].Text == "17.jpeg")
                {
                    correctSquare = adjacentSquare;
                    swapable = true;
                }
            }
            if (col - 1 > -1)
            {
                GridPosition adjacentSquare = new GridPosition(row, col - 1);

                if (_gridItems[adjacentSquare].Text == "17.jpeg")
                {
                    correctSquare = adjacentSquare;
                    swapable = true;
                }
            }
            if (col + 1 < 4)
            {
                GridPosition adjacentSquare = new GridPosition(row, col + 1);

                if (_gridItems[adjacentSquare].Text == "17.jpeg")
                {
                    correctSquare = adjacentSquare;
                    swapable = true;
                }
            }            
            if (swapable == true)
            {
                Swap(item, _gridItems[correctSquare]);
            }
                      
            OnContentViewSizeChanged(this.Content, null);
        }

        void Swap(GridItem item1, GridItem item2)
        {
            GridPosition temp = item1.Position;
            item1.Position = item2.Position;
            item2.Position = temp;

            _gridItems[item1.Position] = item1;
            _gridItems[item2.Position] = item2;

            //if (solved == _gridItems.ToString())
            //{
            //    winner item = new winner(new GridPosition(3, 3));
            //    Rectangle rect = new Rectangle(300, 300, 75, 75);
            //    _absoluteLayout.Children.Add(item, rect);
            //    ContentView contentView = new ContentView
            //    {
            //        Content = _absoluteLayout
            //    };
            //    contentView.SizeChanged += OnContentViewSizeChanged;
            //    this.Padding = new Thickness(5, Device.OnPlatform(25, 5, 5), 5, 5);
            //    this.Content = contentView;
            //}
        }
        class GridPosition
        {
            public int Row
            {
                get; set;
            }
            public int Column
            {
                get; set;
            }
            public GridPosition(int row, int col)
            {
                Row = row;
                Column = col;
            }
            public override bool Equals(object obj)
            {
                GridPosition other = obj as GridPosition;

                if (other != null && this.Row == other.Row && this.Column == other.Column)
                {
                    return true;
                }
                return false;
            }
            public override int GetHashCode()
            {
                return 17 * 23 + this.Row.GetHashCode() * 23 + this.Column.GetHashCode();
            }
        }
        class GridItem : Image
        {
            public GridPosition Position
            {
                get; set;
            }
            public String Text
            {
                get; set;
            }
            public GridItem(GridPosition position, String text)
            {
                Position = position;
                Source = text;
                Text = text;
            }
        }

        class winner : Label
        {
            public GridPosition Position
            {
                get; set;
            }
            public winner(GridPosition position)
            {
                Position = position;
                Text = "Winner";
                TextColor = Color.White;
            }
        }
    }    
}
