using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;


namespace Test_SystemInfoEx
{
    class Program
    {
        [StructLayout(LayoutKind.Sequential)]
        struct MIB_IF_ROW2 // sizeof(1352 + 4)
        {
            public long InterfaceLuid;
            public int InterfaceIndex;
            public byte[] GUID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0)] // 514
            public string Alias;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0)]
            public string Description;
            public int PhysicalAddressLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] PhysicalAddress;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] PermanentPhysicalAddress;
            public int Mtu;
            public int Type;
            public int TunnelType;
            public int MediaType;
            public int PhysicalMediumType;
            public int AccessType;
            public int DirectionType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] InterfaceAndOperStatusFlags;
            public int OperStatus;
            public int AdminStatus;
            public int MediaConnectState;
            public byte[] NetworkGuid;
            public int ConnectionType;
            public long TransmitLinkSpeed;
            public long ReceiveLinkSpeed;
            public long InOctets;
            public long InUcastPkts;
            public long InNUcastPkts;
            public long InDiscards;
            public long InErrors;
            public long InUnknownProtos;
            public long InUcastOctets;
            public long InMulticastOctets;
            public long InBroadcastOctets;
            public long OutOctets;
            public long OutUcastPkts;
            public long OutNUcastPkts;
            public long OutDiscards;
            public long OutErrors;
            public long OutUcastOctets;
            public long OutMulticastOctets;
            public long OutBroadcastOctets;
            public long OutQLen;
        }
 
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_IF_TABLE2
        {
            public int NumEntries;
            public MIB_IF_ROW2[] Table;
        }
        
        [DllImportAttribute("Iphlpapi.dll")]
        static extern int GetIfTable2(ref int Table);
 
        [DllImportAttribute("Iphlpapi.dll")]
        static extern int FreeMibTable(int Table);
 
        [DllImportAttribute("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        static extern int RtlMoveMemory(ref int Destination, int Source, int Length);
 
        [DllImportAttribute("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        static extern int RtlMoveMemory(ref long Destination, int Source, int Length);
 
        [DllImportAttribute("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        static extern int RtlMoveMemory(int Destination, int Source, int Length);
 
        [DllImportAttribute("kernel32.dll")]
        static extern int SetHandleCount(byte[] Bytes);

        public static unsafe  MIB_IF_TABLE2 GetMIB2Interface2()
        {
            int ptr_Mit2 =0;
            MIB_IF_TABLE2 s_Mit2 = new MIB_IF_TABLE2(); // create empty mit2.
            if (GetIfTable2(ref ptr_Mit2) != 0)
                return s_Mit2;
            if (RtlMoveMemory(ref s_Mit2.NumEntries, ptr_Mit2, 4)>0 && s_Mit2.NumEntries > 0)
            {
                ptr_Mit2 = ptr_Mit2 + 8; // ptr_Mit2 += 8; offset pointer to MIB_IF_TABLE2::Table
                int dwBufferSize = 1352; // sizeof(Win32Native.MIB_IF_ROW2);
                s_Mit2.Table = new MIB_IF_ROW2[s_Mit2.NumEntries];
                for (int i = 0; i < s_Mit2.NumEntries; i++)
                {
                    IntPtr ptr = (IntPtr)(ptr_Mit2 + (i * dwBufferSize));
                    s_Mit2.Table[i] = (MIB_IF_ROW2)
                        Marshal.PtrToStructure(ptr, typeof(MIB_IF_ROW2));
                }
            }
            return s_Mit2;
        }

        private  static MIB_IF_TABLE2 GetMIB2Interface()
        {
            int pTable = 0;
            byte[] bin = null;
            MIB_IF_TABLE2 MIB_IF_TABLE2 = new MIB_IF_TABLE2();
            int ret = GetIfTable2(ref pTable);
            if(ret != 0) {
                return MIB_IF_TABLE2;
            }
            int leng = 1352;
            RtlMoveMemory(ref MIB_IF_TABLE2.NumEntries, pTable, 4);
            MIB_IF_TABLE2.Table = new MIB_IF_ROW2[MIB_IF_TABLE2.NumEntries];
            int Address = pTable + 8;
            for (int i = 0; i < MIB_IF_TABLE2.NumEntries; i++) {
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].InterfaceLuid, Address + (i) * leng, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].InterfaceIndex, Address + (i) * leng + 8, 4);
                MIB_IF_TABLE2.Table[i].GUID = new byte[16];
                RtlMoveMemory(SetHandleCount(MIB_IF_TABLE2.Table[i].GUID), Address + (i) * leng + 12, 16);
                bin = new byte[514];
                RtlMoveMemory(SetHandleCount(bin), Address + (i) * leng + 28, 514);
                MIB_IF_TABLE2.Table[i].Alias = Encoding.Unicode.GetString(bin);
                bin = new byte[514];
                RtlMoveMemory(SetHandleCount(bin), Address + (i) * leng + 542, 514);
                MIB_IF_TABLE2.Table[i].Description = Encoding.Unicode.GetString(bin);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].PhysicalAddressLength, Address + (i) * leng + 1056, 4);
                MIB_IF_TABLE2.Table[i].PhysicalAddress = new byte[32];
                RtlMoveMemory(SetHandleCount(MIB_IF_TABLE2.Table[i].PhysicalAddress), Address + (i) * leng + 1060, 32);
                MIB_IF_TABLE2.Table[i].PermanentPhysicalAddress = new byte[32];
                RtlMoveMemory(SetHandleCount(MIB_IF_TABLE2.Table[i].PermanentPhysicalAddress), Address + (i) * leng + 1092, 32);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].Mtu, Address + (i) * leng + 1124, 4);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].Type, Address + (i) * leng + 1128, 4);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].TunnelType, Address + (i) * leng + 1132, 4);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].MediaType, Address + (i) * leng + 1136, 4);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].PhysicalMediumType, Address + (i) * leng + 1140, 4);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].AccessType, Address + (i) * leng + 1144, 4);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].DirectionType, Address + (i) * leng + 1148, 4);
                MIB_IF_TABLE2.Table[i].InterfaceAndOperStatusFlags = new byte[8];
                RtlMoveMemory(SetHandleCount(MIB_IF_TABLE2.Table[i].InterfaceAndOperStatusFlags), Address + (i) * leng + 1152, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].OperStatus, Address + (i) * leng + 1160, 4);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].AdminStatus, Address + (i) * leng + 1164, 4);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].MediaConnectState, Address + (i) * leng + 1168, 4);
                MIB_IF_TABLE2.Table[i].NetworkGuid = new byte[16];
                RtlMoveMemory(SetHandleCount(MIB_IF_TABLE2.Table[i].NetworkGuid), Address + (i) * leng + 1172, 16);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].ConnectionType, Address + (i) * leng + 1188, 4);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].TransmitLinkSpeed, Address + (i) * leng + 1192, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].ReceiveLinkSpeed, Address + (i) * leng + 1200, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].InOctets, Address + (i) * leng + 1208, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].InUcastPkts, Address + (i) * leng + 1216, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].InNUcastPkts, Address + (i) * leng + 1224, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].InDiscards, Address + (i) * leng + 1232, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].InErrors, Address + (i) * leng + 1240, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].InUnknownProtos, Address + (i) * leng + 1248, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].InUcastOctets, Address + (i) * leng + 1256, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].InMulticastOctets, Address + (i) * leng + 1264, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].InBroadcastOctets, Address + (i) * leng + 1272, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].OutOctets, Address + (i) * leng + 1280, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].OutUcastPkts, Address + (i) * leng + 1288, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].OutNUcastPkts, Address + (i) * leng + 1296, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].OutDiscards, Address + (i) * leng + 1304, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].OutErrors, Address + (i) * leng + 1312, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].OutUcastOctets, Address + (i) * leng + 1320, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].OutMulticastOctets, Address + (i) * leng + 1328, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].OutBroadcastOctets, Address + (i) * leng + 1336, 8);
                RtlMoveMemory(ref MIB_IF_TABLE2.Table[i].OutQLen, Address + (i) * leng + 1344, 8);
            }
            FreeMibTable(pTable);
            return MIB_IF_TABLE2;
        }

         
        static void Main(string[] args)
        {
            while (true)
            {
                MIB_IF_TABLE2 MIB_IF_TABLE2 = GetMIB2Interface2();
                for (int i = 0; i < MIB_IF_TABLE2.NumEntries; i++)
                {
                    Console.WriteLine("别名->"+MIB_IF_TABLE2.Table[i].Alias + " Mtu->" + MIB_IF_TABLE2.Table[i].Mtu.ToString() +
                                      " 上传->" +
                                      MIB_IF_TABLE2.Table[i].OutOctets.ToString() + " 下载->" +
                                      MIB_IF_TABLE2.Table[i].InOctets.ToString() + "详细->" +
                                      MIB_IF_TABLE2.Table[i].Description);

                }
                Thread.Sleep(1000);
            }

            Console.WriteLine("Hello World!");
        }
    }
}