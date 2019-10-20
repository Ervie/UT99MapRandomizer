using CommandLine;
using Microsoft.Extensions.Configuration;
using System;

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



			Console.ReadLine();
		}
	}
}