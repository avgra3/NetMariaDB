using MariaDBConnectors;
using Tomlyn;
using Tomlyn.Model;

class Program
{
    public static void Main(string[] args)
    {
    // Getting our toml file loaded
    string toml = File.ReadAllText("Config.toml");
    var model = Toml.ToModel(toml);

    // Database information
    Dictionary<string, object> databaseInformation = new Dictionary<string, object>();

    foreach (var kvp in ((TomlTable) model["database"]!).ToDictionary())
    {
        string key = kvp.Key.ToString();
        object value = kvp.Value;
        databaseInformation.Add(key, value);
    }

    string sqlTest = "SELECT VERSION();";
    Dictionary<string, object> results = MariaDB.ExecuteCommand(databaseInformation, sqlTest).Result;

    Console.WriteLine($"results:\n{results["standardOut"]}");
    Console.WriteLine($"errors:\n{results["standardError"]}");

    }
}


/*
// Executing a sql command 
void ExecuteCommand(Dictionary<string, object> databaseInformation, string sqlScript)
{
    var stdOutBuffer = new StringBuilder();
    var stdErrBuffer = new StringBuilder();

    var cmd = Cli.Wrap(databaseInformation["path_to_exe"].ToString())
            .WithArguments(args =>
                    {
                        args.Add($"--host={databaseInformation["host"].ToString()}")
                        .Add($"--port={databaseInformation["port"]}")
                        .Add($"--user={databaseInformation["username"].ToString()}")
                        .Add($"--password={databaseInformation["password"].ToString()}");

                        // Optional
                        if (databaseInformation["allow_local_infile"] is bool)
                            args.Add("--local-infile=ON");

                        args.Add($"--database={databaseInformation["database"]}")
                        .Add("--verbose")
                        .Add("--column-names")
                        .Add($"--execute").Add(sqlScript);
                    })
    .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
        .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
    .ExecuteAsync();

    var stdOut = stdOutBuffer.ToString();
    var stdErr = stdErrBuffer.ToString();

    Console.WriteLine($"\nOutput of results:\n{stdOut}");
    Console.WriteLine($"\nOutput of errors:\n{stdErr}");

}
*/