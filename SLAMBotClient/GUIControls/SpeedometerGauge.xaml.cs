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
    /// Interaction logic for SpeedometerGauage.xaml
    /// </summary>
    public partial class SpeedometerGauge : UserControl
    {
        public double leftPower;
        public double rightPower;
        public double speed;

        public SpeedometerGauge()
        {
            InitializeComponent();
            speed = 20;
            UpdateGUI();
        }
        public void SetLeftWheelPower(double power)
        {
            leftPower = Math.Abs(power);
            SetSpeed();
        }
        public void SetRightWheelPower(double power)
        {
            rightPower = Math.Abs(power);
            SetSpeed();
        }
        private void SetSpeed()
        {
            //Calculate Speed from two powers
            speed = 320 * ((leftPower / 2) + (rightPower / 2));
            if (speed < 20)
            {
                speed = 20;
            }
            UpdateGUI();
        }

        private void UpdateGUI()
        {
            speedometerBar.RenderTransform = new RotateTransform(speed, 1, 1);
        }
    }
}
