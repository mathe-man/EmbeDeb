using Microsoft.UI.Xaml;

namespace Desktop
{
    public partial class App : Application
    {
        private Window? _window;

        // The entry point, equivalent to main
        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }
    }
}
