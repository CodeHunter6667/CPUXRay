using CPUXRay.Models;
using CPUXRay.Services;
using CPUXRay.ViewModel;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CPUXRay
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;
        private readonly HardwareMonitorService _service;
        private readonly DispatcherQueueTimer _timer;

        // controla com que frequência os discos são atualizados (em ticks; o timer é 1s)
        private int _diskTickCounter;
        private const int DiskUpdateIntervalTicks = 30; // exemplo: 30s

        public MainWindow()
        {
            this.InitializeComponent();

            _service = new HardwareMonitorService();
            _vm = new MainViewModel();

            // Atribui diretamente ao Grid nomeado (deve existir no XAML: x:Name="RootGrid")
            RootGrid.DataContext = _vm;

            // DispatcherQueueTimer é a forma recomendada para timers na UI do WinUI
            _timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1); // atualizar a cada segundo
            _timer.Tick += Timer_Tick;
            _timer.Start();

            // Força atualização inicial dos discos na primeira carga
            _diskTickCounter = DiskUpdateIntervalTicks;

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
                SystemInfo dados = await Task.Run(() => _service.BuscarDadosSistema());
                if (dados != null)
                {
                    // decide se atualiza discos nesta iteração
                    _diskTickCounter++;
                    bool updateDiscos = false;
                    if (_diskTickCounter >= DiskUpdateIntervalTicks)
                    {
                        updateDiscos = true;
                        _diskTickCounter = 0;
                    }

                    // Atualiza ViewModel (propriedades disparam INotifyPropertyChanged)
                    _vm.UpdateFromSistema(dados, updateDiscos);
                }
            }
            catch (Exception)
            {
                // opcional: log
            }
        }
    }
}
