using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ПП
{
    public class MonitoringViewModel : INotifyPropertyChanged
    {
        public SeriesCollection SeriesCollection { get; set; }
        public Func<double, string> DateTimeFormatter { get; set; }

        public MonitoringViewModel()
        {
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "CPU Usage",
                    Values = new ChartValues<double>(),
                    Stroke = Brushes.DodgerBlue,
                    Fill = Brushes.Transparent,
                    PointGeometry = null
                }
            };

            DateTimeFormatter = value =>
                DateTime.FromOADate(value).ToString("HH:mm:ss");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}