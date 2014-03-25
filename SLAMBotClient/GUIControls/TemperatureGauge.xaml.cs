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
    /// Interaction logic for temperatureGuage.xaml
    /// </summary>
    public partial class TemperatureGauge : UserControl
    {
        private double temperatureValue;
        public TemperatureGauge()
        {
            InitializeComponent();
        }
        public void SetTemperatureValue(double power)
        {
            temperatureValue = power;
            UpdateGUI();
        }

        private void UpdateGUI()
        {
            // This is just to test that the temperature gauge GUI works
            if ((temperatureValue * 119) > 5)
            {
                temperatureBar.Margin = new Thickness(temperatureBar.Margin.Left, -(119 * temperatureValue), temperatureBar.Margin.Right, temperatureBar.Margin.Bottom);
                temperatureBar.Height = 119 * temperatureValue;
                temperatureLabelValue.Content = (119 * temperatureValue).ToString("#0.00 ºF");
            }
            else
            {
                temperatureBar.Margin = new Thickness(temperatureBar.Margin.Left, 0, temperatureBar.Margin.Right, temperatureBar.Margin.Bottom);
                temperatureBar.Height = 5;
                temperatureLabelValue.Content = "0.00 ºF";
            }
        }
    }
}
