using System;
using Tomlyn;
using Tomlyn.Model;

// Getting our toml file loaded
string toml = File.ReadAllText("Config.toml");
var model = Toml.ToModel(toml);

// Server information
Dictionary<string, object> serverInformation = new Dictionary<string, object>();

foreach (var kvp in ((TomlTable)model["server"]!).ToDictionary())
{
		string key = kvp.Key.ToString();
		object value = kvp.Value;
		serverInformation.Add(key, value);
}

// Where the base binary is stored
string? binDirectory = (((TomlTable)model["bin"]!)["path_to_exe"]).ToString();

// Database information
Dictionary<string, object> databaseInformation = new Dictionary<string, object>();

foreach (var kvp in ((TomlTable)model["database"]!).ToDictionary())
{
		string key = kvp.Key.ToString();
		object value = kvp.Value;
		databaseInformation.Add(key, value);
}

Console.WriteLine("Loaded Server Information....");
foreach (var pair in serverInformation)
{
		Console.WriteLine($"{pair.Key}: {pair.Value}");
}

Console.WriteLine($"\nLoaded Binary location: {binDirectory}");

Console.WriteLine("\nLoaded Database Information....");
foreach (var pair in databaseInformation)
{
		Console.WriteLine($"{pair.Key}: {pair.Value}");
}
