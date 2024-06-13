namespace LocalUtilities.Net.Protocol
{
    public class ProtocolContext
    {
        public ProtocolContext(ProtocolSession session, Stream stream, ProtocolConverter converter)
        {
            Session = session;
            Stream = stream;
            Converter = converter;
        }

        public ProtocolConverter Converter { get; private set; }

        public Stream Stream { get; private set; }

        public ProtocolSession Session { get; private set; }
    }
}
