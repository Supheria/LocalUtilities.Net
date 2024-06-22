﻿using System;
using System.Threading;

namespace Net;

class DaemonThread
{
    System.Timers.Timer Timer { get; } = new()
    {
        Interval = 1000, // 每分钟检测一次
        Enabled = false,
        AutoReset = true,
    };

    public DaemonThread(Action processDaemon)
    {
        Timer.Elapsed += (sender, e) => processDaemon();
    }

    public void Start()
    {
        Timer.Start();
    }

    public void Stop()
    {
        Timer?.Stop();
    }
}
