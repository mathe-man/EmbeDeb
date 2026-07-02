using Desktop.Models.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace Desktop.ViewModels;

public partial class TimeSerieViewModel : ObservableObject
{
    [ObservableProperty] private string _serieName = string.Empty;
    [ObservableProperty] private string _serieValues = string.Empty;

    public ObservableCollection<string> AvaibleSeries = new();

    public TimeSerieViewModel()
    {
        TimeSerieValueMessage.Handle(new EmbeDebInterpreter.Message.RawMessage("TimeSerieValue", "Serie1,0,1.0"));
        TimeSerieValueMessage.Handle(new EmbeDebInterpreter.Message.RawMessage("TimeSerieValue", "Serie1,3,4.3"));
        TimeSerieValueMessage.Handle(new EmbeDebInterpreter.Message.RawMessage("TimeSerieValue", "Serie1,05,-7.1"));

        RefreshSeries();
    }


    [RelayCommand]
    private void AddMessage()
    {
        TimeSerieValueMessage.Handle(new EmbeDebInterpreter.Message.RawMessage("TimeSerieValue", "Serie1,05,-7.1"));
        RefreshSeries();
    }

    [RelayCommand]
    private void RefreshSeries()
    {
        AvaibleSeries.Clear();
        foreach (var serie in TimeSerieValueMessage.GetSeriesNames())
            AvaibleSeries.Add(serie);

        if (SerieName == string.Empty)
            if (AvaibleSeries.Count > 0)
                SerieName = AvaibleSeries[0];

        _serieValues = string.Empty;
        foreach (var value in TimeSerieValueMessage.GetSerie(SerieName))
            SerieValues += $"TimePoint: {value.TimePoint}, Value: {value.Value}\n";
    }

}