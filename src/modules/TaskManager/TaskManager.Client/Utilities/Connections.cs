using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using TaskManager.Client.Native;
using TaskManager.Shared.Dtos;

namespace TaskManager.Client.Utilities
{
    public static class Connections
    {
        public static IEnumerable<ActiveConnectionDto> GetConnections(int processId)
        {
            return GetUdpConnections(processId).Concat(GetTcpConnections(processId));
        }

        public static IEnumerable<ActiveConnectionDto> GetUdpConnections(int processId)
        {
            MIB_UDPROW_OWNER_PID[] tTable;
            var AF_INET = 2; // IP_v4
            var buffSize = 0;

            // how much memory do we need?
            var ret = NativeMethods.GetExtendedUdpTable(IntPtr.Zero, ref buffSize, true, AF_INET, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);

            if (ret != 0 && ret != 122) // 122 insufficient buffer size
                throw new Exception("bad ret on check " + ret);

            var buffTable = Marshal.AllocHGlobal(buffSize);

            try
            {
                ret = NativeMethods.GetExtendedUdpTable(buffTable, ref buffSize, true, AF_INET, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);
                if (ret != 0)
                    throw new Exception("bad ret " + ret);

                // get the number of entries in the table
                var tab = (MIB_UDPTABLE_OWNER_PID) Marshal.PtrToStructure(buffTable, typeof(MIB_UDPTABLE_OWNER_PID));
                var rowPtr = (IntPtr) ((long) buffTable + Marshal.SizeOf(tab.dwNumEntries));
                tTable = new MIB_UDPROW_OWNER_PID[tab.dwNumEntries];

                for (var i = 0; i < tab.dwNumEntries; i++)
                {
                    var udpRow = (MIB_UDPROW_OWNER_PID) Marshal.PtrToStructure(rowPtr, typeof(MIB_UDPROW_OWNER_PID));
                    tTable[i] = udpRow;
                    // next entry
                    rowPtr = (IntPtr) ((long) rowPtr + Marshal.SizeOf(udpRow));
                }
            }
            finally
            {
                // Free the Memory
                Marshal.FreeHGlobal(buffTable);
            }

            return tTable.Where(x => x.owningPid == processId).Select(x => new ActiveConnectionDto
            {
                ProtocolName = ProtocolName.Udp,
                LocalAddress = IntToIp(x.localAddr),
                LocalPort = x.LocalPort,
                RemoteAddress = IntToIp(x.remoteAddr),
                RemotePort = x.RemotePort,
                State = ConnectionState.NoError
            });
        }

        public static IEnumerable<ActiveConnectionDto> GetTcpConnections(int processId)
        {
            MIB_TCPROW_OWNER_PID[] tTable;
            var AF_INET = 2; // IP_v4
            var buffSize = 0;

            // how much memory do we need?
            var ret = NativeMethods.GetExtendedTcpTable(IntPtr.Zero, ref buffSize, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);

            if (ret != 0 && ret != 122) // 122 insufficient buffer size
                throw new Exception("bad ret on check " + ret);

            var buffTable = Marshal.AllocHGlobal(buffSize);

            try
            {
                ret = NativeMethods.GetExtendedTcpTable(buffTable, ref buffSize, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
                if (ret != 0)
                    throw new Exception("bad ret " + ret);

                // get the number of entries in the table
                var tab = (MIB_TCPTABLE_OWNER_PID) Marshal.PtrToStructure(buffTable, typeof(MIB_TCPTABLE_OWNER_PID));
                var rowPtr = (IntPtr) ((long) buffTable + Marshal.SizeOf(tab.dwNumEntries));
                tTable = new MIB_TCPROW_OWNER_PID[tab.dwNumEntries];

                for (var i = 0; i < tab.dwNumEntries; i++)
                {
                    var tcpRow = (MIB_TCPROW_OWNER_PID) Marshal.PtrToStructure(rowPtr, typeof(MIB_TCPROW_OWNER_PID));
                    tTable[i] = tcpRow;
                    // next entry
                    rowPtr = (IntPtr) ((long) rowPtr + Marshal.SizeOf(tcpRow));
                }
            }
            finally
            {
                // Free the Memory
                Marshal.FreeHGlobal(buffTable);
            }

            return tTable.Where(x => x.owningPid == processId).Select(x => new ActiveConnectionDto
            {
                ProtocolName = ProtocolName.Tcp,
                LocalAddress = IntToIp(x.localAddr),
                LocalPort = x.LocalPort,
                RemoteAddress = IntToIp(x.remoteAddr),
                RemotePort = x.RemotePort,
                State = ConvertToState(x.state)
            });
        }

        private static string IntToIp(uint raw) => new IPAddress(BitConverter.GetBytes(raw)).ToString();

        private static ConnectionState ConvertToState(uint state)
        {
            switch (state)
            {
                case MIB_TCP_STATE_ESTAB:
                    return ConnectionState.Established;
                case MIB_TCP_STATE_LISTEN:
                    return ConnectionState.Listening;
                case MIB_TCP_STATE_TIME_WAIT:
                    return ConnectionState.TimeWait;
                case MIB_TCP_STATE_CLOSE_WAIT:
                    return ConnectionState.CloseWait;
                default:
                    return ConnectionState.Other;
            }
        }

        // ReSharper disable InconsistentNaming
        private const int MIB_TCP_STATE_ESTAB = 5;
        private const int MIB_TCP_STATE_TIME_WAIT = 11;
        private const int MIB_TCP_STATE_LISTEN = 2;

        private const int MIB_TCP_STATE_CLOSE_WAIT = 8;
        // ReSharper restore InconsistentNaming
    }
}