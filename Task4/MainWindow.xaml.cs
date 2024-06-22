using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Task4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer gameTimer;
        private List<Shape> bulletHitBoxes = new List<Shape>();
        private List<Shape> enemyHitBoxes = new List<Shape>();
        private List<Shape> enemyAndBulletHitBoxes = new List<Shape>();
        int lives = 3;
        int score = 0;

        Rectangle bullet;
        Rectangle enemy;
        Random random = new();
        private DispatcherTimer bulletTimer;

        private DispatcherTimer moveEnemyTimer;
        private DispatcherTimer enemyTimer;


        public MainWindow()
        {
            InitializeComponent();

            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            bulletTimer = new DispatcherTimer(DispatcherPriority.Render);
            bulletTimer.Interval = TimeSpan.FromMilliseconds(20);
            bulletTimer.Tick += BulletTimer_Tick;

            moveEnemyTimer = new DispatcherTimer(DispatcherPriority.Render);
            moveEnemyTimer.Interval = TimeSpan.FromMilliseconds(50);
            moveEnemyTimer.Tick += MoveEnemyTimer_Tick;
            moveEnemyTimer.Start();

            enemyTimer = new DispatcherTimer(DispatcherPriority.Render);
            enemyTimer.Interval = TimeSpan.FromMilliseconds(500);
            enemyTimer.Tick += EnemyTimer_Tick;
            enemyTimer.Start();

            livesLabel.Content = lives.ToString();
            scoreLabel.Content = score.ToString();

            endLabel.Visibility = Visibility.Hidden;
        }

        private void EnemyTimer_Tick(object? sender, EventArgs e)
        {
            enemy = new Rectangle
            {
                Width = 100,
                Height = 100,
                Fill = new ImageBrush(new BitmapImage(new Uri("enemy.png", UriKind.Relative)))
            };
            gameCanvas.Children.Add(enemy);
            enemyHitBoxes.Add(enemy);
            enemyAndBulletHitBoxes.Add(enemy);

            double x = new Random().Next(10, 700);
            Canvas.SetLeft(enemy, x);
            Canvas.SetTop(enemy, -enemy.Height);
        }

        public void MoveEnemyTimer_Tick(object? sender, EventArgs e)
        {
            foreach (Shape enemy in enemyHitBoxes)
            {
                double y = (double)enemy.GetValue(Canvas.TopProperty);
                Canvas.SetTop(enemy, y + 3.5);
                if (y >= 340)
                {
                    moveEnemyTimer.Stop();
                    endLabel.Visibility = Visibility.Visible;
                    break;
                }
            }

        }

        public void BulletTimer_Tick(object? sender, EventArgs e)
        {
            foreach (Shape bullet in bulletHitBoxes)
            {
                double y = (double)bullet.GetValue(Canvas.TopProperty);
                Canvas.SetTop(bullet, y - 5);
            }
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            List<Shape> bulletsToRemove = new List<Shape>();
            List<Shape> enemiesToRemove = new List<Shape>();

            if (score >= 1)
                goLabel.Visibility = Visibility.Hidden;




            foreach (Shape enemy in enemyHitBoxes)
            {
                if (CheckCollisionWithSpaceship(enemy))
                {
                    enemiesToRemove.Add(enemy);
                    gameCanvas.Children.Remove(enemy);
                    lives--;
                    livesLabel.Content = lives.ToString();

                    if (lives <= 0)
                    {
                        EndGame();
                        return;
                    }
                }
            }

            foreach (Shape bullet in bulletHitBoxes)
            {
                Rect bulletRect = new Rect(Canvas.GetLeft(bullet), Canvas.GetTop(bullet), bullet.Width, bullet.Height);

                if (Canvas.GetTop(bullet) <= 0)
                {
                    enemiesToRemove.Add(bullet);
                    gameCanvas.Children.Remove(bullet);
                }

                foreach (Shape enemy in enemyHitBoxes)
                {
                    Rect enemyRect = new Rect(Canvas.GetLeft(enemy), Canvas.GetTop(enemy), enemy.Width, enemy.Height);

                    if (enemyRect.IntersectsWith(bulletRect))
                    {
                        bulletsToRemove.Add(bullet);
                        enemiesToRemove.Add(enemy);

                        gameCanvas.Children.Remove(bullet);
                        gameCanvas.Children.Remove(enemy);

                        score = score + 1;
                        scoreLabel.Content = score;
                    }
                }
            }

            foreach (Shape bullet in bulletsToRemove)
            {
                bulletHitBoxes.Remove(bullet);
            }
            foreach (Shape enemy in enemiesToRemove)
            {
                enemyHitBoxes.Remove(enemy);
            }
        }

        private bool CheckCollisionWithSpaceship(Shape enemy)
        {
            Rect spaceshipRect = new Rect(Canvas.GetLeft(spaceship), Canvas.GetTop(spaceship), spaceship.Width, spaceship.Height);
            Rect enemyRect = new Rect(Canvas.GetLeft(enemy), Canvas.GetTop(enemy), enemy.Width, enemy.Height);

            return enemyRect.IntersectsWith(spaceshipRect);
        }

        private void EndGame()
        {
            endLabel.Visibility = Visibility.Visible;

            gameTimer.Stop();
            bulletTimer.Stop();
            moveEnemyTimer.Stop();
            enemyTimer.Stop();
        }

        private void MoveObject(UIElement gameObject, double x, double y)
        {
            Canvas.SetLeft(gameObject, x);
            Canvas.SetTop(gameObject, y);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var speed = 50;
            var geometry = GetGeometry(spaceship);


            double x = 0;
            if (e.Key == Key.Left)
            {
                x = -speed;
            }
            else if (e.Key == Key.Right)
            {
                x = speed;
            }
            else if (e.Key == Key.Space)
            {
                FireBullet();
            }

            x = geometry.Bounds.X + x;

            if (x >= 0 && x + spaceship.Width <= Width)
            {
                MoveObject(spaceship, x, 314);
            }
        }

        private void FireBullet()
        {
            bullet = new Rectangle
            {
                Height = 50,
                Width = 50,
                Fill = new ImageBrush(new BitmapImage(new Uri("bullet.png", UriKind.Relative)))
            };

            Canvas.SetLeft(bullet, Canvas.GetLeft(spaceship) + 25);
            Canvas.SetTop(bullet, Canvas.GetTop(spaceship) - spaceship.Height + 50);

            gameCanvas.Children.Add(bullet);
            bulletHitBoxes.Add(bullet);
            enemyAndBulletHitBoxes.Add(bullet);


            bulletTimer.Start();

        }

        private bool CheckCollisions()
        {
            // Получаем координаты и размеры для врага
            Rect enemyHitBox = new Rect(
            Canvas.GetLeft(enemy),
            Canvas.GetTop(enemy),
            enemy.Width,
                enemy.Height);

            // Получаем координаты и размеры для пули
            Rect bulletHitBox = new Rect(
                Canvas.GetLeft(bullet),
                Canvas.GetTop(bullet),
                bullet.Width,
                bullet.Height);

            // Проверяем пересечение прямоугольников
            return enemyHitBox.IntersectsWith(bulletHitBox);
        }

        private Geometry GetGeometry(Shape gameObject)
        {
            var geometry = gameObject.RenderedGeometry;
            geometry.Transform = (Transform)gameObject.TransformToAncestor(gameCanvas);
            return geometry;
        }
    }
}