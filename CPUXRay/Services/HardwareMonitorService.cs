using CPUXRay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using Vortice.DXGI;

namespace CPUXRay.Services;

public class HardwareMonitorService
{
    public SystemInfo BuscarDadosSistema()
    {

        SystemInfo sistema = new();

        //Obtendo leitura de CPU (WMI) + uso por núcleo (Win32_PerfFormattedData_PerfOS_Processor)
        CpuInfo cpu = new();
        try
        {
            // Informações estáticas/gerais da CPU
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name, NumberOfCores, NumberOfLogicalProcessors, MaxClockSpeed, LoadPercentage FROM Win32_Processor");
            foreach (ManagementObject obj in searcher.Get())
            {
                cpu.NomeProcessador = obj["Name"]?.ToString() ?? string.Empty;
                cpu.NucleosFisicosProcessador = obj["NumberOfCores"] != null ? Convert.ToInt32(obj["NumberOfCores"]) : 0;
                cpu.NucleosLogicosProcessador = obj["NumberOfLogicalProcessors"] != null ? Convert.ToInt32(obj["NumberOfLogicalProcessors"]) : 0;
                cpu.FrequenciaMaximaMHzProcessador = obj["MaxClockSpeed"] != null ? Convert.ToInt32(obj["MaxClockSpeed"]) : 0;
                cpu.UsoPorcentagemProcessador = obj["LoadPercentage"] != null ? Convert.ToInt32(obj["LoadPercentage"]) : 0;
            }

            // Uso por núcleo via Win32_PerfFormattedData_PerfOS_Processor
            try
            {
                var perCoreSearcher = new ManagementObjectSearcher("SELECT Name, PercentProcessorTime FROM Win32_PerfFormattedData_PerfOS_Processor");
                var coreValues = new Dictionary<int, int>();
                foreach (ManagementObject obj in perCoreSearcher.Get())
                {
                    var name = obj["Name"]?.ToString();
                    if (string.IsNullOrEmpty(name) || name == "_Total") continue;
                    if (int.TryParse(name, out int idx))
                    {
                        int val = 0;
                        try { val = obj["PercentProcessorTime"] != null ? Convert.ToInt32(obj["PercentProcessorTime"]) : 0; } catch { val = 0; }
                        coreValues[idx] = val;
                    }
                }

                int logical = cpu.NucleosLogicosProcessador > 0 ? cpu.NucleosLogicosProcessador : Environment.ProcessorCount;
                cpu.UsoPorcentagemPorNucleo = new List<int>();
                for (int i = 0; i < logical; i++)
                {
                    cpu.UsoPorcentagemPorNucleo.Add(coreValues.ContainsKey(i) ? coreValues[i] : 0);
                }
            }
            catch
            {
                // Se falhar ao ler por núcleo, mantém a lista vazia/zeros (o ViewModel faz fallback)
                cpu.UsoPorcentagemPorNucleo = cpu.UsoPorcentagemPorNucleo ?? new List<int>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Erro ao obter informações da CPU: {ex.Message}");
        }

        //Obtendo leitura de Memória RAM
        RamInfo ram = new();
        try
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
            foreach (ManagementObject obj in searcher.Get())
            {
                int totalKBMemoria = Convert.ToInt32(obj["TotalVisibleMemorySize"]);
                int livreKBMemoria = Convert.ToInt32(obj["FreePhysicalMemory"]);
                int usadoKBMemoria = totalKBMemoria - livreKBMemoria;

                ram.TotalMBMemoria = totalKBMemoria / 1024;
                ram.TotalEmUsoMBMemoria = usadoKBMemoria / 1024;
                ram.TotalLivreMBMemoria = livreKBMemoria / 1024;
                ram.UsoPorcentagemMemoria = (int)((usadoKBMemoria / (double)totalKBMemoria) * 100);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Erro ao obter informações da Memória RAM: {ex.Message}");
        }
        //Obtendo leitura de Disco Rígido - agora iterando todos os discos e populando uma lista
        var discos = new List<StorageInfo>();
        try
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT DeviceID, Size, FreeSpace FROM Win32_LogicalDisk WHERE DriveType=3");
            foreach (ManagementObject obj in searcher.Get())
            {
                var deviceId = obj["DeviceID"]?.ToString();
                if (deviceId == null)
                    continue;

                // Proteções contra valores nulos / zero
                if (obj["Size"] == null || obj["FreeSpace"] == null)
                    continue;

                if (!double.TryParse(obj["Size"].ToString(), out double capacidadeTotalBytes))
                    continue;
                if (!double.TryParse(obj["FreeSpace"].ToString(), out double espacoLivreBytes))
                    continue;

                if (capacidadeTotalBytes <= 0)
                    continue;

                double capacidadeTotalGB = Math.Round(capacidadeTotalBytes / (1024 * 1024 * 1024), 2);
                double espacoLivreGB = Math.Round(espacoLivreBytes / (1024 * 1024 * 1024), 2);
                int percentualUsoDisco = (int)(((capacidadeTotalBytes - espacoLivreBytes) / capacidadeTotalBytes) * 100);

                // Tenta mapear o logical disk -> partition -> disk drive para obter modelo/marca
                string modelo = string.Empty;
                try
                {
                    // Associators of logical disk -> partition
                    string assocPartitionsQuery = $"ASSOCIATORS OF {{Win32_LogicalDisk.DeviceID='{deviceId}'}} WHERE AssocClass=Win32_LogicalDiskToPartition";
                    var partitions = new ManagementObjectSearcher(assocPartitionsQuery).Get();
                    foreach (ManagementObject partition in partitions)
                    {
                        var partitionId = partition["DeviceID"]?.ToString();
                        if (string.IsNullOrEmpty(partitionId))
                            continue;

                        // Associators of partition -> disk drive
                        string assocDrivesQuery = $"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partitionId}'}} WHERE AssocClass=Win32_DiskDriveToDiskPartition";
                        var drives = new ManagementObjectSearcher(assocDrivesQuery).Get();
                        foreach (ManagementObject drive in drives)
                        {
                            modelo = drive["Model"]?.ToString() ?? string.Empty;
                            break;
                        }

                        // If found a drive for this partition, break
                        if (!string.IsNullOrEmpty(modelo))
                            break;
                    }
                }
                catch
                {
                    // não falhar inteiro se mapear drive não for possível — manter strings vazias
                }

                var d = new StorageInfo(deviceId, capacidadeTotalGB, espacoLivreGB, percentualUsoDisco, modelo);
                discos.Add(d);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Erro ao obter informações do Disco Rígido: {ex.Message}");
        }
        //Obtendo leitura de Placa Mãe
        MotherboardInfo placaMae = new();
        try
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Manufacturer, Product FROM Win32_BaseBoard");
            foreach (ManagementObject obj in searcher.Get())
            {
                placaMae.FabricantePlacaMae = obj["Manufacturer"]?.ToString();
                placaMae.ModeloPlacaMae = obj["Product"]?.ToString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Erro ao obter informações da Placa Mãe: {ex.Message}");
        }

        //Obtendo leitura de Placa de Vídeo (WMI + fallback DXGI) com merge para evitar duplicatas
        var wmiGpus = new List<(string Name, int MemMB, string Driver, long UsedMB, long AvailableMB)>();
        try
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name, AdapterRAM, DriverVersion FROM Win32_VideoController");
            foreach (ManagementObject obj in searcher.Get())
            {
                string nomePlacaVideo = obj["Name"]?.ToString() ?? string.Empty;
                string versaoDriverPlacaVideo = obj["DriverVersion"]?.ToString() ?? string.Empty;

                long memoriaBytes = 0;
                bool wmiValido = false;
                var adapterRamObj = obj["AdapterRAM"];
                if (adapterRamObj != null)
                {
                    try
                    {
                        memoriaBytes = Convert.ToInt64(adapterRamObj);
                        // Valores próximos de uint.MaxValue (4294967295) ou 0 geralmente indicam resposta inválida do WMI/driver
                        if (memoriaBytes > 0 && memoriaBytes < 4294967295L)
                            wmiValido = true;
                    }
                    catch
                    {
                        wmiValido = false;
                    }
                }

                int memoriaTotalMBPlacaVideo = wmiValido ? (int)(memoriaBytes / (1024 * 1024)) : 0;
                // WMI não fornece uso/disponível; preencher 0 para que DXGI possa sobrescrever se disponível
                wmiGpus.Add((nomePlacaVideo, memoriaTotalMBPlacaVideo, versaoDriverPlacaVideo, 0L, 0L));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Erro ao obter informações da Placa de Vídeo (WMI): {ex.Message}");
        }

        var dxgiGpus = new List<(string Name, int MemMB, string Driver, long UsedMB, long AvailableMB)>();
        try
        {
            using var factory = DXGI.CreateDXGIFactory1<IDXGIFactory1>();
            for (uint i = 0; factory.EnumAdapters1(i, out IDXGIAdapter1 adapter).Success; i++)
            {
                var desc = adapter.Description1;
                string nome = desc.Description.Trim();
                int memoriaMB = (int)(desc.DedicatedVideoMemory.Value / (1024 * 1024));

                long usadaMB = 0;
                long disponivelMB = 0;

                // Tenta obter uso/disponível via IDXGIAdapter3.QueryVideoMemoryInfo (quando suportado)
                try
                {
                    // Se a instância suportar IDXGIAdapter3
                    if (adapter is IDXGIAdapter3 adapter3)
                    {
                        // Primeiro tenta Local
                        var info = adapter3.QueryVideoMemoryInfo(0, MemorySegmentGroup.Local);
                        ulong currentUsage = info.CurrentUsage;
                        ulong budget = info.Budget;

                        // Se Local não fornecer dados, tenta NonLocal (fallback importante para GPUs integradas)
                        if (currentUsage == 0 && budget == 0)
                        {
                            try
                            {
                                var infoNonLocal = adapter3.QueryVideoMemoryInfo(0, MemorySegmentGroup.NonLocal);
                                currentUsage = infoNonLocal.CurrentUsage;
                                budget = infoNonLocal.Budget;
                            }
                            catch
                            {
                                // ignora, manter zeros
                            }
                        }

                        usadaMB = (long)(currentUsage / (1024 * 1024));
                        disponivelMB = (long)(((budget > currentUsage) ? (budget - currentUsage) : 0UL) / (1024 * 1024));
                    }
                    else
                    {
                        // adapter não expõe IDXGIAdapter3 — útil para diagnóstico
                        Console.WriteLine($"   DXGI adapter não expõe IDXGIAdapter3: {nome}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   Erro ao consultar VRAM via DXGI para {nome}: {ex.Message}");
                }

                dxgiGpus.Add((nome, memoriaMB, "DXGI", usadaMB, disponivelMB));
            }
        }
        catch
        {
            // Não falhar se DXGI não estiver disponível
        }

        // Merge: normaliza nomes e combina entradas, preferindo VRAM DXGI quando mais confiável (maior) e mantendo driver válido
        List<(string Name, int MemMB, string Driver, long UsedMB, long AvailableMB)> merged = new List<(string, int, string, long, long)>();

        string NormalizeName(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            // remove conteúdo entre parênteses e depois de vírgula, várias espaços, e baixa caixa
            var withoutParens = Regex.Replace(s, @"\(.+?\)", string.Empty);
            var beforeComma = Regex.Replace(withoutParens, @",.*", string.Empty);
            var compact = Regex.Replace(beforeComma, @"\s+", " ").Trim().ToLowerInvariant();
            return compact;
        }

        bool NamesMatch(string a, string b)
        {
            var na = NormalizeName(a);
            var nb = NormalizeName(b);
            if (string.IsNullOrEmpty(na) || string.IsNullOrEmpty(nb)) return false;
            if (na == nb) return true;
            // se um contém o outro (por exemplo "nvidia geforce gtx 1080" vs "geforce gtx 1080")
            if (na.Contains(nb) || nb.Contains(na)) return true;
            return false;
        }

        // Add WMI entries first
        foreach (var w in wmiGpus)
        {
            merged.Add((w.Name ?? string.Empty, w.MemMB, w.Driver ?? string.Empty, w.UsedMB, w.AvailableMB));
        }

        // Merge DXGI entries: se corresponder a um WMI existente atualiza VRAM se DXGI for mais confiável; caso contrário adiciona novo
        foreach (var d in dxgiGpus)
        {
            var existingIndex = merged.FindIndex(m => NamesMatch(m.Name, d.Name));
            if (existingIndex >= 0)
            {
                var existing = merged[existingIndex];
                int chosenMem = existing.MemMB;
                long chosenUsed = existing.UsedMB;
                long chosenAvailable = existing.AvailableMB;
                // se WMI não trouxe mem (0) ou DXGI é maior, usa DXGI
                if (d.MemMB > existing.MemMB)
                    chosenMem = d.MemMB;
                // Preferir valores DXGI quando fornecidos (não-zero)
                if (d.UsedMB > 0) chosenUsed = d.UsedMB;
                if (d.AvailableMB > 0) chosenAvailable = d.AvailableMB;

                string chosenDriver = !string.IsNullOrWhiteSpace(existing.Driver) && existing.Driver != "0" ? existing.Driver : d.Driver;
                merged[existingIndex] = (existing.Name, chosenMem, chosenDriver, chosenUsed, chosenAvailable);
            }
            else
            {
                merged.Add((d.Name ?? string.Empty, d.MemMB, d.Driver ?? string.Empty, d.UsedMB, d.AvailableMB));
            }
        }

        // Filtra entradas indesejadas (ex.: Microsoft Basic Render Driver com 0 VRAM)
        bool IsMicrosoftBasic(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            var n = name.ToLowerInvariant();
            return n.Contains("microsoft basic") || n.Contains("basic render") || n.Contains("basic display") || n.Contains("microsoft basic render") || n.Contains("microsoft basic display");
        }

        var placas = merged
            .Where(m => !(m.MemMB == 0 && IsMicrosoftBasic(m.Name))) // remove Microsoft Basic com 0 MB
            .Select(m => new GpuInfo(m.Name, m.MemMB, m.Driver, m.UsedMB, m.AvailableMB))
            .ToList();

        //Reunindo os dados do sistema no pacote e retornando
        sistema.Cpu = new CpuInfo(cpu.NomeProcessador, cpu.NucleosFisicosProcessador, cpu.NucleosLogicosProcessador, cpu.FrequenciaMaximaMHzProcessador, cpu.UsoPorcentagemProcessador);
        // copia a lista por núcleo se existir
        try
        {
            // copia os valores populados acima
            sistema.Cpu.UsoPorcentagemPorNucleo = cpu.UsoPorcentagemPorNucleo != null ? new System.Collections.Generic.List<int>(cpu.UsoPorcentagemPorNucleo) : new System.Collections.Generic.List<int>();
        }
        catch
        {
            sistema.Cpu.UsoPorcentagemPorNucleo = new System.Collections.Generic.List<int>();
        }

        sistema.Memoria = new RamInfo(ram.TotalMBMemoria, ram.TotalEmUsoMBMemoria, ram.TotalLivreMBMemoria, ram.UsoPorcentagemMemoria);

        sistema.Discos = discos;

        sistema.PlacaMae = new MotherboardInfo(placaMae.FabricantePlacaMae, placaMae.ModeloPlacaMae);
        sistema.PlacasVideo = placas;
        return sistema;
    }
}
