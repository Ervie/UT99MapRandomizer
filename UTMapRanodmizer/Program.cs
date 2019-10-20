using CommandLine;
using Microsoft.Extensions.Configuration;
using System;

namespace UTMapRanodmizer
{
	internal class Program
	{
		private static Options cmdOptions;
		private static IConfiguration config;

		private static void Main(string[] args)
		{
			config = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.Build();

			Parser.Default.ParseArguments<Options>(args)
				.WithParsed(o =>
				{
					cmdOptions = o;
					if (o.Repeat)
					{
						Console.WriteLine($"Read value: {o.Repeat}");
					}
					else
					{
						Console.WriteLine($"Read value: {o.Repeat}");
					}
				});

			Console.ReadLine();
		}
	}
}