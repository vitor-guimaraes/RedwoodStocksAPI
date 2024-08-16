namespace RedwoodStocksAPI.Domain.Services
{
    public class LogErrorService
    {
        public static void LogError(string message)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string logsFolderPath = Path.Combine(desktopPath, "logs");

            if (!Directory.Exists(logsFolderPath))
            {
                Directory.CreateDirectory(logsFolderPath);
            }

            string logFilePath = Path.Combine(logsFolderPath, "error_log.txt");

            File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }

    }

}

