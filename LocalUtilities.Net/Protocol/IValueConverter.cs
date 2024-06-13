namespace LocalUtilities.Net.Protocol
{
    public interface IValueConverter<T>
    {
        T ConverterFrom(Stream stream);

        byte[] ConverterTo(T value);
    }
}
