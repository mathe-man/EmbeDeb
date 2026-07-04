using System.Windows.Controls;
using Desktop.ViewModels;

namespace Desktop.Views;

public partial class EventView : UserControl
{
    public EventView()
    {
        InitializeComponent();

        DataContext = new EventViewModel();
    }
}
