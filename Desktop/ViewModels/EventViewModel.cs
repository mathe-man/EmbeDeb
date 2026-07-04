using Desktop.Models.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

using Timer = System.Timers.Timer;

namespace Desktop.ViewModels;

public partial class EventViewModel : ObservableObject
{
    public ObservableCollection<EventMessage> Events { get; } = new ();
    public ObservableCollection<EventMessage> RecentEvents { get; } = new();

    // Time before an event is considered expired and removed from the RecentEvents collection
    [ObservableProperty] private int _expirationTimeInSeconds = 20;

    public EventViewModel()
    {
        EventMessage.ObjectCreated += (sender, e) => {
            if (sender is EventMessage message)
            {
                Events.Add(message);
                RecentEvents.Add(message);

                // Create a timer to remove the event once it's expired
                Timer timer = new Timer(ExpirationTimeInSeconds * 1000);
                timer.Elapsed += (s, args) =>
                {
                    RecentEvents.Remove(message);

                    (s as Timer)?.Dispose();  // Dispose the timer after it has elapsed
                };

                // Disable auto-reset so the timer only ticks once
                timer.AutoReset = false;
                timer.Start();
            }
        };
    }


}
