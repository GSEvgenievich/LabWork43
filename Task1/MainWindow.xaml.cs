using System.Windows;
using System.Windows.Threading;

namespace Task1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Render);

        int cookies = 0;
        int grandmothers = 0;
        int cena = 15;

        public MainWindow()
        {
            InitializeComponent();

            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            timer.Start();

            GrandmotherRectangle.IsEnabled = false;
            GrandmotherRectangle.Opacity = 0.5;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            cookies = cookies + 1;
            ScoreLabel.Content = cookies;

            EnabledConfirmation();
        }

        private void CookieEllipse_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Timer_Tick(sender, e);

            EnabledConfirmation();
        }

        private void GrandmotherRectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            cookies = cookies - cena;
            ScoreLabel.Content = cookies; 
            EnabledConfirmation();
            grandmothers = grandmothers + 1;
            GrandmothersLabel.Content = grandmothers;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / (grandmothers + 1));
            timer.Start();
        }

        private void EnabledConfirmation()
        {
            GrandmotherRectangle.IsEnabled = (cookies >= cena);
            GrandmotherRectangle.Opacity = (cookies >= cena) ? 1 : 0.5;
        }
    }
}