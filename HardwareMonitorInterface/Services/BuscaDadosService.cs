using HardwareMonitorInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using Vortice.DXGI;
namespace HardwareMonitorInterface.Services;

public class BuscaDadosService
{
    public Sistema BuscarDadosSistema()
    {

        Sistema sistema = new Sistema();

        //Obtendo leitura de CPU
        Cpu cpu = new Cpu();
        try
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name, NumberOfCores, NumberOfLogicalProcessors, MaxClockSpeed, LoadPercentage FROM Win32_Processor");

            foreach (ManagementObject obj in searcher.Get())
            {
                cpu.NomeProcessador = obj["Name"]?.ToString();
                cpu.NucleosFisicosProcessador = Convert.ToInt32(obj["NumberOfCores"]);
                cpu.NucleosLogicosProcessador = Convert.ToInt32(obj["NumberOfLogicalProcessors"]);
                cpu.FrequenciaMaximaMHzProcessador = Convert.ToInt32(obj["MaxClockSpeed"]);
                cpu.UsoPorcentagemProcessador = Convert.ToInt32(obj["LoadPercentage"]);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Erro ao obter informações da CPU: {ex.Message}");
        }
        //Obtendo leitura de Memória RAM
        Memoria ram = new Memoria();
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
        var discos = new List<Disco>();
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

                var d = new Disco(deviceId, capacidadeTotalGB, espacoLivreGB, percentualUsoDisco, modelo);
                discos.Add(d);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Erro ao obter informações do Disco Rígido: {ex.Message}");
        }
        //Obtendo leitura de Placa Mãe
        PlacaMae placaMae = new PlacaMae();
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
        var wmiGpus = new List<(string Name, int MemMB, string Driver)>();
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
                wmiGpus.Add((nomePlacaVideo, memoriaTotalMBPlacaVideo, versaoDriverPlacaVideo));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Erro ao obter informações da Placa de Vídeo (WMI): {ex.Message}");
        }

        var dxgiGpus = new List<(string Name, int MemMB, string Driver)>();
        try
        {
            using var factory = DXGI.CreateDXGIFactory1<IDXGIFactory1>();
            for (uint i = 0; factory.EnumAdapters1(i, out IDXGIAdapter1 adapter).Success; i++)
            {
                var desc = adapter.Description1;
                string nome = desc.Description.Trim();
                int memoriaMB = (int)(desc.DedicatedVideoMemory.Value / (1024 * 1024));
                dxgiGpus.Add((nome, memoriaMB, "DXGI"));
            }
        }
        catch
        {
            // Não falhar se DXGI não estiver disponível
        }

        // Merge: normaliza nomes e combina entradas, preferindo VRAM DXGI quando mais confiável (maior) e mantendo driver válido
        List<(string Name, int MemMB, string Driver)> merged = new List<(string, int, string)>();

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
            merged.Add((w.Name ?? string.Empty, w.MemMB, w.Driver ?? string.Empty));
        }

        // Merge DXGI entries: se corresponder a um WMI existente atualiza VRAM se DXGI for mais confiável; caso contrário adiciona novo
        foreach (var d in dxgiGpus)
        {
            var existingIndex = merged.FindIndex(m => NamesMatch(m.Name, d.Name));
            if (existingIndex >= 0)
            {
                var existing = merged[existingIndex];
                int chosenMem = existing.MemMB;
                // se WMI não trouxe mem (0) ou DXGI é maior, usa DXGI
                if (d.MemMB > existing.MemMB)
                    chosenMem = d.MemMB;
                string chosenDriver = !string.IsNullOrWhiteSpace(existing.Driver) && existing.Driver != "0" ? existing.Driver : d.Driver;
                merged[existingIndex] = (existing.Name, chosenMem, chosenDriver);
            }
            else
            {
                merged.Add((d.Name ?? string.Empty, d.MemMB, d.Driver ?? string.Empty));
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
            .Select(m => new PlacaVideo(m.Name, m.MemMB, m.Driver))
            .ToList();

        //Reunindo os dados do sistema no pacote e retornando
        sistema.Cpu = new Cpu(cpu.NomeProcessador, cpu.NucleosFisicosProcessador, cpu.NucleosLogicosProcessador, cpu.FrequenciaMaximaMHzProcessador, cpu.UsoPorcentagemProcessador);
        sistema.Memoria = new Memoria(ram.TotalMBMemoria, ram.TotalEmUsoMBMemoria, ram.TotalLivreMBMemoria, ram.UsoPorcentagemMemoria);

        sistema.Discos = discos;

        sistema.PlacaMae = new PlacaMae(placaMae.FabricantePlacaMae, placaMae.ModeloPlacaMae);
        sistema.PlacasVideo = placas;
        return sistema;
    }
}
