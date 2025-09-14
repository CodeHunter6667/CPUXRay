using CPUXRay.Models;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;

namespace CPUXRay.Services;

public class HardwareMonitorService
{
    private readonly DispatcherTimer _timer;
    private readonly Action<CpuInfo, RamInfo, GpuInfo, List<StorageInfo>, MotherboardInfo> _onUpdate;

    public HardwareMonitorService(Action<CpuInfo, RamInfo, GpuInfo, List<StorageInfo>, MotherboardInfo> onUpdateCallback, int intervalSeconds = 2)
    {
        _onUpdate = onUpdateCallback;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(intervalSeconds)
        };
        _timer.Tick += Timer_Tick;
    }

    public void Start() => _timer.Start();
    public void Stop() => _timer.Stop();

    private void Timer_Tick(object sender, object e)
    {
        var cpu = CpuService.GetCpuInfo();
        var ram = RamService.GetRamInfo();
        var gpu = GpuService.GetGpuInfo();
        var storage = StorageService.GetStorageInfo();
        var motherboard = MotherboardService.GetMotherboardInfo();

        _onUpdate?.Invoke(cpu, ram, gpu, storage, motherboard);
    }
}