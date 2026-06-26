namespace Alpha5MiniStageController.Protocol;

public sealed class Alpha5Protocol
{
    public const byte DefaultSlaveId = 0x01;

    private readonly byte _slaveId;

    public Alpha5Protocol(byte slaveId = DefaultSlaveId)
    {
        if (slaveId == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(slaveId), "Modbus slave id 0 is reserved for broadcast frames.");
        }

        _slaveId = slaveId;
    }

    public byte[] ReadCurrentPosition()
    {
        return ModbusRtuFrame.ReadHoldingRegisters(_slaveId, Alpha5Registers.CurrentPositionHighWord, 2);
    }

    public byte[] ServoOn()
    {
        return WriteCommandCoil(Alpha5Coils.ServoOn, true);
    }

    public byte[] ServoOff()
    {
        return WriteCommandCoil(Alpha5Coils.ServoOn, false);
    }

    public byte[] Origin()
    {
        return PulseCommandCoil(Alpha5Coils.Origin);
    }

    public byte[] PositionReset()
    {
        return PulseCommandCoil(Alpha5Coils.PositionReset);
    }

    public byte[] Stop()
    {
        return PulseCommandCoil(Alpha5Coils.Stop);
    }

    public byte[] MoveRelative(double distanceMm, double speedMmPerSec)
    {
        int targetCount = StageUnitConverter.MmToCount(distanceMm);
        ushort rpm = checked((ushort)Math.Round(StageUnitConverter.MmPerSecToRpm(speedMmPerSec), MidpointRounding.AwayFromZero));

        return ModbusRtuFrame.WriteMultipleRegisters(
            _slaveId,
            Alpha5Registers.RelativeMoveDistanceHighWord,
            SplitInt32(targetCount)
                .Concat(new[] { rpm, Alpha5CommandCodes.MoveRelative })
                .ToArray());
    }

    public byte[] JogLeftStart(double speedMmPerSec)
    {
        return BuildJogStart(Alpha5CommandCodes.JogLeftStart, speedMmPerSec);
    }

    public byte[] JogRightStart(double speedMmPerSec)
    {
        return BuildJogStart(Alpha5CommandCodes.JogRightStart, speedMmPerSec);
    }

    public byte[] JogStop()
    {
        return PulseCommandCoil(Alpha5Coils.JogStop);
    }

    public static string ToHexString(ReadOnlySpan<byte> frame)
    {
        return Convert.ToHexString(frame).ToUpperInvariant();
    }

    private byte[] BuildJogStart(ushort commandCode, double speedMmPerSec)
    {
        ushort rpm = checked((ushort)Math.Round(StageUnitConverter.MmPerSecToRpm(speedMmPerSec), MidpointRounding.AwayFromZero));

        return ModbusRtuFrame.WriteMultipleRegisters(
            _slaveId,
            Alpha5Registers.JogSpeedRpm,
            new[] { rpm, commandCode });
    }

    private byte[] WriteCommandCoil(ushort coilAddress, bool value)
    {
        return ModbusRtuFrame.WriteSingleCoil(_slaveId, coilAddress, value);
    }

    private byte[] PulseCommandCoil(ushort coilAddress)
    {
        return WriteCommandCoil(coilAddress, true);
    }

    private static ushort[] SplitInt32(int value)
    {
        uint unsigned = unchecked((uint)value);

        return new[]
        {
            (ushort)(unsigned >> 16),
            (ushort)(unsigned & 0xFFFF)
        };
    }
}
