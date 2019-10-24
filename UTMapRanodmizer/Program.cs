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
				.Where(line => line.StartsWith("Maps["))
				.Select(line => line.Split('=')?[1])
				.Where(mapName => mapName is { })
				.ToList();
		}

		private static ICollection<string> SelectNewRotation(ICollection<string> availableMaps, ICollection<string> oldRotation)
		{
			Random rng = new Random();
			List<string> newRotation = new List<string>();

			int maxPossibleMapCount = Math.Min(32, availableMaps.Count);

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
		}
	}
}