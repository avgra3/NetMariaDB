using CliWrap;
using System.Text;

namespace MariaDBConnectors;

public static class MariaDB
{
	public static async Task<Dictionary<string, object>> ExecuteCommand(Dictionary<string, object> databaseInformation, string sqlScript)
	{
		if (string.IsNullOrEmpty(databaseInformation["path_to_exe"].ToString()))
		{
			throw new ArgumentException("You did not specify a valid path to the MariaDB executable!") ;
		}

		int verbosity;
		
		var stdOutBuffer = new StringBuilder();
		var stdErrBuffer = new StringBuilder();

		var cmd = await Cli.Wrap(databaseInformation["path_to_exe"].ToString())
				.WithArguments(args =>
				{
					args.Add($"--host={databaseInformation["host"].ToString()}")
						.Add($"--port={databaseInformation["port"]}")
						.Add($"--user={databaseInformation["username"].ToString()}")
						.Add($"--password={databaseInformation["password"].ToString()}")
						// If database is not provided (needs to be empty string) we do not pass it to the command
						.Add($"{(databaseInformation["database"].ToString() != "" ? "--database=" + databaseInformation["database"] : "")}")
						.Add("--column-names")
						.Add("--quick")
						.Add($"--execute={sqlScript}");
					
					// How verbose?
					if (int.TryParse(databaseInformation["verbosity"].ToString().Trim(), out verbosity) && !string.IsNullOrEmpty(databaseInformation["verbosity"].ToString()))
					{
						if (verbosity == 1)
						{
							args.Add("--verbose");
						}
						if (verbosity == 2)
						{
							args.Add("--verbose")
								.Add("--verbose");
						}
					}
					if (databaseInformation["allow_local_infile"] is bool)
						args.Add("--local-infile=ON");						
				})
				.WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
				.WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
				.ExecuteAsync();

		var stdOut = stdOutBuffer.ToString();
		var stdErr = stdErrBuffer.ToString();

		Dictionary<string, object> results = new Dictionary<string, object>(7)
        {
            { "standardOut", stdOut },
            { "standardError", stdErr },
			{ "isSuccess", $"{cmd.IsSuccess}" },
			{ "exitCode", $"{cmd.ExitCode}" },
			{ "startTime", $"{cmd.StartTime}"},
			{ "exitTime", $"{cmd.ExitTime}"},
			{ "runTime", $"{cmd.RunTime}"}
        };

		return results;
	}
}
