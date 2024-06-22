﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Net;

public class CommandParser
{
    public string Header { get; private set; } = "";

    public string Command { get; private set; } = "";

    public List<string> Names { get; } = [];

    public List<string> Values { get; } = [];

    public CommandParser()
    {
    }

    public bool DecodeProtocolText(string protocolText)
    {
        Header = "";
        Names.Clear();
        Values.Clear();
        int speIndex = protocolText.IndexOf(ProtocolKey.ReturnWrap);
        if (speIndex < 0)
        {
            return false;
        }
        else
        {
            string[] tmpNameValues = protocolText.Split(new string[] { ProtocolKey.ReturnWrap }, StringSplitOptions.RemoveEmptyEntries);
            if (tmpNameValues.Length < 2) //每次命令至少包括两行
                return false;
            for (int i = 0; i < tmpNameValues.Length; i++)
            {
                string[] tmpStr = tmpNameValues[i].Split(new string[] { ProtocolKey.EqualSign }, StringSplitOptions.None);
                if (tmpStr.Length > 1) //存在等号
                {
                    if (tmpStr.Length > 2) //超过两个等号，返回失败
                        return false;
                    if (tmpStr[0].Equals(ProtocolKey.Command, StringComparison.CurrentCultureIgnoreCase))
                    {
                        Command = tmpStr[1];
                    }
                    else
                    {
                        Names.Add(tmpStr[0].ToLower());
                        Values.Add(tmpStr[1]);
                    }
                }
            }
            return true;
        }
    }

    public bool GetValueAsString(string protocolKey, [NotNullWhen(true)] out string? value)
    {
        value = null;
        int index = Names.IndexOf(protocolKey.ToLower());
        if (index > -1)
        {
            value = Values[index];
            return true;
        }
        return false;
    }

    public List<string> GetValue(string protocolKey)
    {
        var result = new List<string>();
        for (int i = 0; i < Names.Count; i++)
        {
            if (protocolKey.Equals(Names[i], StringComparison.CurrentCultureIgnoreCase))
                result.Add(Values[i]);
        }
        return result;
    }

    public bool GetValueAsShort(string protocolKey, out short value)
    {
        value = 0;
        int index = Names.IndexOf(protocolKey.ToLower());
        if (index > -1)
        {
            return short.TryParse(Values[index], out value);
        }
        else
            return false;
    }

    public bool GetValueAsInt(string protocolKey, out int value)
    {
        value = 0;
        int index = Names.IndexOf(protocolKey.ToLower());
        if (index > -1)
            return int.TryParse(Values[index], out value);
        else
            return false;
    }

    public bool GetValueAsLong(string protocolKey, out long value)
    {
        value = 0;
        int index = Names.IndexOf(protocolKey.ToLower());
        if (index > -1)
            return long.TryParse(Values[index], out value);
        else
            return false;
    }

    public bool GetValueAsFloat(string protocolKey, out float value)
    {
        value = 0f;
        int index = Names.IndexOf(protocolKey.ToLower());
        if (index > -1)
            return float.TryParse(Values[index], out value);
        else
            return false;
    }

    public bool GetValueAsDouble(string protocolKey, out double value)
    {
        value = 0d;
        int index = Names.IndexOf(protocolKey.ToLower());
        if (index > -1)
            return double.TryParse(Values[index], out value);
        else
            return false;
    }
}