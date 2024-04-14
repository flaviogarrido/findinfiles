internal class Config
{
    static readonly string
        _arquivoConfiguracao = "findinfiles.cfg";

    static readonly Dictionary<string, List<string>>
        _keyValues = new Dictionary<string, List<string>>();

    internal static bool Ok()
    {
        return VerifyKeysExist();
    }

    internal static void Load()
    {
        LoadKeyValuePairs();
    }

    internal static bool Exists()
    {
        return File.Exists(_arquivoConfiguracao);
    }

    private static void LoadKeyValuePairs()
    {
        _keyValues.Clear();
        foreach (string line in File.ReadLines(_arquivoConfiguracao))
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                continue;

            int separatorIndex = line.IndexOf('=');
            if (separatorIndex == -1)
                continue; // Ignora linhas que não contêm o separador '='

            string key = line.Substring(0, separatorIndex).Trim();
            string value = line.Substring(separatorIndex + 1).Trim();

            if (!_keyValues.ContainsKey(key))
                _keyValues[key] = new List<string>();

            _keyValues[key].Add(value);
        }
    }

    internal static bool VerifyKeysExist()
    {
        string[] requiredKeys = { "folder", "find", "result" };

        foreach (string key in requiredKeys)
        {
            if (!_keyValues.ContainsKey(key))
            {
                Console.WriteLine($"Chave ausente: {key}");
                return false;
            }
        }

        return true;
    }

    internal static IEnumerable<string> Folders
    {
        get
        {
            return _keyValues["folder"];
        }
    }

    internal static string ResultFile
    {
        get
        {
            return _keyValues["result"][0];
        }
    }

    internal static IEnumerable<string> Finds
    {
        get
        {
            return _keyValues["find"];
        }
    }

    internal static IEnumerable<string> IgnoreFolders
    {
        get
        {
            var key = "ignore.folder";
            if (_keyValues.ContainsKey(key))
                return _keyValues[key];
            return new string[] { };
        }
    }

    internal static bool IgnoreAllFiles
    {
        get
        {
            var key = "ignore.files";
            if (_keyValues.ContainsKey(key))
                return _keyValues[key][0].Equals("true", StringComparison.OrdinalIgnoreCase);
            return false;
        }
    }
    internal static IEnumerable<string> IncludeExtensions
    {
        get
        {
            var key = "include.extension";
            if (_keyValues.ContainsKey(key))
                return _keyValues[key];
            return new string[] { };
        }
    }

    internal static IEnumerable<string> IgnoreFiles
    {
        get
        {
            var key = "ignore.file";
            if (_keyValues.ContainsKey(key))
                return _keyValues[key];
            return new string[] { };
        }
    }

    internal static IEnumerable<string> IgnoreExtension
    {
        get
        {
            var key = "ignore.extension";
            if (_keyValues.ContainsKey(key))
                return _keyValues[key];
            return new string[] { };
        }
    }

}