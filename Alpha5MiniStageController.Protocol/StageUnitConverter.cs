namespace Alpha5MiniStageController.Protocol;

public static class StageUnitConverter
{
    public const double LeadScrewMmPerRev = 20.0;
    public const double CountsPerRev = 20_000.0;
    public const double CountsPerMm = CountsPerRev / LeadScrewMmPerRev;

    public static int MmToCount(double millimeters)
    {
        return checked((int)Math.Round(millimeters * CountsPerMm, MidpointRounding.AwayFromZero));
    }

    public static double CountToMm(int count)
    {
        return count / CountsPerMm;
    }

    public static double MmPerSecToRpm(double millimetersPerSecond)
    {
        return millimetersPerSecond * 60.0 / LeadScrewMmPerRev;
    }
}
