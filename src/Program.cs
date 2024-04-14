Console.ForegroundColor = ConsoleColor.Green;
Console.BackgroundColor = ConsoleColor.Black;

Console.WriteLine("FindInFiles - aplicativo para apoiar no mapeamento de impacto em alterações");

if (Config.Exists())
    Config.Load();

if (Config.Ok())
    Execute();
else
    CancelExecute("As configurações não foram carregadas corretamente");


void Execute()
{
    Find.Execute();
    Console.WriteLine("FindInFiles executado com sucesso, verifique o resultado em #resutado");
    Environment.Exit(0);
}

void CancelExecute(string msg)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(msg);
    Environment.Exit(1);
}