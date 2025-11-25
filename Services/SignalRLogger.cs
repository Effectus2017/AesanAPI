using System.IO;

namespace Api.Services;

public static class SignalRLogger
{
    private static readonly string _logFilePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "logs",
        $"signalr_{DateTime.Now:yyyy-MM-dd}.log"
    );

    static SignalRLogger()
    {
        // Asegurar que el directorio de logs existe
        var logDirectory = Path.GetDirectoryName(_logFilePath);
        if (!string.IsNullOrEmpty(logDirectory) && !Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }
        
        // Log la ubicación del archivo al inicializar
        Console.WriteLine($"[SignalRLogger] Archivo de log inicializado en: {_logFilePath}");
        Console.WriteLine($"[SignalRLogger] Directorio base: {AppDomain.CurrentDomain.BaseDirectory}");
    }

    public static void LogToFile(string message)
    {
        try
        {
            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
            Console.WriteLine(logMessage);
            System.IO.File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error escribiendo log de SignalR: {ex.Message}");
            Console.WriteLine($"Ruta del archivo: {_logFilePath}");
        }
    }
    
    /// <summary>
    /// Obtiene la ruta completa del archivo de log actual
    /// </summary>
    public static string GetLogFilePath()
    {
        return _logFilePath;
    }
}

