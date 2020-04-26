using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TopoGiraffe.Noyau
{
    class PointAltitude
    {
        private Point point;
        private double altitude;
        private Ellipse cercle;
        private Polygon triangle;


        public PointAltitude(Point aPoint, double anAltitude)
        {
            point = aPoint;
            altitude = anAltitude;

        }


        public void DrawPointCote(Canvas canvas)
        {
            cercle = new Ellipse();
            cercle = new Ellipse();
            cercle.Stroke = System.Windows.Media.Brushes.Black;
            cercle.Fill = System.Windows.Media.Brushes.DarkBlue;
            cercle.HorizontalAlignment = HorizontalAlignment.Left;
            cercle.VerticalAlignment = VerticalAlignment.Center;
            cercle.Width = 10;

            cercle.Height = 10;

            Canvas.SetLeft(cercle, point.X - (cercle.Width / 2));
            Canvas.SetTop(cercle, point.Y - (cercle.Height / 2));

            canvas.Children.Add(cercle);

        }

        public void DrawPointSommet(Canvas canvas)
        {
            cercle = new Ellipse();
            cercle = new Ellipse();
            cercle.Stroke = System.Windows.Media.Brushes.Black;
            cercle.Fill = System.Windows.Media.Brushes.DarkBlue;
            cercle.HorizontalAlignment = HorizontalAlignment.Left;
            cercle.VerticalAlignment = VerticalAlignment.Center;
            cercle.Width = 50;
            cercle.Height = 50;

            Canvas.SetLeft(cercle, point.X - (cercle.Width / 2));
            Canvas.SetTop(cercle, point.Y - (cercle.Height / 2));


            canvas.Children.Add(cercle);

        }

        public void makeTriangle(Canvas canvas)
        {
            triangle = new Polygon();
            triangle.Points.Add(new Point(point.X - 5, point.Y + 5));
            triangle.Points.Add(new Point(point.X + 5, point.Y + 5));
            triangle.Points.Add(new Point(point.X, point.Y - 5));


            triangle.Fill = Brushes.Black;
            triangle.Width = 15;
            triangle.Height = 15;
            triangle.Stretch = Stretch.Fill;
            



            
            Canvas.SetLeft(triangle, point.X - (triangle.Width / 2));
            Canvas.SetTop(triangle, point.Y - (triangle.Height / 2));



            canvas.Children.Add(triangle);



        }







    }




}
