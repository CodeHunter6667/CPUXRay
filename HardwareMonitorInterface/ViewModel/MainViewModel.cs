using HardwareMonitor.Models;
using System;
using System.ComponentModel;

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

        // Memória
        public int MemTotalMB { get => _memTotalMB; set { _memTotalMB = value; Raise(nameof(MemTotalMB)); } }
        private int _memTotalMB;
        public int MemUsedMB { get => _memUsedMB; set { _memUsedMB = value; Raise(nameof(MemUsedMB)); } }
        private int _memUsedMB;
        public int MemFreeMB { get => _memFreeMB; set { _memFreeMB = value; Raise(nameof(MemFreeMB)); } }
        private int _memFreeMB;
        public int MemUsagePct { get => _memUsagePct; set { _memUsagePct = value; Raise(nameof(MemUsagePct)); } }
        private int _memUsagePct;

        // Disco
        public string DiskName { get => _diskName; set { _diskName = value; Raise(nameof(DiskName)); } }
        private string _diskName;
        public double DiskTotalGB { get => _diskTotalGB; set { _diskTotalGB = value; Raise(nameof(DiskTotalGB)); } }
        private double _diskTotalGB;
        public double DiskFreeGB { get => _diskFreeGB; set { _diskFreeGB = value; Raise(nameof(DiskFreeGB)); } }
        private double _diskFreeGB;
        public int DiskUsagePct { get => _diskUsagePct; set { _diskUsagePct = value; Raise(nameof(DiskUsagePct)); } }
        private int _diskUsagePct;

        // Placa Mãe
        public string MotherboardManufacturer { get => _motherboardManufacturer; set { _motherboardManufacturer = value; Raise(nameof(MotherboardManufacturer)); } }
        private string _motherboardManufacturer;
        public string MotherboardModel { get => _motherboardModel; set { _motherboardModel = value; Raise(nameof(MotherboardModel)); } }
        private string _motherboardModel;

        // GPU
        public string GpuName { get => _gpuName; set { _gpuName = value; Raise(nameof(GpuName)); } }
        private string _gpuName;
        public int GpuMemoryMB { get => _gpuMemoryMB; set { _gpuMemoryMB = value; Raise(nameof(GpuMemoryMB)); } }
        private int _gpuMemoryMB;
        public string GpuDriverVersion { get => _gpuDriverVersion; set { _gpuDriverVersion = value; Raise(nameof(GpuDriverVersion)); } }
        private string _gpuDriverVersion;

        public MainViewModel() { }

        public void UpdateFromSistema(Sistema s)
        {
            if (s == null) return;

            if (s.cpu != null)
            {
                CpuName = s.cpu.nomeProcessador;
                CpuCores = s.cpu.nucleosFisicosProcessador;
                CpuLogical = s.cpu.nucleosLogicosProcessador;
                CpuMaxClockMHz = s.cpu.frequenciaMaximaMHzProcessador;
                CpuUsage = s.cpu.usoPorcentagemProcessador;
            }

            if (s.memoria != null)
            {
                MemTotalMB = s.memoria.totalMBMemoria;
                MemUsedMB = s.memoria.totalEmUsoMBMemoria;
                MemFreeMB = s.memoria.totalLivreMBMemoria;
                MemUsagePct = s.memoria.usoPorcentagemMemoria;
            }

            if (s.disco != null)
            {
                DiskName = s.disco.nomeDisco;
                DiskTotalGB = s.disco.capacidadeTotalGBDisco;
                DiskFreeGB = s.disco.espacoLivreGBDisco;
                DiskUsagePct = s.disco.percentualUsoDisco;
            }

            if (s.placaMae != null)
            {
                MotherboardManufacturer = s.placaMae.fabricantePlacaMae;
                MotherboardModel = s.placaMae.modeloPlacaMae;
            }

            if (s.placaVideo != null)
            {
                GpuName = s.placaVideo.nomePlacaVideo;
                GpuMemoryMB = s.placaVideo.memoriaTotalMBPlacaVideo;
                GpuDriverVersion = s.placaVideo.versaoDriverPlacaVideo;
            }
        }
    }
}