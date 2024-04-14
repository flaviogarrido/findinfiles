using System.Text;

internal class Find
{
    static bool _saved = false;

    internal static void Execute()
    {
        Process();
    }

    private static void Process()
    {
        foreach (string folder in Config.Folders)
        {
            Console.WriteLine($"Processando a pasta: {folder}");
            ProcessFilesInDirectory(folder);
        }
    }

    private static void ProcessFilesInDirectory(string directoryPath)
    {
        // Cria um objeto DirectoryInfo para representar o diretório
        DirectoryInfo directory = new DirectoryInfo(directoryPath);

        // Verifica se o diretório existe
        if (!directory.Exists)
        {
            Console.WriteLine("Diretório não encontrado: " + directoryPath);
            return;
        }

        // Ignora pastas específicas
        if (IgnoreFolder(directory))
            return;

        // Processa cada arquivo no diretório
        foreach (FileInfo file in directory.GetFiles())
        {
            // Ignora arquivos .exe e o arquivo .gitignore
            if (!IgnoreFile(file))
                ProcessFile(file.FullName);
        }

        // Recursivamente processa cada subdiretório
        foreach (DirectoryInfo subDir in directory.GetDirectories())
            ProcessFilesInDirectory(subDir.FullName);
    }

    private static bool IgnoreFile(FileInfo file)
    {
        if (Config.IgnoreAllFiles)
        {
            foreach (string extension in Config.IncludeExtensions)
                if (file.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase))
                    return false;

            return true;
        }

        foreach (string ignoreFile in Config.IgnoreFiles)
            if (file.Name.Equals(ignoreFile, StringComparison.OrdinalIgnoreCase))
                return true;

        foreach (string ignoreExtension in Config.IgnoreExtension)
            if (file.Name.Equals(ignoreExtension, StringComparison.OrdinalIgnoreCase))
                return true;

        return false;
    }

    private static bool IgnoreFolder(DirectoryInfo directory)
    {
        foreach (string folder in Config.IgnoreFolders)
            if (directory.Name.Equals(folder, StringComparison.OrdinalIgnoreCase))
                return true;

        return false;
    }

    private static void ProcessFile(string filePath)
    {
        Console.WriteLine("Processando arquivo: " + filePath);
        var lineNumber = 0;

        try
        {
            foreach (string lineText in File.ReadLines(filePath))
            {
                lineNumber++;
                foreach (string findText in Config.Finds)
                    if (lineText.IndexOf(findText, StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        var found = new Found(findText, filePath, lineNumber, lineText);
                        SaveResult(found);
                    }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao processar o arquivo " + filePath + ": " + ex.Message);
        }
    }

    private static void SaveResult(Found found)
    {
        Save(found);
        Print(found);
    }

    private static void Save(Found found)
    {
        if (!_saved)
        {
            if (File.Exists(Config.ResultFile))
                File.Delete(Config.ResultFile);
            else
            {
                string directoryPath = Path.GetDirectoryName(Config.ResultFile);
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
            }
        }

        using (StreamWriter writer = new StreamWriter(Config.ResultFile, append: true, Encoding.UTF8))
        {
            if (!_saved)
            {
                writer.WriteLine("[Text Found];[File];[Line Number];[Line Text]");
                _saved = true;
            }
            writer.WriteLine($"{found.text};{found.filePath};{found.lineNumber};{found.lineText}");
        }
    }

    private static void Print(Found found)
    {
        var oldColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write($"\tFound {found.text} at line {found.lineNumber}");
        Console.WriteLine();
        Console.ForegroundColor = oldColor;
    }

    record Found(string text, string filePath, int lineNumber, string lineText);

}