using HardwareMonitor.Models;
using System;
using System.Management;
namespace HardwareMonitor.Services
{
    internal class BuscaDadosService
    {
        public Sistema BuscarDadosSistema()
        {

            Sistema sistema = new Sistema();

            //Obtendo leitura de CPU
            Cpu cpu = new Cpu();
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name, NumberOfCores, NumberOfLogicalProcessors, MaxClockSpeed FROM Win32_Processor");

                foreach (ManagementObject obj in searcher.Get())
                {
                    cpu.nomeProcessador = obj["Name"]?.ToString();
                    cpu.nucleosFisicosProcessador = Convert.ToInt32(obj["NumberOfCores"]);
                    cpu.nucleosLogicosProcessador = Convert.ToInt32(obj["NumberOfLogicalProcessors"]);
                    cpu.frequenciaMaximaMHzProcessador = Convert.ToInt32(obj["MaxClockSpeed"]);
                }

                ManagementObjectSearcher cpuUsage = new ManagementObjectSearcher("SELECT LoadPercentage FROM Win32_Processor");

                foreach (ManagementObject obj in cpuUsage.Get())
                {
                    cpu.usoPorcentagemProcessador = Convert.ToInt32(obj["LoadPercentage"]);
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

                    ram.totalMBMemoria = totalKBMemoria / 1024;
                    ram.totalEmUsoMBMemoria = usadoKBMemoria / 1024;
                    ram.totalLivreMBMemoria = livreKBMemoria / 1024;
                    ram.usoPorcentagemMemoria = (int)((usadoKBMemoria / (double)totalKBMemoria) * 100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Erro ao obter informações da Memória RAM: {ex.Message}");
            }
            //Obtendo leitura de Disco Rígido
            Disco disco = new Disco();
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT DeviceID, Size, FreeSpace FROM Win32_LogicalDisk WHERE DriveType=3");
                foreach (ManagementObject obj in searcher.Get())
                {
                    string nomeDisco = obj["DeviceID"]?.ToString();
                    double capacidadeTotalBytes = Convert.ToDouble(obj["Size"]);
                    double espacoLivreBytes = Convert.ToDouble(obj["FreeSpace"]);
                    double capacidadeTotalGB = Math.Round(capacidadeTotalBytes / (1024 * 1024 * 1024), 2);
                    double espacoLivreGB = Math.Round(espacoLivreBytes / (1024 * 1024 * 1024), 2);
                    int percentualUsoDisco = (int)(((capacidadeTotalBytes - espacoLivreBytes) / capacidadeTotalBytes) * 100);
                    disco.nomeDisco = nomeDisco;
                    disco.capacidadeTotalGBDisco = capacidadeTotalGB;
                    disco.espacoLivreGBDisco = espacoLivreGB;
                    disco.percentualUsoDisco = percentualUsoDisco;
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
                    placaMae.fabricantePlacaMae = obj["Manufacturer"]?.ToString();
                    placaMae.modeloPlacaMae = obj["Product"]?.ToString();
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
                    placaVideo.nomePlacaVideo = obj["Name"]?.ToString();
                    long memoriaBytes = Convert.ToInt64(obj["AdapterRAM"]);
                    placaVideo.memoriaTotalMBPlacaVideo = (int)(memoriaBytes / (1024 * 1024));
                    placaVideo.versaoDriverPlacaVideo = obj["DriverVersion"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Erro ao obter informações da Placa de Vídeo: {ex.Message}");
            }
            //Reunindo os dados do sistema no pacote e retornando
            sistema.cpu = new Cpu(cpu.nomeProcessador, cpu.nucleosFisicosProcessador, cpu.nucleosLogicosProcessador, cpu.frequenciaMaximaMHzProcessador, cpu.usoPorcentagemProcessador);
            sistema.memoria = new Memoria(ram.totalMBMemoria, ram.totalEmUsoMBMemoria, ram.totalLivreMBMemoria, ram.usoPorcentagemMemoria);
            sistema.disco = new Disco(disco.nomeDisco, disco.capacidadeTotalGBDisco, disco.espacoLivreGBDisco, disco.percentualUsoDisco);
            sistema.placaMae = new PlacaMae(placaMae.fabricantePlacaMae, placaMae.modeloPlacaMae);
            sistema.placaVideo = new PlacaVideo(placaVideo.nomePlacaVideo, placaVideo.memoriaTotalMBPlacaVideo, placaVideo.versaoDriverPlacaVideo);
            return sistema;
        }
    }
}
