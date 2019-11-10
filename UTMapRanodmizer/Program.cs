using CommandLine;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UTMapRanodmizer
{
	internal class Program
	{
		public static IConfiguration Config { get; set; }
		public static Options CmdOptions { get; set; }

		private const string _mapsLinePrefix = "Maps[";
		private const int _maxMapRotationSize = 32;

		private static void Main(string[] args)
		{
			Config = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.Build();

			Parser.Default.ParseArguments<Options>(args)
				.WithParsed(o =>
				{
					CmdOptions = o;
					if (o.Repeat)
					{
						Console.WriteLine($"Maps will may be repeated.");
					}
				});

			var availableMaps = LoadMapsNamesFromMapFolder();

			var mapsFromCurrentRotation = LoadMapsFromCurrentRotation();

			var newRotation = SelectNewRotation(availableMaps, mapsFromCurrentRotation);

			SaveNewRotationToIniFile(newRotation);

			

			Console.ReadLine();
		}

		private static ICollection<string> LoadMapsNamesFromMapFolder()
		{
			return Directory
				.GetFiles(Config["mapsCatalogPath"], "*.unr", SearchOption.AllDirectories)
				.Select(map => Path.GetFileName(map))
				.Where(mapName => mapName.StartsWith("DM"))
				.ToList();
		}

		private static ICollection<string> LoadMapsFromCurrentRotation()
		{
			return File.ReadAllLines(Config["iniFilePath"])
				.Where(line => line.StartsWith(_mapsLinePrefix))
				.Select(line => line.Split('=')?[1])
				.Where(mapName => mapName is { })
				.ToList();
		}

		private static ICollection<string> SelectNewRotation(ICollection<string> availableMaps, ICollection<string> oldRotation)
		{
			Random rng = new Random();
			List<string> newRotation = new List<string>();

			int maxPossibleMapCount = Math.Min(_maxMapRotationSize, availableMaps.Count);

			while (newRotation.Count < maxPossibleMapCount)
			{
				string selectedMap = availableMaps.ElementAt(rng.Next(0, availableMaps.Count));
				newRotation.Add(selectedMap);
				availableMaps.Remove(selectedMap);
			}

			return newRotation;
		}

		private static void SaveNewRotationToIniFile(ICollection<string> newRotation)
		{
			if (!(newRotation is { }) || !newRotation.Any())
				return;

			string[] configFileLines = File.ReadAllLines(Config["iniFilePath"]);

			for (int i = 0; i < configFileLines.Length; i++)
			{
				if (configFileLines[i].StartsWith(_mapsLinePrefix))
				{
					configFileLines[i] = string.Concat(configFileLines[i].Split('=')[0], "=", newRotation.ElementAt(i % _maxMapRotationSize) ?? string.Empty);
				}
			}

			File.WriteAllLines(Config["iniFilePath"], configFileLines);
		}

		private static void RestartUTServer()
		{

		}
	}
}