using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Net;

public class ControlProtocol(IocpServer asyncSocketServer, AsyncUserToken asyncSocketUserToken) : IocpProtocol(IocpProtocolTypes.Control, asyncSocketServer, asyncSocketUserToken)
{
    enum Command
    {
        None = 0,
        Login = 1,
        Active = 2,
        GetClients = 3,
    }

    public override bool ProcessCommand(byte[] buffer, int offset, int count)
    {
        Command command = StrToCommand(CommandParser.Command);
        CommandComposer.Clear();
        CommandComposer.AddResponse();
        CommandComposer.AddCommand(CommandParser.Command);
        if (!CheckLogined(command)) //检测登录
            return CommandFail(ProtocolCode.UserHasLogined, "");
        try
        {
            return command switch
            {
                Command.Login => DoLogin(),
                Command.Active => DoActive(),
                Command.GetClients => DoGetClients(),
                _ => throw new IocpException("Unknow command: " + CommandParser.Command)
            };
        }
        catch (Exception ex)
        {
            return CommandFail(ProtocolCode.ParameterError, ex.Message);
            //Program.Logger.Error("Unknow command: " + CommandParser.Command);
            //return false;
        }
    }

    private Command StrToCommand(string command)
    {
        if (compare(ProtocolKey.Active))
            return Command.Active;
        else if (compare(ProtocolKey.Login))
            return Command.Login;
        else if (compare(ProtocolKey.GetClients))
            return Command.GetClients;
        else
            return Command.None;
        bool compare(string key)
        {
            return command.Equals(key, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    private bool CheckLogined(Command command)
    {
        if (command is Command.Login || command is Command.Active)
            return true;
        else
            return IsLogin;
    }

    public bool DoGetClients()
    {
        Server.UserTokenList.CopyTo(out var tokens);
        var values = new List<object>();
        foreach(var token in tokens)
        {
            if (token.AcceptSocket is null)
                continue;
            var sb = new StringBuilder()
                .Append(token.AcceptSocket.LocalEndPoint).Append('\t')
                .Append(token.AcceptSocket.RemoteEndPoint).Append('\t');
            if (token.Protocol is null)
                continue;
            sb.Append(token.Protocol.Type).Append('\t')
                .Append(token.Protocol.UserName).Append('\t')
                .Append(token.Protocol.ConnectTime).Append('\t')
                .Append(token.Protocol.ActiveTime).Append('\t');
            values.Add(sb.ToString());
        }
        return CommandSuccess(new() { [ProtocolKey.Item] = values });
    }
}
