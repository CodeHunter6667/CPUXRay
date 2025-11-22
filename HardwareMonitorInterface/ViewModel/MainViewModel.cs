using HardwareMonitorInterface.Models;
using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace HardwareMonitorInterface.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void Raise(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // CPU
        public string CpuName { get => _cpuName; set { _cpuName = value; Raise(nameof(CpuName)); } }
        private string _cpuName;
        public int CpuUsage { get => _cpuUsage; set { _cpuUsage = value; Raise(nameof(CpuUsage)); } }
        private int _cpuUsage;
        public int CpuCores { get => _cpuCores; set { _cpuCores = value; Raise(nameof(CpuCores)); } }
        private int _cpuCores;
        public int CpuLogical { get => _cpuLogical; set { _cpuLogical = value; Raise(nameof(CpuLogical)); } }
        private int _cpuLogical;
        public int CpuMaxClockMHz { get => _cpuMaxClockMHz; set { _cpuMaxClockMHz = value; Raise(nameof(CpuMaxClockMHz)); } }
        private int _cpuMaxClockMHz;

        // Uso por núcleo lógico (binding para o ItemsControl no XAML)
        public ObservableCollection<CoreItem> CpuCoresUsage { get; } = new();

        // Memória
        public int MemTotalMB { get => _memTotalMB; set { _memTotalMB = value; Raise(nameof(MemTotalMB)); } }
        private int _memTotalMB;
        public int MemUsedMB { get => _memUsedMB; set { _memUsedMB = value; Raise(nameof(MemUsedMB)); } }
        private int _memUsedMB;
        public int MemFreeMB { get => _memFreeMB; set { _memFreeMB = value; Raise(nameof(MemFreeMB)); } }
        private int _memFreeMB;
        public int MemUsagePct { get => _memUsagePct; set { _memUsagePct = value; Raise(nameof(MemUsagePct)); } }
        private int _memUsagePct;

        // Discos - collection to support multiple disks
        public ObservableCollection<Disco> Discos { get => _discos; }
        private readonly ObservableCollection<Disco> _discos = new();

        // Back-compat: single-disk properties still available and populated from first disk (if any)
        public string DiskName { get => _diskName; set { _diskName = value; Raise(nameof(DiskName)); } }
        private string _diskName;
        public double DiskTotalGB { get => _diskTotalGB; set { _diskTotalGB = value; Raise(nameof(DiskTotalGB)); } }
        private double _diskTotalGB;
        public double DiskFreeGB { get => _diskFreeGB; set { _diskFreeGB = value; Raise(nameof(DiskFreeGB)); } }
        private double _diskFreeGB;
        public int DiskUsagePct { get => _diskUsagePct; set { _diskUsagePct = value; Raise(nameof(DiskUsagePct)); } }
        private int _diskUsagePct;
        public string DiskModel { get => _diskModel; set { _diskModel = value; Raise(nameof(DiskModel)); } }
        private string _diskModel;

        // Placa Mãe
        public string MotherboardManufacturer { get => _motherboardManufacturer; set { _motherboardManufacturer = value; Raise(nameof(MotherboardManufacturer)); } }
        private string _motherboardManufacturer;
        public string MotherboardModel { get => _motherboardModel; set { _motherboardModel = value; Raise(nameof(MotherboardModel)); } }
        private string _motherboardModel;

        // GPU
        public ObservableCollection<PlacaVideo> PlacasVideo { get => _placasVideo; }
        private readonly ObservableCollection<PlacaVideo> _placasVideo = new();

        public string GpuName { get => _gpuName; set { _gpuName = value; Raise(nameof(GpuName)); } }
        private string _gpuName;
        public long GpuMemoryMB { get => _gpuMemoryMB; set { _gpuMemoryMB = value; Raise(nameof(GpuMemoryMB)); } }
        private long _gpuMemoryMB;
        public string GpuDriverVersion { get => _gpuDriverVersion; set { _gpuDriverVersion = value; Raise(nameof(GpuDriverVersion)); } }
        private string _gpuDriverVersion;

        // Novas propriedades para VRAM em uso e disponível
        public long GpuMemoryUsedMB { get => _gpuMemoryUsedMB; set { _gpuMemoryUsedMB = value; Raise(nameof(GpuMemoryUsedMB)); } }
        private long _gpuMemoryUsedMB;
        public long GpuMemoryAvailableMB { get => _gpuMemoryAvailableMB; set { _gpuMemoryAvailableMB = value; Raise(nameof(GpuMemoryAvailableMB)); } }
        private long _gpuMemoryAvailableMB;

        public MainViewModel() { }

        /// <summary>
        /// Atualiza propriedades a partir de Sistema.
        /// Se <paramref name="updateDiscos"/> for true, atualiza a coleção Discos e as propriedades de single-disk (compatibilidade).
        /// Caso contrário, mantém os discos como estão (não serão substituídos a cada tick).
        /// </summary>
        public void UpdateFromSistema(Sistema s, bool updateDiscos = false)
        {
            if (s == null) return;

            if (s.Cpu != null)
            {
                CpuName = s.Cpu.NomeProcessador;
                CpuCores = s.Cpu.NucleosFisicosProcessador;
                CpuLogical = s.Cpu.NucleosLogicosProcessador;
                CpuMaxClockMHz = s.Cpu.FrequenciaMaximaMHzProcessador;
                CpuUsage = s.Cpu.UsoPorcentagemProcessador;

                // Atualiza barras por núcleo (suporta tanto a nova propriedade UsoPorcentagemPorNucleo quanto fallback)
                AtualizarNucleosFromCpu(s.Cpu);
            }

            if (s.Memoria != null)
            {
                MemTotalMB = s.Memoria.TotalMBMemoria;
                MemUsedMB = s.Memoria.TotalEmUsoMBMemoria;
                MemFreeMB = s.Memoria.TotalLivreMBMemoria;
                MemUsagePct = s.Memoria.UsoPorcentagemMemoria;
            }

            if (updateDiscos)
            {
                if (s.Discos != null)
                {
                    _discos.Clear();
                    foreach (var disco in s.Discos)
                    {
                        _discos.Add(disco);
                    }

                    var first = s.Discos.FirstOrDefault();
                    if (first != null)
                    {
                        DiskName = first.NomeDisco;
                        DiskTotalGB = first.CapacidadeTotalGBDisco;
                        DiskFreeGB = first.EspacoLivreGBDisco;
                        DiskUsagePct = first.PercentualUsoDisco;
                        DiskModel = first.Modelo;
                    }
                }
                else
                {
                    _discos.Clear();
                    DiskName = string.Empty;
                    DiskTotalGB = 0;
                    DiskFreeGB = 0;
                    DiskUsagePct = 0;
                    DiskModel = string.Empty;
                }
            }

            if (s.PlacaMae != null)
            {
                MotherboardManufacturer = s.PlacaMae.FabricantePlacaMae;
                MotherboardModel = s.PlacaMae.ModeloPlacaMae;
            }

            if (s.PlacasVideo != null)
            {
                // Recria a coleção de placas (mantém comportamento atual de rebind para garantir UI atualizada)
                _placasVideo.Clear();
                foreach (var placa in s.PlacasVideo)
                {
                    _placasVideo.Add(placa);
                }

                var placasVideo = s.PlacasVideo.FirstOrDefault();
                if (placasVideo != null)
                {
                    // Usa os setters para disparar PropertyChanged
                    GpuName = placasVideo.NomePlacaVideo;
                    GpuMemoryMB = placasVideo.MemoriaTotalMBPlacaVideo;
                    GpuDriverVersion = placasVideo.VersaoDriverPlacaVideo;
                    GpuMemoryUsedMB = placasVideo.MemoriaEmUsoMBPlacaVideo;
                    GpuMemoryAvailableMB = placasVideo.MemoriaDisponivelMBPlacaVideo;
                }
            }
        }

        // Tenta extrair lista de percentuais por núcleo da instância Cpu (por reflection para compatibilidade).
        // Se a propriedade não existir, cria fallback com zeros (contagem baseada em CpuLogical).
        private void AtualizarNucleosFromCpu(Cpu cpu)
        {
            if (cpu == null)
            {
                CpuCoresUsage.Clear();
                return;
            }

            // tenta obter property named "UsoPorcentagemPorNucleo"
            List<int>? percents = null;
            try
            {
                var prop = cpu.GetType().GetProperty("UsoPorcentagemPorNucleo");
                if (prop != null)
                {
                    var val = prop.GetValue(cpu);
                    if (val is IEnumerable<int> ints)
                        percents = ints.ToList();
                    else if (val is IEnumerable<object> objs)
                    {
                        percents = objs.Select(o =>
                        {
                            if (o == null) return 0;
                            if (o is int i) return i;
                            if (int.TryParse(o.ToString(), out int parsed)) return parsed;
                            return 0;
                        }).ToList();
                    }
                }
            }
            catch
            {
                percents = null;
            }

            // fallback: se não houver dados por núcleo, cria lista baseada em CpuLogical preenchida com 0
            if (percents == null)
            {
                int logical = cpu.NucleosLogicosProcessador > 0 ? cpu.NucleosLogicosProcessador : Environment.ProcessorCount;
                percents = Enumerable.Repeat(0, logical).ToList();
            }

            // Atualiza coleção ObservableCollection<CoreItem>
            if (CpuCoresUsage.Count != percents.Count)
            {
                CpuCoresUsage.Clear();
                for (int i = 0; i < percents.Count; i++)
                {
                    CpuCoresUsage.Add(new CoreItem { Index = i, Usage = percents[i] });
                }
            }
            else
            {
                for (int i = 0; i < percents.Count; i++)
                {
                    CpuCoresUsage[i].Usage = percents[i];
                }
            }
        }

        // Classe interna para representar um núcleo lógico no binding
        public class CoreItem : INotifyPropertyChanged
        {
            private int _usage;
            public int Index { get; set; }
            public int DisplayIndex => Index + 1; // para exibir Núcleo 1,2,...
            public int Usage
            {
                get => _usage;
                set
                {
                    if (_usage == value) return;
                    _usage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Usage)));
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }
    }
}