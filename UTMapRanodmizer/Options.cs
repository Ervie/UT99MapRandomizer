using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace UTMapRanodmizer
{
	public class Options
	{
		[Option('r', "repeat", Required = false, Default = false, HelpText = "Do maps from last rotation can be repeated in this selection")]
		public bool Repeat { get; set; }
	}
}
