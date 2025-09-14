using CPUXRay.Models;
using CPUXRay.Services;
using Microsoft.UI.Dispatching;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CPUXRay.ViewModel;

public class MainViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public CpuInfo Cpu { get; set; } = new();
    public RamInfo Ram { get; set; } = new RamInfo();
    public GpuInfo Gpu { get; set; } = new();
    public List<StorageInfo> Storage { get; set; } = new();
    public MotherboardInfo Motherboard { get; set; } = new();

    private HardwareMonitorService _monitorService;

    public MainViewModel()
    {
        var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _monitorService = new HardwareMonitorService(dispatcherQueue, OnHardwareUpdated);
        _monitorService.Start();
    }

    private void OnHardwareUpdated(CpuInfo cpu, RamInfo ram, GpuInfo gpu, List<StorageInfo> storage, MotherboardInfo board)
    {
        Cpu = cpu;
        Ram = ram;
        Gpu = gpu;
        Storage = storage;
        Motherboard = board;
        OnPropertyChanged(nameof(Cpu));
        OnPropertyChanged(nameof(Ram));
        OnPropertyChanged(nameof(Gpu));
        OnPropertyChanged(nameof(Storage));
        OnPropertyChanged(nameof(Motherboard));
    }

    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
