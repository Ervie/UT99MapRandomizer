using CommandLine;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace UTMapRanodmizer
{
	internal class Program
	{
		public static IConfiguration Config { get; set; }
		public static Options CmdOptions { get; set; }

		private const string _mapsLinePrefix = "Maps[";
		private const string _iniFileSubPath = "\\System\\UnrealTournament.ini";
		private const string _mapsCatalogSubPath = "\\Maps";
		private const string _utServerRestartCommand = "./ucc.init restart";
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

			RestartUTServer();
		}

		private static ICollection<string> LoadMapsNamesFromMapFolder()
		{
			return Directory
				.GetFiles(string.Concat(Config["UTfolderPath"], _mapsCatalogSubPath), "*.unr", SearchOption.AllDirectories)
				.Select(map => Path.GetFileName(map))
				.Where(mapName => mapName.StartsWith("DM"))
				.ToList();
		}

		private static ICollection<string> LoadMapsFromCurrentRotation()
		{
			return File.ReadAllLines(string.Concat(Config["UTfolderPath"], _iniFileSubPath))
				.Where(line => line.StartsWith(_mapsLinePrefix))
				.Select(line => line.Split('=')?[1])
				.Where(mapName => mapName is { })
				.ToList();
		}

		private static ICollection<string> SelectNewRotation(ICollection<string> availableMaps, ICollection<string> oldRotation)
		{
			Random rng = new Random();
			List<string> newRotation = new List<string>();

			int maxPossibleMapCount = CmdOptions.Repeat ?
				Math.Min(_maxMapRotationSize, availableMaps.Count) :
				Math.Min(_maxMapRotationSize, availableMaps.Count - oldRotation.Count);

			while (newRotation.Count < maxPossibleMapCount && availableMaps.Any())
			{
				string selectedMap = availableMaps.ElementAt(rng.Next(0, availableMaps.Count));

				if (newRotation.Contains(selectedMap))
					continue;

				if (!CmdOptions.Repeat && oldRotation.Contains(selectedMap))
					continue;

				newRotation.Add(selectedMap);
				availableMaps.Remove(selectedMap);
			}

			return newRotation;
		}

		private static void SaveNewRotationToIniFile(ICollection<string> newRotation)
		{
			if (!(newRotation is { }) || !newRotation.Any())
				return;

			string[] configFileLines = File.ReadAllLines(string.Concat(Config["UTfolderPath"], _iniFileSubPath));

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
			using Process process = new Process()
			{
				StartInfo = new ProcessStartInfo
				{
					WindowStyle = ProcessWindowStyle.Hidden,
					FileName = "/bin/bash",
					Arguments = string.Concat(Config["UTfolderPath"], _utServerRestartCommand)
				}
			};
			process.Start();
			process.WaitForExit();
		}
	}
}