using CommunityToolkit.Mvvm.ComponentModel;
using Desktop.Models.Messages;
using System.Collections.ObjectModel;

namespace Desktop.ViewModels;

public partial class EventViewModel : ObservableObject
{
    public ObservableCollection<EventMessage> Events = new ();

}
