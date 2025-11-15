using HardwareMonitor.Services;
using HardwareMonitor.Models;
using HardwareMonitorInterface.ViewModels;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;

namespace HardwareMonitorInterface
{
    public sealed partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;
        private readonly BuscaDadosService _service;
        private readonly DispatcherQueueTimer _timer;

        public MainWindow()
        {
            this.InitializeComponent();

            _service = new BuscaDadosService();
            _vm = new MainViewModel();

            // Atribui diretamente ao Grid nomeado (deve existir no XAML: x:Name="RootGrid")
            RootGrid.DataContext = _vm;

            // DispatcherQueueTimer é a forma recomendada para timers na UI do WinUI
            _timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1); // atualizar a cada segundo
            _timer.Tick += Timer_Tick;
            _timer.Start();

            // Primeira carga imediata
            _ = RefreshAsync();
        }

        private async void Timer_Tick(object sender, object e)
        {
            // Executa atualização assíncrona
            await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            try
            {
                // Executa leitura em thread de background para não travar a UI
                Sistema dados = await Task.Run(() => _service.BuscarDadosSistema());
                if (dados != null)
                {
                    // Atualiza ViewModel (propriedades disparam INotifyPropertyChanged)
                    _vm.UpdateFromSistema(dados);
                }
            }
            catch (Exception)
            {
                // opcional: log
            }
        }
    }
}