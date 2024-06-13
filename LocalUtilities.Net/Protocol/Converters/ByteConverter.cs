namespace LocalUtilities.Net.Protocol.Converters
{
    public class ByteConverter : IValueConverter<byte>
    {
        public byte ConverterFrom(Stream stream)
        {
            return (byte)stream.ReadByte();
        }

        public byte[] ConverterTo(byte value)
        {
            return new byte[] { value };
        }
    }
}
