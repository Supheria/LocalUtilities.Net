namespace LocalUtilities.Net.Protocol
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ProtocolAttribute : Attribute
    {
        public ProtocolAttribute(params object[] args)
        {
            Arguments = args;
        }

        public object[] Arguments { get; private set; }
    }
}
