namespace Alpha5MiniStageController.Protocol;

public static class ModbusRtuFrame
{
    public static byte[] ReadHoldingRegisters(byte slaveId, ushort startAddress, ushort registerCount)
    {
        Span<byte> payload = stackalloc byte[6];
        payload[0] = slaveId;
        payload[1] = 0x03;
        WriteUInt16BigEndian(payload[2..], startAddress);
        WriteUInt16BigEndian(payload[4..], registerCount);

        return AppendCrc(payload);
    }

    public static byte[] WriteSingleCoil(byte slaveId, ushort coilAddress, bool value)
    {
        Span<byte> payload = stackalloc byte[6];
        payload[0] = slaveId;
        payload[1] = 0x05;
        WriteUInt16BigEndian(payload[2..], coilAddress);
        WriteUInt16BigEndian(payload[4..], value ? (ushort)0xFF00 : (ushort)0x0000);

        return AppendCrc(payload);
    }

    public static byte[] WriteMultipleRegisters(byte slaveId, ushort startAddress, IReadOnlyList<ushort> values)
    {
        if (values.Count == 0)
        {
            throw new ArgumentException("At least one register value is required.", nameof(values));
        }

        if (values.Count > 123)
        {
            throw new ArgumentOutOfRangeException(nameof(values), "Modbus function 16 supports up to 123 registers per frame.");
        }

        byte[] payload = new byte[7 + (values.Count * 2)];
        payload[0] = slaveId;
        payload[1] = 0x10;
        WriteUInt16BigEndian(payload.AsSpan(2, 2), startAddress);
        WriteUInt16BigEndian(payload.AsSpan(4, 2), checked((ushort)values.Count));
        payload[6] = checked((byte)(values.Count * 2));

        for (int index = 0; index < values.Count; index++)
        {
            WriteUInt16BigEndian(payload.AsSpan(7 + (index * 2), 2), values[index]);
        }

        return AppendCrc(payload);
    }

    private static byte[] AppendCrc(ReadOnlySpan<byte> payload)
    {
        ushort crc = Crc16Modbus.Compute(payload);
        byte[] frame = new byte[payload.Length + 2];
        payload.CopyTo(frame);
        frame[^2] = Crc16Modbus.LowByte(crc);
        frame[^1] = Crc16Modbus.HighByte(crc);

        return frame;
    }

    private static void WriteUInt16BigEndian(Span<byte> destination, ushort value)
    {
        destination[0] = (byte)(value >> 8);
        destination[1] = (byte)(value & 0x00FF);
    }
}
