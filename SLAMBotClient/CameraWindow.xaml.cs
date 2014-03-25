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

namespace SLAMBotClient
{
    /// <summary>
    /// Interaction logic for CameraWindow.xaml
    /// </summary>
    public partial class CameraWindow : Window
    {
        #region Members

        public double XForce;
        public double YForce;
        public double ZForce;
        public double Temperature;
        public event EventHandler AngleUpdated;
        CameraAngle cameraAngleControl;
        WheelPower wheelPowerControl;
        TemperatureGauge temperatureGaugeControl;
        DrivingDirection drivingDirectionControl;
        SpeedometerGauge speedometerControl;
        LineGraph temperatureLineGraph;
        LineGraph latencyLineGraph;

        //Ben - Testing purposes for line graphs
        Random randomNumber;

        private bool isHUDHidden;

        #endregion

        #region Properties

        public int CameraAngle
        {
            get
            {
                return cameraAngleControl.Angle;
            }
        }

        #endregion

        #region Constructor

        public CameraWindow()
        {
            isHUDHidden = false;
            randomNumber = new Random();
            InitializeComponent();
        }

        #endregion

        #region Public Methods

        public void LoadFrame(byte[] frame)
        {
            var bitmapImage = new BitmapImage();
            Stream stream = new MemoryStream(frame);
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            image1.Source = bitmapImage;
        }

        public void SetCameraAngle(float direction)
        {
            cameraAngleControl.SetAngle(direction);
        }

        public void SetLeftWheelPower(double power)
        {
            wheelPowerControl.SetLeftWheelPower(power);
            speedometerControl.SetLeftWheelPower(power);
            drivingDirectionControl.SetLeftWheelPower(power);
        }

        public void SetRightWheelPower(double power)
        {
            wheelPowerControl.SetRightWheelPower(power);
            speedometerControl.SetRightWheelPower(power);
            drivingDirectionControl.SetRightWheelPower(power);
        }

        public void UpdateLineGraphs()
        {
            temperatureLineGraph.AddPoint((double)randomNumber.Next(-12, 12));
            latencyLineGraph.AddPoint((double)randomNumber.Next(-12, 12));
        }

        public void SetTemperatureValue(double temperature)
        {
            temperatureGaugeControl.SetTemperatureValue(temperature);
        }

        #endregion

        #region Events

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                WindowStyle = System.Windows.WindowStyle.None;
                WindowState = System.Windows.WindowState.Maximized;
            }
            else if (e.Key == Key.Escape)
            {
                WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
                WindowState = System.Windows.WindowState.Normal;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            image1.Height = Height;
            image1.Width = Width;

            wheelPowerControl = new WheelPower();
            canvasWheelPower.Children.Add(wheelPowerControl);
            //canvasWheelPower.Margin = new Thickness(Width - 120, 0, 0, 0);
            
            cameraAngleControl = new CameraAngle();
            cameraAngleControl.AngleUpdated += new EventHandler(cameraAngleControl_AngleUpdated);
            canvasCameraAngle.Children.Add(cameraAngleControl);


            temperatureGaugeControl = new TemperatureGauge();
            canvasTemperature.Children.Add(temperatureGaugeControl);

            drivingDirectionControl = new DrivingDirection();
            canvasDrivingDirection.Children.Add(drivingDirectionControl);

            speedometerControl = new SpeedometerGauge();
            canvasSpeedometerGauge.Children.Add(speedometerControl);

            temperatureLineGraph = new LineGraph(canvasTempLineGraph, Brushes.Green);
            latencyLineGraph = new LineGraph(canvasLatencyLineGraph, Brushes.Red);

            canvasTempLineGraph.Children.Add(temperatureLineGraph);
            canvasLatencyLineGraph.Children.Add(latencyLineGraph);
            
            //canvasCameraAngle.Margin = new Thickness(Width - 120, 10, 0, 0);
        }

        void cameraAngleControl_AngleUpdated(object sender, EventArgs e)
        {
            if (AngleUpdated != null)
                AngleUpdated(this, EventArgs.Empty);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            image1.Height = Height;
            image1.Width = Width;
            //canvasCameraAngle.Margin = new Thickness(Width - 120, 10, 0, 0);canvasTemperature

        }

        private void hideHudLabel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!isHUDHidden)
            {

                bottomCanvas.Visibility = rightCanvas.Visibility = Visibility.Hidden;
                hideHudLabel.Content = "Show HUD";
            }
            else
            {
                bottomCanvas.Visibility = rightCanvas.Visibility = Visibility.Visible;
                hideHudLabel.Content = "Hide HUD";
            }

            isHUDHidden = !isHUDHidden;

        }

        #endregion     

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }
    }
}
