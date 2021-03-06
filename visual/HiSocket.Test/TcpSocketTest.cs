﻿using System;
using HiSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HiSocket.Test
{
    [TestClass]
    public class TcpSocketTest
    {
        private TcpServer _server;
        [TestInitialize]
        public void Init()
        {
            _server = new TcpServer();
        }
        [TestCleanup]
        public void Cleanup()
        {
            _server.Close();
            _server = null;
        }

        [TestMethod]
        public void TestEvent()
        {
            var tcp = new TcpSocket();
            bool isConnecting = false;
            tcp.OnConnecting += () => { isConnecting = true; };
            bool isConnected = false;
            tcp.OnConnected += () => { isConnected = true; };
            bool isDisconnected = false;
            tcp.OnDisconnected += () => { isDisconnected = true; };
            tcp.Connect(Common.GetIpEndPoint());
            Common.WaitConnect(tcp);
            tcp.DisConnect();
            Assert.IsTrue(isConnecting);
            Assert.IsTrue(isConnected);
            Assert.IsTrue(isDisconnected);
        }

        [TestMethod]
        public void TestSendReceive()
        {
            var tcp = new TcpSocket();
            tcp.Connect(Common.GetIpEndPoint());
            Common.WaitConnect(tcp);
            int length = 0;
            tcp.OnSocketReceive += (x) =>
            {
                length = x.Length;
            };
            tcp.Send(new byte[10]);
            Common.WaitValue(ref length, 10, 3000);
            tcp.DisConnect();
            Assert.AreEqual(length, 10);
        }
        [TestMethod]
        public void TestLargeMessage()
        {
            var tcp = new TcpSocket();
            tcp.Connect(Common.GetIpEndPoint());
            Common.WaitConnect(tcp);
            int length = 0;
            tcp.OnSocketReceive += (x) =>
            {
                length += x.Length;
            };
            tcp.Send(new byte[1 << 20]);
            Common.WaitValue(ref length, 1 << 20, 10000);
            tcp.DisConnect();
            Console.WriteLine(length);
            Assert.AreEqual(length, 1 << 20);
        }
    }
}
