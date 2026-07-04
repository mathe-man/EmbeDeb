using System.IO.Ports;
using EmbeDebInterpreter.Communication.CommunicationProvider;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;


namespace Desktop.ViewModels;

public partial class SerialCommunicationViewModel : ObservableObject
{
    [ObservableProperty] private int _baudRate = 115200;
    [ObservableProperty] private bool _connectWithDTR = true;

    [ObservableProperty] private bool _canConnect = false;
    [ObservableProperty] private int _selectedPortIndex = 0;
    public ObservableCollection<string> PortList { get; set;  } = new();

    [RelayCommand]
    public void UpdatePortList()
    {
        PortList.Clear();
        var list = SerialPort.GetPortNames();




        PortList.Add("Select Port");

        foreach (var port in list)
            PortList.Add(port);

        SelectedPortIndex = 0;
        CanConnect = false;
    }
    partial void OnSelectedPortIndexChanged(int value)
    {
        CanConnect = value != 0;
    }

    public SerialCommunicationViewModel()
    {
        UpdatePortList();
    }

    [RelayCommand]
    public void OpenSerialPort()
    {
        SerialCommunicationProvider provider = new(PortList[SelectedPortIndex], BaudRate, ConnectWithDTR);


        if (Application.Current.MainWindow is MainWindow mainWindow) {
            mainWindow.AddProvider(provider);
        }
    }
}
