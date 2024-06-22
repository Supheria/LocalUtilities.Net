using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net;

public class CommandComposer
{
    private List<string> ProtocolText { get; } = [];

    public void Clear()
    {
        ProtocolText.Clear();
    }

    public string GetProtocolText()
    {
        string tmpStr = "";
        if (ProtocolText.Count > 0)
        {
            tmpStr = ProtocolText[0];
            for (int i = 1; i < ProtocolText.Count; i++)
            {
                tmpStr = tmpStr + ProtocolKey.ReturnWrap + ProtocolText[i];
            }
        }
        return tmpStr;
    }

    public void AddRequest()
    {
        ProtocolText.Add(ProtocolKey.OpenBracket + ProtocolKey.Request + ProtocolKey.CloseBracket);
    }

    public void AddResponse()
    {
        ProtocolText.Add(ProtocolKey.OpenBracket + ProtocolKey.Response + ProtocolKey.CloseBracket);
    }

    public void AddCommand(string commandKey)
    {
        ProtocolText.Add(ProtocolKey.Command + ProtocolKey.EqualSign + commandKey);
    }

    public void AddSuccess()
    {
        ProtocolText.Add(ProtocolKey.Code + ProtocolKey.EqualSign + ProtocolCode.Success.ToString());
    }

    public void AddFailure(int errorCode, string message)
    {
        ProtocolText.Add(ProtocolKey.Code + ProtocolKey.EqualSign + errorCode.ToString());
        ProtocolText.Add(ProtocolKey.Message + ProtocolKey.EqualSign + message);
    }

    public void AddValue(string protocolKey, string value)
    {
        ProtocolText.Add(protocolKey + ProtocolKey.EqualSign + value);
    }

    public void AddValue(string protocolKey, short value)
    {
        ProtocolText.Add(protocolKey + ProtocolKey.EqualSign + value.ToString());
    }

    public void AddValue(string protocolKey, int value)
    {
        ProtocolText.Add(protocolKey + ProtocolKey.EqualSign + value.ToString());
    }

    public void AddValue(string protocolKey, long value)
    {
        ProtocolText.Add(protocolKey + ProtocolKey.EqualSign + value.ToString());
    }

    public void AddValue(string protocolKey, Single value)
    {
        ProtocolText.Add(protocolKey + ProtocolKey.EqualSign + value.ToString());
    }

    public void AddValue(string protocolKey, double value)
    {
        ProtocolText.Add(protocolKey + ProtocolKey.EqualSign + value.ToString());
    }
}
