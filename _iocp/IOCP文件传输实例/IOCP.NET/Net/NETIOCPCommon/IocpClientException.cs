using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net;

public class IocpClientException(string message) : IocpException(message)
{
}
