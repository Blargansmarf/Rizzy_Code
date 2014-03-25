using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Threading;
using SLAMBotClient.GUIControls;

namespace SLAMBotClient.GUIControls
{
    /// <summary>
    /// Interaction logic for WheelPower.xaml
    /// </summary>
    public partial class WheelPower : UserControl
    {
        private Dictionary<int, Brush> colors;
        private double leftWheelPower;
        private double rightWheelPower;
        private DateTime lastChange;

        public WheelPower()
        {
            leftWheelPower = 0.0;
            rightWheelPower = 0.0;
            colors = new Dictionary<int, Brush>();
            colors.Add(1, Brushes.Green);
            colors.Add(2, Brushes.Green);
            colors.Add(3, Brushes.Green);
            colors.Add(4, Brushes.GreenYellow);
            colors.Add(5, Brushes.Yellow);
            colors.Add(6, Brushes.Yellow);
            colors.Add(7, Brushes.Yellow);
            colors.Add(8, Brushes.Red);
            colors.Add(9, Brushes.Red);
            colors.Add(10, Brushes.Red);

            InitializeComponent();
        }

        public void SetLeftWheelPower(double power)
        {
            leftWheelPower = power;
            UpdateGUI();
            lastChange = DateTime.Now;
        }

        public void SetRightWheelPower(double power)
        {
            rightWheelPower = power;
            UpdateGUI();
            lastChange = DateTime.Now;
        }

        private void UpdateGUI()
        {

            // If the wheel power module is hidden, then you want to show it.
            if (Visibility == System.Windows.Visibility.Hidden)
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate() { Visibility = System.Windows.Visibility.Visible; }));

            Rectangle item = new Rectangle();
            object rect;
            string leftRectName, rightRectName;

            for (int i = 10; i >= 0; i--)
            {
                leftRectName = "leftTop" + i;
                rightRectName = "rightTop" + i;

                rect = wheelPowerGrid.FindName(leftRectName);
                if ((item = rect as Rectangle) != null)
                {
                    if ((leftWheelPower * 10) >= i) item.Fill = colors[i];
                    else item.Fill = null;
                }

                rect = wheelPowerGrid.FindName(rightRectName);
                if ((item = rect as Rectangle) != null)
                {
                    if ((rightWheelPower * 10) >= i) item.Fill = colors[i];
                    else item.Fill = null;
                }
            }
            for (int i = 1; i <= 10; i++)
            {
                leftRectName = "leftBottom" + i;
                rightRectName = "rightBottom" + i;

                rect = wheelPowerGrid.FindName(leftRectName);
                if ((item = rect as Rectangle) != null)
                {
                    if ((leftWheelPower * 10) <= -i) item.Fill = colors[i];
                    else item.Fill = null;
                }

                rect = wheelPowerGrid.FindName(rightRectName);
                if ((item = rect as Rectangle) != null)
                {
                    if ((rightWheelPower * 10) <= -i) item.Fill = colors[i];
                    else item.Fill = null;
                }
            }
        }
    }
}
