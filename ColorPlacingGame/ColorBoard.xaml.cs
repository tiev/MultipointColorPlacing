using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Multipoint.Sdk;
using Microsoft.Multipoint.Sdk.Controls;

namespace ColorPlacingGame
{
    /// <summary>
    /// Interaction logic for ColorBoard.xaml
    /// </summary>
    public partial class ColorBoard : UserControl
    {
        #region Properties


        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register("RowCount", typeof(int), typeof(ColorBoard), new UIPropertyMetadata(0));


        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.Register("ColumnCount", typeof(int), typeof(ColorBoard), new UIPropertyMetadata(0));



        public Brush DefaultColor
        {
            get { return (Brush)GetValue(DefaultColorProperty); }
            set { SetValue(DefaultColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultColorProperty =
            DependencyProperty.Register("DefaultColor", typeof(Brush), typeof(ColorBoard), new UIPropertyMetadata((Brushes.WhiteSmoke)));


        public int MaxPath
        {
            get { return (int)GetValue(MaxPathProperty); }
            set { SetValue(MaxPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxPathProperty =
            DependencyProperty.Register("MaxPath", typeof(int), typeof(ColorBoard), new UIPropertyMetadata(int.MaxValue));


        public Dictionary<string, Brush> CursorColors { get; set; }

        #endregion


        public ColorBoard()
        {
            InitializeComponent();
            CursorColors = new Dictionary<string, Brush>();
        }

        public void FillButtons()
        {
            for (int i = 0; i < RowCount; i++)
                BoardGrid.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < ColumnCount; i++)
                BoardGrid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0; i < RowCount; i++)
                for (int j = 0; j < ColumnCount; j++)
                {
                    MultipointButton mb = new MultipointButton();
                    mb.SetCurrentValue(Grid.RowProperty, i);
                    mb.SetCurrentValue(Grid.ColumnProperty, j);
                    mb.BorderThickness = new Thickness(2);
                    mb.BorderBrush = Brushes.Blue;
                    mb.Background = DefaultColor;
                    mb.Name = "b" + i.ToString() + "_" + j.ToString();
                    BoardGrid.RegisterName("b" + i.ToString() + "_" + j.ToString(), mb);
                    //mb.Visibility = Visibility.Hidden;
                    mb.MultipointClick += new RoutedEventHandler(mb_MultipointClick);
                    BoardGrid.Children.Add(mb);
                }
        }

        void mb_MultipointClick(object sender, RoutedEventArgs e)
        {
            var mb = sender as MultipointButton;
            var args = e as MultipointMouseEventArgs;
            if (mb == null || args == null)
                return;
            if (mb.Background != DefaultColor) return;

            //#region test
            //mb.Background = Brushes.Red;
            //RefreshBoard((int)mb.GetValue(Grid.RowProperty), (int)mb.GetValue(Grid.ColumnProperty));
            //#endregion

            string id = args.DeviceInfo.DeviceId;
            if (!CursorColors.ContainsKey(id)) return;

            if (CursorColors[id] != null)
            {
                int x = (int)mb.GetValue(Grid.RowProperty);
                int y = (int)mb.GetValue(Grid.ColumnProperty);
                if (x == 0 || GetButton(x - 1, y).Background != DefaultColor)
                {
                    mb.Background = (Brush)CursorColors[id];
                    RefreshBoard(x, y);
                }
                while (RefreshAllBoard()) ;
            }
        }

        List<MultipointButton> path;
        public bool RefreshBoard(int x, int y)
        {
            bool result = false;
            path = new List<MultipointButton>();
            var btn = GetButton(x, y);

            // Do nothing when this is empty button
            if (btn.Background == DefaultColor) return false;

            if (btn != null)
            {
                path.Add(btn);
                TracePath(x, y, btn.Background);
                if (path.Count > MaxPath)
                {
                    result = true;
                    foreach (MultipointButton mb in path)
                        mb.Background = DefaultColor;
                }
            }
            StackAll();
            return result;
        }

        public bool RefreshAllBoard()
        {
            bool result = false;
            for (int i = RowCount - 1; i >= 0; i--)
                for (int j = 0; j < ColumnCount; j++)
                    if (RefreshBoard(i, j))
                        result = true;
            return result;
        }

        public void StackAll()
        {
            for (int i = 0; i < ColumnCount; i++)
                Stack(i);
        }

        public void Stack(int i)
        {
            int top = RowCount - 1;
            int bottom = 0;
            int mid;

            while (top >= 0 && GetButton(top, i).Background == DefaultColor) top--;
            if (top < 0) return;

            while (bottom < RowCount && GetButton(bottom, i).Background != DefaultColor) bottom++;
            if (bottom >= RowCount) return;

            mid = bottom + 1;
            while (bottom < top)
            {
                MultipointButton midBtn = GetButton(mid, i);
                while (mid <= top && midBtn.Background == DefaultColor)
                {
                    mid++;
                    midBtn = GetButton(mid, i);
                }
                if (mid <= top)
                {
                    GetButton(bottom, i).Background = midBtn.Background;
                    midBtn.Background = DefaultColor;
                }
                bottom++;
                mid++;
            }
        }

        private void TracePath(int x, int y, Brush color)
        {
            // up
            if (x > 0)
            {
                int xNext = x - 1;
                int yNext = y;
                var btn = GetButton(xNext, yNext);
                if (btn != null)
                    if ((!path.Contains(btn)) && btn.Background == color)
                    {
                        path.Add(btn);
                        TracePath(xNext, yNext, color);
                    }
            }
            // right
            if (y < ColumnCount - 1)
            {
                int xNext = x;
                int yNext = y + 1;
                var btn = GetButton(xNext, yNext);
                if (btn != null)
                    if ((!path.Contains(btn)) && btn.Background == color)
                    {
                        path.Add(btn);
                        TracePath(xNext, yNext, color);
                    }
            }
            // down
            if (x < RowCount - 1)
            {
                int xNext = x + 1;
                int yNext = y;
                var btn = GetButton(xNext, yNext);
                if (btn != null)
                    if ((!path.Contains(btn)) && btn.Background == color)
                    {
                        path.Add(btn);
                        TracePath(xNext, yNext, color);
                    }
            }
            // left
            if (y > 0)
            {
                int xNext = x;
                int yNext = y - 1;
                var btn = GetButton(xNext, yNext);
                if (btn != null)
                    if ((!path.Contains(btn)) && btn.Background == color)
                    {
                        path.Add(btn);
                        TracePath(xNext, yNext, color);
                    }
            }
        }

        public MultipointButton GetButton(int x, int y)
        {
            var btn = BoardGrid.FindName("b" + x.ToString() + "_" + y.ToString()) as MultipointButton;
            return btn;
        }

        public bool SetColor(int x, int y, Brush color)
        {
            var btn = BoardGrid.FindName("b" + x.ToString() + "_" + y.ToString());
            if (btn != null)
            {
                ((MultipointButton)btn).Background = color;
                return true;
            }
            return false;
        }


    }
}
