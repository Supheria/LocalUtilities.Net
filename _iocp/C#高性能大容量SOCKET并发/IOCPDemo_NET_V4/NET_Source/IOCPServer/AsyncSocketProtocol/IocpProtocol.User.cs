using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Net;

partial class IocpProtocol
{
    public string UserName { get; protected set; } = "";

    public bool IsLogin { get; protected set; } = false;

    public bool DoLogin()
    {
        if (!CommandParser.GetValueAsString(ProtocolKey.UserName, out var userName) || !CommandParser.GetValueAsString(ProtocolKey.Password, out var password))
            return CommandFail(ProtocolCode.ParameterError, "");
        // TODO: modify logic of validation
        var success = password.Equals(BasicFunc.MD5String("admin"), StringComparison.CurrentCultureIgnoreCase);
        if (success)
        {
            UserName = userName;
            IsLogin = true;
            //Program.Logger.InfoFormat("{0} login success", userName);
            return CommandSuccess([]);
        }
        return CommandFail(ProtocolCode.UserOrPasswordError, "");
    }

    public bool DoActive()
    {
        return CommandSuccess([]);
    }
}
