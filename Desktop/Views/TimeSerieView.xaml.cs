using System.Windows.Controls;
using Desktop.ViewModels;

namespace Desktop.Views;

public partial class TimeSerieView : UserControl
{
    public TimeSerieView()
    {
        InitializeComponent();

        DataContext = new TimeSerieViewModel();
    }
}
