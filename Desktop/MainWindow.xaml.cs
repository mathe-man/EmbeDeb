using System.Windows;
using AvalonDock;
using AvalonDock.Themes;

namespace Desktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // AvalonDock Arc theme (Dark)
        DockingManager.Theme = new ArcDarkTheme();
    }
}