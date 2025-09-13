using CPUXRay.Models;
using CPUXRay.Readers;
using System.Collections.Generic;

namespace CPUXRay.Services;

public class StorageService
{
    public static List<StorageInfo> GetStorageInfo()
    {
        var drives = StorageReader.GetStorageInfo();
        StorageReader.EnrichStorageInfo(drives);
        return drives;
    }
}
