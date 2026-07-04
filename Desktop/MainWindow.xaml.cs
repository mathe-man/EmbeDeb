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
    private List<ICommunicationProvider> _providers = new();
    public void AddProvider(ICommunicationProvider provider)
    {
        _providers.Add(provider);
        dispatcher.SubscribeToProvider(provider);
    }

    public void RemoveProvider(ICommunicationProvider provider)
    {
        _providers.Remove(provider);
        // TODO unsubscribe the dispatcher, and remove the list as subscribtions are managed by the dispatcher and are not usefull to know by the MainWindow class
    }

    // Dispatcher
    private MessageDispatcher dispatcher = new(false);

    public MainWindow()
    {
        InitializeComponent();

        // AvalonDock Arc theme (Dark)
        DockingManager.Theme = new ArcDarkTheme();

        // Embedeb interpreter setup
        // Provider
        DebuggingCommunicationProvider provider = new();

        dispatcher.SubscribeToProvider(provider);

        // Register all message handlers in the current assembly
        dispatcher.RegisterAssemblyHandlers(Assembly.GetExecutingAssembly());

        provider.SendCommunication("XX|Ard|TimeSerieValue=Serie1,65,-7.1");
        provider.SendCommunication("XX|Ard|e=Somethinghappened,34");
    }
}