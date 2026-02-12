using Course.Views;

namespace Course
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new SplashPage(); 
        }
    }
}