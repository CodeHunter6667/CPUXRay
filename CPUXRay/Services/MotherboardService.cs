using CPUXRay.Models;
using CPUXRay.Readers;

namespace CPUXRay.Services;

public static class MotherboardService
{
    public static MotherboardInfo GetMotherboardInfo()
    {
        var board = MotherboardReader.GetMotherboardInfo();
        MotherboardReader.EnrichWithBiosInfo(board);
        MotherboardReader.EnrichWithMemorySlots(board);
        return board;
    }
}
