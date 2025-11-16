using HardwareMonitorInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
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

                var d = new Disco(deviceId, capacidadeTotalGB, espacoLivreGB, percentualUsoDisco);
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
        //Obtendo leitura de Placa de Vídeo
        PlacaVideo placaVideo = new PlacaVideo();
        try
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name, AdapterRAM, DriverVersion FROM Win32_VideoController");
            foreach (ManagementObject obj in searcher.Get())
            {
                placaVideo.NomePlacaVideo = obj["Name"]?.ToString();
                long memoriaBytes = Convert.ToInt64(obj["AdapterRAM"]);
                placaVideo.MemoriaTotalMBPlacaVideo = (int)(memoriaBytes / (1024 * 1024));
                placaVideo.VersaoDriverPlacaVideo = obj["DriverVersion"]?.ToString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Erro ao obter informações da Placa de Vídeo: {ex.Message}");
        }
        //Reunindo os dados do sistema no pacote e retornando
        sistema.Cpu = new Cpu(cpu.NomeProcessador, cpu.NucleosFisicosProcessador, cpu.NucleosLogicosProcessador, cpu.FrequenciaMaximaMHzProcessador, cpu.UsoPorcentagemProcessador);
        sistema.Memoria = new Memoria(ram.TotalMBMemoria, ram.TotalEmUsoMBMemoria, ram.TotalLivreMBMemoria, ram.UsoPorcentagemMemoria);

        // Se precisar manter a assinatura atual (um único Disco), atribui o primeiro disco encontrado ou um novo objeto vazio.
        sistema.Discos = discos;

        sistema.PlacaMae = new PlacaMae(placaMae.FabricantePlacaMae, placaMae.ModeloPlacaMae);
        sistema.PlacaVideo = new PlacaVideo(placaVideo.NomePlacaVideo, placaVideo.MemoriaTotalMBPlacaVideo, placaVideo.VersaoDriverPlacaVideo);
        return sistema;
    }
}
