using Alpha5MiniStageController.Protocol;

var tests = new (string Name, Action Body)[]
{
    ("CRC16 Modbus known vector", CrcKnownVector),
    ("Unit conversions", UnitConversions),
    ("ReadCurrentPosition hex", ReadCurrentPositionHex),
    ("ServoOn hex", ServoOnHex),
    ("ServoOff hex", ServoOffHex),
    ("Origin hex", OriginHex),
    ("PositionReset hex", PositionResetHex),
    ("Stop hex", StopHex),
    ("MoveRelative hex", MoveRelativeHex),
    ("JogLeftStart hex", JogLeftStartHex),
    ("JogRightStart hex", JogRightStartHex),
    ("JogStop hex", JogStopHex)
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

static void ReadCurrentPositionHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual("010300000002C40B", Alpha5Protocol.ToHexString(protocol.ReadCurrentPosition()), "ReadCurrentPosition hex");
}

static void ServoOnHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual("01050001FF00DDFA", Alpha5Protocol.ToHexString(protocol.ServoOn()), "ServoOn hex");
}

static void ServoOffHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual("0105000100009C0A", Alpha5Protocol.ToHexString(protocol.ServoOff()), "ServoOff hex");
}

static void OriginHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual("01050002FF002DFA", Alpha5Protocol.ToHexString(protocol.Origin()), "Origin hex");
}

static void PositionResetHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual("01050003FF007C3A", Alpha5Protocol.ToHexString(protocol.PositionReset()), "PositionReset hex");
}

static void StopHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual("01050004FF00CDFB", Alpha5Protocol.ToHexString(protocol.Stop()), "Stop hex");
}

static void MoveRelativeHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual("0110010000040800001388001E0001F781", Alpha5Protocol.ToHexString(protocol.MoveRelative(5.0, 10.0)), "MoveRelative hex");
}

static void JogLeftStartHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual("01100110000204001E00021EF4", Alpha5Protocol.ToHexString(protocol.JogLeftStart(10.0)), "JogLeftStart hex");
}

static void JogRightStartHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual("01100110000204001E0003DF34", Alpha5Protocol.ToHexString(protocol.JogRightStart(10.0)), "JogRightStart hex");
}

static void JogStopHex()
{
    var protocol = new Alpha5Protocol();
    AssertEqual("01050005FF009C3B", Alpha5Protocol.ToHexString(protocol.JogStop()), "JogStop hex");
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
