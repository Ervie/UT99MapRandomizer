using CommandLine;
using System;

namespace UTMapRanodmizer
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Parser.Default.ParseArguments<Options>(args)
				.WithParsed(o =>
				{
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