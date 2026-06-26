using Alpha5MiniStageController.Protocol;

var tests = new (string Name, Action Body)[]
{
    ("CRC16 Modbus known vector", CrcKnownVector),
    ("Unit conversions", UnitConversions),
    ("MFC ReadCurrentPosition hex", MfcReadCurrentPositionHex),
    ("MFC ServoOn hex", MfcServoOnHex),
    ("MFC ServoOff hex", MfcServoOffHex),
    ("MFC Origin hex", MfcOriginHex),
    ("MFC PositionReset sequence hex", MfcPositionResetSequenceHex),
    ("MFC Stop sequence hex", MfcStopSequenceHex),
    ("MFC PositionMove sequence hex", MfcPositionMoveSequenceHex),
    ("MFC JogLeftStart sequence hex", MfcJogLeftStartSequenceHex),
    ("MFC JogRightStart sequence hex", MfcJogRightStartSequenceHex),
    ("MFC JogStop hex", MfcJogStopHex)
};

foreach (var test in tests)
{
    test.Body();
    Console.WriteLine($"PASS {test.Name}");
}

return 0;

static void CrcKnownVector()
{
    ushort crc = Crc16Modbus.Compute("123456789"u8);
    AssertEqual((ushort)0x4B37, crc, "CRC16 Modbus known vector");
}

static void UnitConversions()
{
    AssertEqual(10_000, StageUnitConverter.MmToCount(10.0), "MmToCount");
    AssertNear(-1.25, StageUnitConverter.CountToMm(-1_250), 0.000_001, "CountToMm");
    AssertNear(30.0, StageUnitConverter.MmPerSecToRpm(10.0), 0.000_001, "MmPerSecToRpm");
}

static void MfcReadCurrentPositionHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual("01031006000220CA", Alpha5Protocol.ToHexString(protocol.ReadCurrentPosition()), "MFC ReadCurrentPosition hex");
}

static void MfcServoOnHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual("0110000000020400000001326F", Alpha5Protocol.ToHexString(protocol.ServoOn()), "MFC ServoOn hex");
}

static void MfcServoOffHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual("0110000000020400000000F3AF", Alpha5Protocol.ToHexString(protocol.ServoOff()), "MFC ServoOff hex");
}

static void MfcOriginHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual("011000000002040000001133A3", Alpha5Protocol.ToHexString(protocol.Origin(servoOn: true)), "MFC Origin servo-on hex");
    AssertEqual("0110000000020400000010F263", Alpha5Protocol.ToHexString(protocol.Origin(servoOn: false)), "MFC Origin servo-off hex");
}

static void MfcPositionResetSequenceHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual(
        "0110000000020400000041339F|0110000000020400000001326F",
        ToHexSequence(protocol.PositionResetSequence(servoOn: true)),
        "MFC PositionReset servo-on sequence hex");
    AssertEqual(
        "0110000000020400000040F25F|0110000000020400000000F3AF",
        ToHexSequence(protocol.PositionResetSequence(servoOn: false)),
        "MFC PositionReset servo-off sequence hex");
}

static void MfcStopSequenceHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual(
        "011000000002040000002133B7|011000000002040000008133CF",
        ToHexSequence(protocol.StopSequence(servoOn: true)),
        "MFC Stop servo-on sequence hex");
    AssertEqual(
        "0110000000020400000020F277|0110000000020400000080F20F",
        ToHexSequence(protocol.StopSequence(servoOn: false)),
        "MFC Stop servo-off sequence hex");
}

static void MfcPositionMoveSequenceHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual(
        "01105100000A14010000000000C35000002710000003E8000003E805EF|011000000002040000000933A9|0110000000020400000001326F",
        ToHexSequence(protocol.PositionMoveSequence(positionMm: 50.0, speed: 100.0, acceleration: 100.0, deceleration: 100.0, servoOn: true)),
        "MFC PositionMove default sequence hex");
}

static void MfcJogLeftStartSequenceHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual(
        "0110402800020400002710DBEE|0110000000020400000105323C",
        ToHexSequence(protocol.JogLeftStartSequence(speed: 100.0, servoOn: true)),
        "MFC JogLeftStart servo-on sequence hex");
    AssertEqual(
        "0110402800020400002710DBEE|0110000000020400000104F3FC",
        ToHexSequence(protocol.JogLeftStartSequence(speed: 100.0, servoOn: false)),
        "MFC JogLeftStart servo-off sequence hex");
}

static void MfcJogRightStartSequenceHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual(
        "0110402800020400002710DBEE|0110000000020400000103B23E",
        ToHexSequence(protocol.JogRightStartSequence(speed: 100.0, servoOn: true)),
        "MFC JogRightStart servo-on sequence hex");
    AssertEqual(
        "0110402800020400002710DBEE|011000000002040000010273FE",
        ToHexSequence(protocol.JogRightStartSequence(speed: 100.0, servoOn: false)),
        "MFC JogRightStart servo-off sequence hex");
}

static void MfcJogStopHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual("011000000002040000002133B7", Alpha5Protocol.ToHexString(protocol.JogStop(servoOn: true)), "MFC JogStop servo-on hex");
    AssertEqual("0110000000020400000020F277", Alpha5Protocol.ToHexString(protocol.JogStop(servoOn: false)), "MFC JogStop servo-off hex");
}

static string ToHexSequence(IEnumerable<byte[]> frames)
{
    return string.Join("|", frames.Select(frame => Alpha5Protocol.ToHexString(frame)));
}

static void AssertEqual<T>(T expected, T actual, string name)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        throw new InvalidOperationException($"{name}: expected {expected}, actual {actual}.");
    }
}

static void AssertNear(double expected, double actual, double tolerance, string name)
{
    if (Math.Abs(expected - actual) > tolerance)
    {
        throw new InvalidOperationException($"{name}: expected {expected}, actual {actual}, tolerance {tolerance}.");
    }
}
