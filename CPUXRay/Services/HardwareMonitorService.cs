using CPUXRay.Models;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CPUXRay.Services;

public class HardwareMonitorService
{
    private readonly DispatcherTimer _timer;
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly Action<CpuInfo, RamInfo, GpuInfo, List<StorageInfo>, MotherboardInfo> _onUpdate;

    public HardwareMonitorService(DispatcherQueue dispatcherQueue, Action<CpuInfo, RamInfo, GpuInfo, List<StorageInfo>, MotherboardInfo> onUpdateCallback, int intervalSeconds = 2)
    {
        _dispatcherQueue = dispatcherQueue;
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
        Task.Run(() =>
        {
            var cpuTask = Task.Run(() => CpuService.GetCpuInfo());
            var ramTask = Task.Run(() => RamService.GetRamInfo());
            var gpuTask = Task.Run(() => GpuService.GetGpuInfo());
            var storageTask = Task.Run(() => StorageService.GetStorageInfo());
            var boardTask = Task.Run(() => MotherboardService.GetMotherboardInfo());
            Task.WaitAll(cpuTask, ramTask, gpuTask, storageTask, boardTask);

            var cpu = cpuTask.Result;
            var ram = ramTask.Result;
            var gpu = gpuTask.Result;
            var storage = storageTask.Result;
            var board = boardTask.Result;

            _dispatcherQueue.TryEnqueue(() =>
            {
                _onUpdate?.Invoke(cpu, ram, gpu, storage, board);
            });
        });
    }
}