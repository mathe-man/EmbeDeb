using AvalonDock;
using AvalonDock.Themes;
using Desktop.Models.Messages;
using EmbeDebInterpreter.Communication.CommunicationProvider;
using EmbeDebInterpreter.Message;
using System.Reflection;
using System.Windows;

namespace Desktop;

public partial class MainWindow : Window
{
    private static MessageDispatcher _dispatcher;
    public void AddProvider(ICommunicationProvider provider)
        => _dispatcher.SubscribeToProvider(provider);

    public void AddAssemblyHandlers(Assembly assembly)
        => _dispatcher.RegisterAssemblyHandlers(assembly);

    public MainWindow()
    {
        InitializeComponent();

        // AvalonDock Arc theme (Dark)
        DockingManager.Theme = new ArcDarkTheme();

        // Embedeb interpreter setup
        // Dispatcher
        _dispatcher = new MessageDispatcher(true);

        // Register all message handlers in the current assembly
        _dispatcher.RegisterAssemblyHandlers(Assembly.GetExecutingAssembly());

    }
}