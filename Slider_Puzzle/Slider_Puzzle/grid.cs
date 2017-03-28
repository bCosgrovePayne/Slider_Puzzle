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
            ContentView contentView = new ContentView
            {
                Content = _absoluteLayout
            };
            contentView.SizeChanged += OnContentViewSizeChanged;
            this.Padding = new Thickness(5, Device.OnPlatform(25, 5, 5), 5, 5);
            this.Content = contentView;
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
            GridItem item = (GridItem)sender;
            Random rand = new Random();
            int move;
            int row;
            int col;

            // 0, 1, 2, 3 = up right down left
            //adjust for corners and edges
            if (item.Position.Row == 0)
            {
                if (item.Position.Column == 0)
                {
                    move = rand.Next(0, 1);
                    if (move == 0)
                    {
                        move = 2;
                    }
                }
                else if (item.Position.Column == 3)
                {
                    move = rand.Next(2, 3);
                }
                else
                {
                    move = rand.Next(1, 3);
                }
            }
            else if (item.Position.Row == 3)
            {
                if (item.Position.Column == 0)
                {
                    move = rand.Next(0, 1);
                }
                else if (item.Position.Column == 3)
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
            else if (item.Position.Column == 0)
            {
                move = rand.Next(0, 2);
            }
            else if (item.Position.Column == 3)
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
                row = item.Position.Row - 1;
                col = item.Position.Column;
            }
            else if (move == 1)
            {
                row = item.Position.Row;
                col = item.Position.Column + 1;
            }
            else if (move == 2)
            {
                row = item.Position.Row + 1;
                col = item.Position.Column;
            }
            else
            {
                row = item.Position.Row;
                col = item.Position.Column - 1;
            }

            GridItem swapWith = _gridItems[new GridPosition(row, col)];
            Swap(item, swapWith);
            OnContentViewSizeChanged(this.Content, null);
        }

        void Swap(GridItem item1, GridItem item2)
        {
            GridPosition temp = item1.Position;
            item1.Position = item2.Position;
            item2.Position = temp;

            _gridItems[item1.Position] = item1;
            _gridItems[item2.Position] = item2;
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
            public GridItem(GridPosition position, String text)
            {
                Position = position;
                Source = text;
            }
        }
    }    
}
