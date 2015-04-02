using System.Windows;
using System.Windows.Media;
using Microsoft.Multipoint.Sdk;
using Microsoft.Multipoint.Sdk.Controls;
using System.Collections.Generic;
using Microsoft.Multipoint.Sdk.Samples.Common;
using System.Windows.Input;
using System;
using System.Windows.Threading;

namespace ColorPlacingGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMultipointMouseEvents
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            KeyDown += new System.Windows.Input.KeyEventHandler(MainWindow_KeyDown);

            board1.RowCount = Properties.Settings.Default.MaxRow;
            board1.ColumnCount = Properties.Settings.Default.MaxColumn;
            board1.FillButtons();

            board2.RowCount = Properties.Settings.Default.MaxRow;
            board2.ColumnCount = Properties.Settings.Default.MaxColumn;
            board2.FillButtons();

            InitTimer();
        }
        #region Timer

        DispatcherTimer timer = new DispatcherTimer();
        TimeSpan elapsedTime;

        private void InitTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += new EventHandler(timer_Tick);
            elapsedTime = new TimeSpan(0);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Add(new TimeSpan(0, 0, 1));
            timeText.Text = elapsedTime.TotalSeconds.ToString("00:00");
        }

        #endregion

        #region Event Handlers

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MultipointSdk.Instance.Register(this);
            MultipointMouseEvents.AddMultipointMouseMoveHandler(this, this.Multipoint_MouseMove);

            MultipointSdk.Instance.DeviceArrivalEvent += new System.EventHandler<DeviceNotifyEventArgs>(Instance_DeviceArrivalEvent);
            MultipointSdk.Instance.DeviceRemoveCompleteEvent += new System.EventHandler<DeviceNotifyEventArgs>(Instance_DeviceRemoveCompleteEvent);

            foreach (DeviceInfo di in MultipointSdk.Instance.MouseDeviceList)
                CursorAssignments.Instance.AssignCursorToMouse(di);

            SetChoosingState();

            Random rand = new Random();
            int initRowCount;
            if (Properties.Settings.Default.InitRowCount > Properties.Settings.Default.MaxRow)
                initRowCount = Properties.Settings.Default.MaxRow - 1;
            else
                initRowCount = Properties.Settings.Default.InitRowCount;
            bool cont = true;
            while (cont)
            {
                for (int i = 0; i < board1.ColumnCount; i++)
                {
                    int top = 0;
                    while (top < initRowCount && board1.GetButton(top, i).Background != board1.DefaultColor) top++;
                    while (top < initRowCount)
                    {
                        int x = rand.Next(4);
                        switch (x)
                        {
                            case 0: board1.GetButton(top, i).Background = Brushes.Red; break;
                            case 1: board1.GetButton(top, i).Background = Brushes.Green; break;
                            case 2: board1.GetButton(top, i).Background = Brushes.Blue; break;
                            case 3: board1.GetButton(top, i).Background = Brushes.Yellow; break;
                        }
                        top++;
                    }
                }
                board1.RefreshAllBoard();

                cont = false;
                for (int i = 0; i < board1.ColumnCount; i++)
                    if (!cont)
                    {
                        int top = Properties.Settings.Default.InitRowCount - 1;
                        if (board1.GetButton(top, i).Background == board1.DefaultColor)
                            cont = true;
                    }
            }

            for (int i = 0; i < Properties.Settings.Default.InitRowCount; i++)
                for (int j = 0; j < board2.ColumnCount; j++)
                    board2.GetButton(i, j).Background = board1.GetButton(i, j).Background;
        }

        void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape) this.Close();
            if (e.Key == Key.S && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                SetChoosingState();
            if (e.Key == Key.F5)
            {
                if (colorManVisual1 != null) colorManVisual1.DisableMouse = false;
                if (colorManVisual2 != null) colorManVisual2.DisableMouse = false;
                if (placingManVisual1 != null) placingManVisual1.DisableMouse = false;
                if (placingManVisual2 != null) placingManVisual2.DisableMouse = false;
                elapsedTime = new TimeSpan(0, 0, 0);
                timer.Start();
            }
        }

        void Instance_DeviceRemoveCompleteEvent(object sender, DeviceNotifyEventArgs e)
        {
            CursorAssignments.Instance.RemoveCursorFromMouse(e.DeviceInfo);
        }

        void Instance_DeviceArrivalEvent(object sender, DeviceNotifyEventArgs e)
        {
            DeviceInfo di = e.DeviceInfo;

            if (di.DeviceId == colorManId1 || di.DeviceId == colorManId2)
            {
                CursorAssignments.Instance.AssignCursorToMouse(di);
                di.DeviceVisual.SetPosition(stackColors.PointToScreen(new Point(0, 0)));
                return;
            }

            if (di.DeviceId == placingManId1)
            {
                placingManVisual1 = di.DeviceVisual;
                placingManVisual1.CursorBitmap = Cursors.Resources.none_cursor;
                placingManVisual1.SetPosition(board1.PointToScreen(new Point(0, 0)));
                return;
            }

            if (di.DeviceId == placingManId2)
            {
                placingManVisual2 = di.DeviceVisual;
                placingManVisual2.CursorBitmap = Cursors.Resources.none_cursor;
                placingManVisual2.SetPosition(board1.PointToScreen(new Point(0, 0)));
                return;
            }
            //else all
            {
                Point point = dockWaiting.PointToScreen(new Point(0, 0));
                Rect rect = new Rect(point.X, point.Y, dockWaiting.ActualWidth, dockWaiting.ActualHeight);
                rect.Inflate(-3, -3);

                if (!RegionList.ContainsKey(di.DeviceId))
                    RegionList.Add(di.DeviceId, rect);
                else
                    RegionList[di.DeviceId] = rect;

                CursorAssignments.Instance.AssignCursorToMouse(di);
            }
        }

        private void Multipoint_MouseMove(object sender, RoutedEventArgs e)
        {
            var args = e as MultipointMouseEventArgs;
            if (args == null) return;

            if (RegionList.ContainsKey(args.DeviceInfo.DeviceId))
            {
                var screenPosition = args.DeviceInfo.DeviceVisual.Position;
                Rect rect = RegionList[args.DeviceInfo.DeviceId];

                if (screenPosition.X < rect.Left)
                    screenPosition.X = rect.Left;
                if (screenPosition.X > rect.Right)
                    screenPosition.X = rect.Right;
                if (screenPosition.Y < rect.Top)
                    screenPosition.Y = rect.Top;
                if (screenPosition.Y > rect.Bottom)
                    screenPosition.Y = rect.Bottom;

                args.DeviceInfo.DeviceVisual.SetPosition(screenPosition);
            }
        }
        #endregion


        #region Fields

        Dictionary<string, Rect> RegionList;
        string colorManId1, colorManId2, placingManId1, placingManId2;
        MultipointMouseDevice placingManVisual1, placingManVisual2;
        MultipointMouseDevice colorManVisual1, colorManVisual2;

        #endregion

        #region Private Methods

        private void SetChoosingState()
        {
            RegionList = new Dictionary<string, Rect>();
            foreach (DeviceInfo di in MultipointSdk.Instance.MouseDeviceList)
            {
                di.DeviceVisual.DisableMouse = false;
                Point point = dockWaiting.PointToScreen(new Point(0, 0));
                Rect rect = new Rect(point.X, point.Y, dockWaiting.ActualWidth, dockWaiting.ActualHeight);
                rect.Inflate(-3, -3);
                RegionList.Add(di.DeviceId, rect);
                di.DeviceVisual.SetPosition(point);
                if (di.DeviceId == placingManId1 || di.DeviceId == placingManId2)
                {
                    CursorAssignments.Instance.RemoveCursorFromMouse(di);
                    CursorAssignments.Instance.AssignCursorToMouse(di);
                }
            }

            colorManId1 = string.Empty;
            colorManId2 = string.Empty;
            placingManId1 = string.Empty;
            placingManId2 = string.Empty;

            btnTeam1.IsEnabled = true;
            btnTeam2.IsEnabled = true;

            timer.Stop();
        }

        private void btnTeam_MultipointClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as MultipointButton;
            if (btn == null) return;

            var args = e as MultipointMouseEventArgs;
            if (args == null) return;

            switch (btn.Name)
            {
                case "btnTeam1":
                    {
                        if (string.IsNullOrEmpty(colorManId1))
                        {
                            string id = args.DeviceInfo.DeviceId;
                            colorManId1 = id;

                            Point point = stackColors.PointToScreen(new Point(0, 0));
                            Rect rect = new Rect(point.X, point.Y, stackColors.ActualWidth, stackColors.ActualHeight);
                            rect.Inflate(-3, -3);

                            if (!RegionList.ContainsKey(id))
                                RegionList.Add(id, rect);
                            else
                                RegionList[id] = rect;

                            colorManVisual1 = args.DeviceInfo.DeviceVisual;
                            colorManVisual1.SetPosition(point);
                            colorManVisual1.DisableMouse = true;
                        }
                        else
                            if (string.IsNullOrEmpty(placingManId1))
                            {
                                string id = args.DeviceInfo.DeviceId;
                                placingManId1 = id;

                                Point point = board1.PointToScreen(new Point(0, 0));
                                Rect rect = new Rect(point.X, point.Y, board1.ActualWidth, board1.ActualHeight);
                                rect.Inflate(-3, -3);

                                if (!RegionList.ContainsKey(id))
                                    RegionList.Add(id, rect);
                                else
                                    RegionList[id] = rect;

                                btn.IsEnabled = false;

                                placingManVisual1 = args.DeviceInfo.DeviceVisual;
                                placingManVisual1.CursorBitmap = Cursors.Resources.none_cursor;
                                placingManVisual1.SetPosition(point);
                                placingManVisual1.DisableMouse = true;
                            }
                    } break;
                case "btnTeam2":
                    {
                        if (string.IsNullOrEmpty(colorManId2))
                        {
                            string id = args.DeviceInfo.DeviceId;
                            colorManId2 = id;

                            Point point = stackColors.PointToScreen(new Point(0, 0));
                            Rect rect = new Rect(point.X, point.Y, stackColors.ActualWidth, stackColors.ActualHeight);
                            rect.Inflate(-3, -3);

                            if (!RegionList.ContainsKey(id))
                                RegionList.Add(id, rect);
                            else
                                RegionList[id] = rect;

                            colorManVisual2 = args.DeviceInfo.DeviceVisual;
                            colorManVisual2.SetPosition(point);
                            colorManVisual2.DisableMouse = true;
                        }
                        else
                            if (string.IsNullOrEmpty(placingManId1))
                            {
                                string id = args.DeviceInfo.DeviceId;
                                placingManId2 = id;

                                Point point = board2.PointToScreen(new Point(0, 0));
                                Rect rect = new Rect(point.X, point.Y, board1.ActualWidth, board1.ActualHeight);
                                rect.Inflate(-3, -3);

                                if (!RegionList.ContainsKey(id))
                                    RegionList.Add(id, rect);
                                else
                                    RegionList[id] = rect;

                                btn.IsEnabled = false;

                                placingManVisual2 = args.DeviceInfo.DeviceVisual;
                                placingManVisual2.CursorBitmap = Cursors.Resources.none_cursor;
                                placingManVisual2.SetPosition(point);
                                placingManVisual2.DisableMouse = true;
                            }
                    } break;
            }
        }

        private void ColorButton_MultipointClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as MultipointButton;
            if (btn == null) return;

            var args = e as MultipointMouseEventArgs;
            if (args == null) return;

            if (args.DeviceInfo.DeviceId == colorManId1
                && (!string.IsNullOrEmpty(placingManId1))
                && (placingManVisual1 != null))
            {
                if (board1.CursorColors.ContainsKey(placingManId1))
                    board1.CursorColors[placingManId1] = btn.Background;
                else
                    board1.CursorColors.Add(placingManId1, btn.Background);

                switch (btn.Name)
                {
                    case "redColor": placingManVisual1.CursorBitmap = Cursors.Resources.red_cursor; break;
                    case "greenColor": placingManVisual1.CursorBitmap = Cursors.Resources.green_cursor; break;
                    case "blueColor": placingManVisual1.CursorBitmap = Cursors.Resources.blue_cursor; break;
                    case "yellowColor": placingManVisual1.CursorBitmap = Cursors.Resources.yellow_cursor; break;
                }
            }

            if (args.DeviceInfo.DeviceId == colorManId2
                && (!string.IsNullOrEmpty(placingManId2))
                && (placingManVisual2 != null))
            {
                if (board2.CursorColors.ContainsKey(placingManId2))
                    board2.CursorColors[placingManId2] = btn.Background;
                else
                    board2.CursorColors.Add(placingManId2, btn.Background);

                switch (btn.Name)
                {
                    case "redColor": placingManVisual2.CursorBitmap = Cursors.Resources.red_cursor; break;
                    case "greenColor": placingManVisual2.CursorBitmap = Cursors.Resources.green_cursor; break;
                    case "blueColor": placingManVisual2.CursorBitmap = Cursors.Resources.blue_cursor; break;
                    case "yellowColor": placingManVisual2.CursorBitmap = Cursors.Resources.yellow_cursor; break;
                }
            }
        }

        #endregion


        #region IMultipointMouseEvents Members

        public event RoutedEventHandler MultipointMouseDownEvent;

        public event RoutedEventHandler MultipointMouseEnterEvent;

        public event RoutedEventHandler MultipointMouseLeaveEvent;

        public event RoutedEventHandler MultipointMouseMoveEvent;

        public event RoutedEventHandler MultipointMouseUpEvent;

        public event RoutedEventHandler MultipointMouseWheelEvent;

        public event RoutedEventHandler MultipointPreviewMouseDownEvent;

        public event RoutedEventHandler MultipointPreviewMouseMoveEvent;

        public event RoutedEventHandler MultipointPreviewMouseUpEvent;

        public event RoutedEventHandler MultipointPreviewMouseWheelEvent;

        #endregion


    }
}
