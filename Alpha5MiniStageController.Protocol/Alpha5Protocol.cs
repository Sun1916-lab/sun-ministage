namespace Alpha5MiniStageController.Protocol;

public sealed class Alpha5Protocol
{
    public const byte DefaultSlaveId = 0x01;
    public const double DefaultMoveAcceleration = 100.0;
    public const double DefaultMoveDeceleration = 100.0;

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
        return WriteControl(Alpha5CommandCodes.ServoOn);
    }

    public byte[] ServoOff()
    {
        return WriteControl(0);
    }

    public byte[] Origin(bool servoOn = true)
    {
        return WriteControl(WithServoState(Alpha5CommandCodes.Origin, servoOn));
    }

    public byte[] PositionReset(bool servoOn = true)
    {
        return WriteControl(WithServoState(Alpha5CommandCodes.PositionReset, servoOn));
    }

    public byte[][] PositionResetSequence(bool servoOn = true)
    {
        return new[]
        {
            PositionReset(servoOn),
            WriteControl(servoOn ? Alpha5CommandCodes.ServoOn : 0)
        };
    }

    public byte[] Stop(bool servoOn = true)
    {
        return WriteControl(WithServoState(Alpha5CommandCodes.Stop, servoOn));
    }

    public byte[][] StopSequence(bool servoOn = true)
    {
        return new[]
        {
            Stop(servoOn),
            WriteControl(WithServoState(Alpha5CommandCodes.PositionCancel, servoOn))
        };
    }

    public byte[] MoveRelative(double distanceMm, double speedMmPerSec)
    {
        return PositionMove(distanceMm, speedMmPerSec);
    }

    public byte[] PositionMove(
        double positionMm,
        double speed,
        double acceleration = DefaultMoveAcceleration,
        double deceleration = DefaultMoveDeceleration)
    {
        int positionCount = StageUnitConverter.MmToCount(positionMm);
        int speedCommand = ScaleCommandValue(speed, 100.0);
        int accelerationCommand = ScaleCommandValue(acceleration, 10.0);
        int decelerationCommand = ScaleCommandValue(deceleration, 10.0);
        ushort[] positionWords = SplitInt32(positionCount);
        ushort[] speedWords = SplitInt32(speedCommand);
        ushort[] accelerationWords = SplitInt32(accelerationCommand);
        ushort[] decelerationWords = SplitInt32(decelerationCommand);

        return ModbusRtuFrame.WriteMultipleRegisters(
            _slaveId,
            Alpha5Registers.PositionMoveParameterHighWord,
            new[]
            {
                (ushort)0x0100,
                (ushort)0x0000,
                positionWords[0],
                positionWords[1],
                speedWords[0],
                speedWords[1],
                accelerationWords[0],
                accelerationWords[1],
                decelerationWords[0],
                decelerationWords[1]
            });
    }

    public byte[][] PositionMoveSequence(
        double positionMm,
        double speed,
        double acceleration = DefaultMoveAcceleration,
        double deceleration = DefaultMoveDeceleration,
        bool servoOn = true)
    {
        return new[]
        {
            PositionMove(positionMm, speed, acceleration, deceleration),
            WriteControl(WithServoState(Alpha5CommandCodes.PositionMoveStart, servoOn)),
            WriteControl(servoOn ? Alpha5CommandCodes.ServoOn : 0)
        };
    }

    public byte[] JogLeftStart(double speed, bool servoOn = true)
    {
        return WriteControl(WithServoState(Alpha5CommandCodes.JogLeftStart, servoOn));
    }

    public byte[][] JogLeftStartSequence(double speed, bool servoOn = true)
    {
        return new[]
        {
            JogSpeed(speed),
            JogLeftStart(speed, servoOn)
        };
    }

    public byte[] JogRightStart(double speed, bool servoOn = true)
    {
        return WriteControl(WithServoState(Alpha5CommandCodes.JogRightStart, servoOn));
    }

    public byte[][] JogRightStartSequence(double speed, bool servoOn = true)
    {
        return new[]
        {
            JogSpeed(speed),
            JogRightStart(speed, servoOn)
        };
    }

    public byte[] JogStop(bool servoOn = true)
    {
        return Stop(servoOn);
    }

    public static string ToHexString(ReadOnlySpan<byte> frame)
    {
        return Convert.ToHexString(frame).ToUpperInvariant();
    }

    public byte[] JogSpeed(double speed)
    {
        int speedCommand = ScaleCommandValue(speed, 100.0);

        return ModbusRtuFrame.WriteMultipleRegisters(
            _slaveId,
            Alpha5Registers.JogSpeedHighWord,
            SplitInt32(speedCommand));
    }

    private byte[] WriteControl(uint controlValue)
    {
        return ModbusRtuFrame.WriteMultipleRegisters(
            _slaveId,
            Alpha5Registers.ControlHighWord,
            SplitUInt32(controlValue));
    }

    private static uint WithServoState(uint commandValue, bool servoOn)
    {
        return servoOn ? commandValue | Alpha5CommandCodes.ServoOn : commandValue;
    }

    private static int ScaleCommandValue(double value, double factor)
    {
        return checked((int)Math.Round(value * factor, MidpointRounding.AwayFromZero));
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

    private static ushort[] SplitUInt32(uint value)
    {
        return new[]
        {
            (ushort)(value >> 16),
            (ushort)(value & 0xFFFF)
        };
    }
}
