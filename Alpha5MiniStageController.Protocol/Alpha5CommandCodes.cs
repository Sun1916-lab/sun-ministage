namespace Alpha5MiniStageController.Protocol;

public static class Alpha5CommandCodes
{
    public const uint ServoOn = 0x0000_0001;
    public const uint PositionMoveStart = 0x0000_0008;
    public const uint Origin = 0x0000_0010;
    public const uint Stop = 0x0000_0020;
    public const uint PositionReset = 0x0000_0040;
    public const uint PositionCancel = 0x0000_0080;
    public const uint JogRightStart = 0x0000_0102;
    public const uint JogLeftStart = 0x0000_0104;
}
