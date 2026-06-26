namespace Alpha5MiniStageController.Protocol;

public static class Crc16Modbus
{
    public static ushort Compute(ReadOnlySpan<byte> data)
    {
        ushort crc = 0xFFFF;

        foreach (byte item in data)
        {
            crc ^= item;

            for (int bit = 0; bit < 8; bit++)
            {
                bool carry = (crc & 0x0001) != 0;
                crc >>= 1;

                if (carry)
                {
                    crc ^= 0xA001;
                }
            }
        }

        return crc;
    }

    public static byte LowByte(ushort crc)
    {
        return (byte)(crc & 0x00FF);
    }

    public static byte HighByte(ushort crc)
    {
        return (byte)(crc >> 8);
    }
}
