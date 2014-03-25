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
    /// Interaction logic for LineGraph.xaml
    /// </summary>
    public partial class LineGraph : UserControl
    {
        public Polyline line;
        private PointCollection lineCollection;
        private double left;

        public LineGraph(Canvas lineGraphCanvas, Brush color)
        {
            InitializeComponent();
            left = 0;
            lineCollection = new PointCollection();
            line = new Polyline()
            {
                Stroke = color,
                StrokeThickness = 1,
                Margin = new Thickness(0, 15, 0, 0),
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
            };
            //Add to the canvas
            lineGraphCanvas.Children.Add(line);
        }

        public void AddPoint(double pointValue)
        {
            line.Points.Add(new Point(left, pointValue));
            left += 2;

            // I'm not sure if this is the best method of making the line graph
            // What if it exceeds the maximum marginLeft?  Another possiblilty
            // would to store the points in an array, and not have to 
            // shift it to the left... Hmmm.. Ricky? Any suggestions?
            if (left > 530)
            {
                line.Margin = new Thickness((line.Margin.Left - 2), 15, 0, 0);
            }
        }
    }
}
