using CommunityToolkit.Mvvm.ComponentModel;
using Desktop.Models.Messages;
using EmbeDebInterpreter.Message;
using System.Collections.ObjectModel;
using System.Windows;

namespace Desktop.ViewModels;

public partial class EventViewModel : ObservableObject
{
    public ObservableCollection<EventMessage> RecentEvents { get; } = new();
    public ObservableCollection<EventMessage> Events { get; } = new ();

    [ObservableProperty]
    public int recentDelaySecond = 5;

    public EventViewModel()
    {
        Message.OnMessage<EventMessage>(e =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Events.Add(e);
                RecentEvents.Add(e);
            });

            _ = RemoveFromRecentDelayedAsync(e, RecentDelaySecond);
        });
    }

    private async Task RemoveFromRecentDelayedAsync(EventMessage message, int delaySecond)
    {
        await Task.Delay(TimeSpan.FromSeconds(delaySecond));

        Application.Current.Dispatcher.Invoke(() =>
        {
            RecentEvents.Remove(message);
        });
    }
}
