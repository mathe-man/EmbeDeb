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
    public MainWindow()
    {
        InitializeComponent();

        // AvalonDock Arc theme (Dark)
        DockingManager.Theme = new ArcDarkTheme();

        // Embedeb interpreter setup
        // Provider
        DebuggingCommunicationProvider provider = new();

        // Dispatcher
        MessageDispatcher dispatcher = new (false, provider);

        // Register all message handlers in the current assembly
        dispatcher.RegisterAssemblyHandlers(Assembly.GetExecutingAssembly());

        provider.SendCommunication("XX|Ard|TimeSerieValue=Serie1,65,-7.1");
    }
}