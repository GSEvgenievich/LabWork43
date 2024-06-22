using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Task3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new(DispatcherPriority.Render);

        int ballX = 5;
        int ballY = 5;

        Random random = new Random();


        public MainWindow()
        {
            InitializeComponent();

            int randX = random.Next(100, 300);
            int randY = random.Next(100, 300);

            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Timer_Tick; 
            timer.Start();

            Canvas.SetLeft(ball, randX);
            Canvas.SetTop(ball, randY);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            var geomerty = GetGeometry(ball);
            if (geomerty.Bounds.X + ballX < 0 || geomerty.Bounds.X + ballX > Width)
            {
                ballX = -ballX;
            }
            if (geomerty.Bounds.Y + ballY < 0 || geomerty.Bounds.Y + ballY > Height)
            {
                ballY = -ballY;
            }
            else if (CheckIntersection())
            {
                ballY = -ballY;
            }
            else if (geomerty.Bounds.Y + geomerty.Bounds.Height + ballY > Height)
            {
                timer.Stop();
                MessageBox.Show("Вы проиграли");
            }

            MoveObject(ball, geomerty.Bounds.X + ballX, geomerty.Bounds.Y + ballY);
        }

        bool CheckIntersection()
        {
            var geometryBall = GetGeometry(ball);
            var geometryRacket = GetGeometry(racket);
            IntersectionDetail intersection = geometryBall.FillContainsWithDetail(geometryRacket);
            return intersection == IntersectionDetail.Intersects;
        }

        bool CheckIntersection(Shape obgect1, Shape obgect2)
        {
            var geometryObgect1 = GetGeometry(obgect1);
            var geometryObgect2 = GetGeometry(obgect2);
            IntersectionDetail intersection = geometryObgect1.FillContainsWithDetail(geometryObgect2);
            return intersection == IntersectionDetail.Intersects;
        }

        private void MoveObject(UIElement gameObject, double x, double y)
        {
            Canvas.SetLeft(gameObject, x);
            Canvas.SetTop(gameObject, y);
        }
        

        private Geometry GetGeometry(Shape gameObject)
        {
            var geometry = gameObject.RenderedGeometry;
            geometry.Transform = (Transform)gameObject.TransformToAncestor(gameCanvas);
            return geometry;
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var speed = 50;
            var geometry = GetGeometry(racket);


            double x = 0;
            if (e.Key == Key.Left)
            {
                x = -speed;
            }
            else if (e.Key == Key.Right)
            {
                x = speed;
            }

            x = geometry.Bounds.X + x;

            if (x >= 0 && x + racket.Width <= Width)
            {
                MoveObject(racket, x, 350);
            }
        }
    }
}