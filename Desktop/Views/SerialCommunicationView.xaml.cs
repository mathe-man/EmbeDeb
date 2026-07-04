using System.Windows.Controls;

namespace Desktop.Views;

public partial class SerialCommunicationView : UserControl
{
    public SerialCommunicationView()
    {
        InitializeComponent();

        DataContext = new ViewModels.SerialCommunicationViewModel();
    }
}
