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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SLAMBotClient.GUIControls
{
    /// <summary>
    /// Interaction logic for DrivingDirection.xaml
    /// </summary>
    public partial class DrivingDirection : UserControl
    {
        public double leftPower;
        public double rightPower;
        public double direction;

        public DrivingDirection()
        {
            InitializeComponent();
            leftPower = rightPower = direction = 0;
            SetDirection();
        }
        public void SetLeftWheelPower(double power)
        {
            leftPower = power;
            SetDirection();
        }
        public void SetRightWheelPower(double power)
        {
            rightPower = power;
            SetDirection();
        }
        private void SetDirection()
        {
            if (leftPower > 0 && rightPower > 0) { direction = 180; }
            else if (leftPower < 0 && rightPower < 0) { direction = 0; }
            else if (leftPower == 0 && rightPower != 0) { direction = (rightPower > 0) ? 180 : 0; }
            else if (leftPower != 0 && rightPower == 0) { direction = (leftPower > 0) ? 180 : 0; }
            else if (leftPower > 0 && rightPower < 0) { direction = (Math.Abs(leftPower) > Math.Abs(rightPower)) ? 180 : 0; }
            else if (leftPower < 0 && rightPower > 0) { direction = (Math.Abs(leftPower) > Math.Abs(rightPower)) ? 0 : 180; }
            else { direction = 180; }

            direction -= 90 * rightPower;
            direction += 90 * leftPower;
            
            UpdateGUI();
        }

        private void UpdateGUI()
        {
            directionBar.RenderTransform = new RotateTransform(direction, 1, 1);
        }
    }
}
