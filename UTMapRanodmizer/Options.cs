using CommandLine;

namespace UTMapRanodmizer
{
	public class Options
	{
		[Option('r', "repeat", Required = false, Default = false, HelpText = "Do maps from last rotation can be repeated in this selection")]
		public bool Repeat { get; set; }

		[Option('e', "excluded-maps-file-path", Required = false, Default = "", HelpText = "Path to file with names of maps which should be excluded from rotation. If not chosen, no map will be excluded.")]
		public string ExcludedMapsFilePath { get; set; }

		[Option('m', "mode", Required = false, Default = "DM", HelpText = "Selects mode, after which maps will be selected should be abbreviation (DM, CTF, DOM, etc.). If not chosen will start as Deathmatch.")]
		public string Mode { get; set; }
	}
}