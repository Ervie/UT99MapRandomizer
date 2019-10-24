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
				.Where(mapName => mapName is {})
				.ToList();
		}
	}
}