namespace Apple.CoreHaptics
{
	internal class CHHapticPlayerConfiguration
	{
		public CHHapticPattern Ahap { get; }
		public CHHapticPatternPlayer Player { get; }

		public CHHapticPlayerConfiguration(CHHapticPattern ahap, CHHapticPatternPlayer player)
		{
			Ahap = ahap;
			Player = player;
		}
	}
}
