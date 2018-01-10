
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ProntoDoc.Framework.CoreObject.DataSegment;

namespace Pdw.SharedMemoryWrapper
{
    public class MemorySegment
    {
        #region Members
        /// <summary>
        /// Store object of HeaderInfo by id
        /// </summary>
        private static Dictionary<string, DSHeaderInfo> dicHeaderInfo;
        private static int DataLength = 16300;
        private static IntPtr hDll;
        #endregion

        #region delegate funtions
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int GetBlockNumberOfHeader();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int GetHeaderLength();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void GetDataOfHeaderBlock(int index, byte[] data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void GetDataOfDataBlock(int index, byte[] data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void GetPlugin(byte[] data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int GetPluginLength();
        #endregion

        #region Methods
        public static DataSegmentInfo GetAllDomain(string DllPath)
        {
            try
            {
                DataSegmentInfo dsInfo = new DataSegmentInfo();
                hDll = NativeMethods.LoadLibrary(DllPath);

                if (hDll != IntPtr.Zero)
                {
                    IntPtr pAddressOfFunctionToCall = NativeMethods.GetProcAddress(hDll, FunctionName.GetBlockNumberOfHeader);
                    GetBlockNumberOfHeader getBlockNumOfHeader = (GetBlockNumberOfHeader)Marshal.GetDelegateForFunctionPointer(
                                                                                            pAddressOfFunctionToCall,
                                                                                            typeof(GetBlockNumberOfHeader));
                    int intBlockNo = getBlockNumOfHeader();

                    pAddressOfFunctionToCall = NativeMethods.GetProcAddress(hDll, FunctionName.GetHeaderLength);
                    GetHeaderLength getHeaderLength = (GetHeaderLength)Marshal.GetDelegateForFunctionPointer(
                                                                                            pAddressOfFunctionToCall,
                                                                                            typeof(GetHeaderLength));
                    int intHeaderLength = getHeaderLength();

                    object obj = ReadObject(1, intBlockNo, intHeaderLength, true);
                    if (obj == null) return null;

                    List<DSHeaderInfo> dsHeaderInfos = (List<DSHeaderInfo>)obj;
                    if (dicHeaderInfo == null)
                    {
                        dicHeaderInfo = new Dictionary<string, DSHeaderInfo>();
                        foreach (DSHeaderInfo dsHeader in dsHeaderInfos)
                        {
                            // get icon
                            if (dsHeader.IsIcon)
                            {
                                Icon icon = (Icon)ReadObject(dsHeader.StartBlock, dsHeader.EndBlock, dsHeader.Length, false);
                                dsInfo.Icons.Add(icon);
                                continue;
                            }

                            // get legend
                            if (dsHeader.IsLanguage)
                            {
                                LegendInfo legend = (LegendInfo)ReadObject(dsHeader.StartBlock, dsHeader.EndBlock, dsHeader.Length, false);
                                dsInfo.LegendInfos.Add(legend);
                                continue;
                            }

                            // get domain name
                            dsInfo.DSHeaderInfos.Add(dsHeader);
                            dicHeaderInfo.Add(dsHeader.Name.ToLower(), dsHeader);
                        }
                    }
                    NativeMethods.FreeLibrary(hDll);
                }

                return dsInfo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DSDomain GetDomain(string DllPath, string domain)
        {
            hDll = NativeMethods.LoadLibrary(DllPath);

            if (string.IsNullOrEmpty(domain)) throw new ArgumentNullException();
            if (dicHeaderInfo == null)
                GetAllDomain(DllPath);
            if (!dicHeaderInfo.ContainsKey(domain.ToLower())) return null;

            DSHeaderInfo header = dicHeaderInfo[domain.ToLower()];
            DSDomain oDSDomain = (DSDomain)ReadObject(header.StartBlock, header.EndBlock, header.Length, false);
            NativeMethods.FreeLibrary(hDll);

            return oDSDomain;
        }

        private static object ReadObject(int startBlock, int endBlock, int length, bool isHeader)
        {
            byte[] data = new byte[length];
            int intLastBlockLength = length % DataLength;

            for (int i = startBlock; i <= endBlock; i++)
            {
                int count = i < endBlock ? DataLength : intLastBlockLength;
                byte[] temp = new byte[count];
                if (isHeader)
                {
                    IntPtr pAddressOfFunctionToCall = NativeMethods.GetProcAddress(hDll, FunctionName.GetDataOfHeaderBlock);
                    GetDataOfHeaderBlock getDataOfHeaderBlock = (GetDataOfHeaderBlock)Marshal.GetDelegateForFunctionPointer(
                                                                                            pAddressOfFunctionToCall,
                                                                                            typeof(GetDataOfHeaderBlock));
                    getDataOfHeaderBlock(i, temp);
                }
                else
                {
                    IntPtr pAddressOfFunctionToCall = NativeMethods.GetProcAddress(hDll, FunctionName.GetDataOfDataBlock);
                    GetDataOfDataBlock getDataOfDataBlock = (GetDataOfDataBlock)Marshal.GetDelegateForFunctionPointer(
                                                                                            pAddressOfFunctionToCall,
                                                                                            typeof(GetDataOfDataBlock));
                    getDataOfDataBlock(i, temp);
                }

                Buffer.BlockCopy(temp, 0, data, (i - startBlock) * DataLength, count);

            }
            return Convertor.ConvertBinArrayToObj(data);
        }

        public static DSPlugin GetPluginInfo(string DllPath)
        {
            DSPlugin oDSPlugin = null;

            hDll = NativeMethods.LoadLibrary(DllPath);

            if (hDll != IntPtr.Zero)
            {
                IntPtr pAddressOfFunctionToCall = NativeMethods.GetProcAddress(hDll, FunctionName.GetPluginLength);
                GetPluginLength getPluginLength = (GetPluginLength)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(GetPluginLength));
                int iLength = getPluginLength();

                pAddressOfFunctionToCall = NativeMethods.GetProcAddress(hDll, FunctionName.GetPlugin);
                GetPlugin getPlugin = (GetPlugin)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(GetPlugin));
                byte[] data = new byte[iLength];
                getPlugin(data);

                oDSPlugin = (DSPlugin)Convertor.ConvertBinArrayToObj(data);
            }

            NativeMethods.FreeLibrary(hDll);

            return oDSPlugin;
        }
        #endregion

        #region helper classes
        private class NativeMethods
        {
            [DllImport("kernel32.dll")]
            public static extern IntPtr LoadLibrary(string dllToLoad);

            [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

            [DllImport("kernel32.dll")]
            public static extern bool FreeLibrary(IntPtr hModule);
        }

        private class FunctionName
        {
            /// <summary>
            /// GetBlockNumberOfHeader
            /// </summary>
            public const string GetBlockNumberOfHeader = "GetBlockNumberOfHeader";

            /// <summary>
            /// GetHeaderLength
            /// </summary>
            public const string GetHeaderLength = "GetHeaderLength";

            /// <summary>
            /// GetDataOfHeaderBlock
            /// </summary>
            public const string GetDataOfHeaderBlock = "GetDataOfHeaderBlock";

            /// <summary>
            /// GetDataOfDataBlock
            /// </summary>
            public const string GetDataOfDataBlock = "GetDataOfDataBlock";

            /// <summary>
            /// GetPluginLength
            /// </summary>
            public const string GetPluginLength = "GetPluginLength";

            /// <summary>
            /// GetPlugin
            /// </summary>
            public const string GetPlugin = "GetPlugin";
        }
        #endregion
    }
}
