using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace WindowsService1
{
    public class TSControl
    {
        /**/
        /// <summary> 
        /// Terminal Services API Functions,The WTSEnumerateSessions function retrieves a list of sessions on a specified terminal server, 
        /// </summary> 
        /// <param name="hServer">[in] Handle to a terminal server. Specify a handle opened by the WTSOpenServer function, or specify WTS_CURRENT_SERVER_HANDLE to indicate the terminal server on which your application is running</param> 
        /// <param name="Reserved">Reserved; must be zero</param> 
        /// <param name="Version">[in] Specifies the version of the enumeration request. Must be 1. </param> 
        /// <param name="ppSessionInfo">[out] Pointer to a variable that receives a pointer to an array of WTS_SESSION_INFO structures. Each structure in the array contains information about a session on the specified terminal server. To free the returned buffer, call the WTSFreeMemory function. 
        /// To be able to enumerate a session, you need to have the Query Information permission.</param> 
        /// <param name="pCount">[out] Pointer to the variable that receives the number of WTS_SESSION_INFO structures returned in the ppSessionInfo buffer. </param> 
        /// <returns>If the function succeeds, the return value is a nonzero value. If the function fails, the return value is zero</returns> 
        [DllImport("wtsapi32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool WTSEnumerateSessions(int hServer, int Reserved, int Version, ref long ppSessionInfo, ref int pCount);

        /**/
        /// <summary> 
        /// Terminal Services API Functions,The WTSFreeMemory function frees memory allocated by a Terminal Services function. 
        /// </summary> 
        /// <param name="pMemory">[in] Pointer to the memory to free</param> 
        [DllImport("wtsapi32.dll")]
        public static extern void WTSFreeMemory(System.IntPtr pMemory);

        /**/
        /// <summary> 
        /// Terminal Services API Functions,The WTSLogoffSession function logs off a specified Terminal Services session. 
        /// </summary> 
        /// <param name="hServer">[in] Handle to a terminal server. Specify a handle opened by the WTSOpenServer function, or specify WTS_CURRENT_SERVER_HANDLE to indicate the terminal server on which your application is running. </param> 
        /// <param name="SessionId">[in] A Terminal Services session identifier. To indicate the current session, specify WTS_CURRENT_SESSION. You can use the WTSEnumerateSessions function to retrieve the identifiers of all sessions on a specified terminal server. 
        /// To be able to log off another user's session, you need to have the Reset permission </param> 
        /// <param name="bWait">[in] Indicates whether the operation is synchronous. 
        /// If bWait is TRUE, the function returns when the session is logged off. 
        /// If bWait is FALSE, the function returns immediately.</param> 
        /// <returns>If the function succeeds, the return value is a nonzero value. 
        /// If the function fails, the return value is zero.</returns> 
        [DllImport("wtsapi32.dll")]
        public static extern bool WTSLogoffSession(int hServer, long SessionId, bool bWait);


        [DllImport("Wtsapi32.dll")]
        public static extern bool WTSQuerySessionInformation(
            System.IntPtr hServer,
            int sessionId,
            WTSInfoClass wtsInfoClass,
            out StringBuilder ppBuffer,
            out int pBytesReturned
            );

        public enum WTSInfoClass
        {
            WTSInitialProgram,
            WTSApplicationName,
            WTSWorkingDirectory,
            WTSOEMId,
            WTSSessionId,
            WTSUserName,
            WTSWinStationName,
            WTSDomainName,
            WTSConnectState,
            WTSClientBuildNumber,
            WTSClientName,
            WTSClientDirectory,
            WTSClientProductId,
            WTSClientHardwareId,
            WTSClientAddress,
            WTSClientDisplay,
            WTSClientProtocolType
        }

        /**/
        /// <summary> 
        /// The WTS_CONNECTSTATE_CLASS enumeration type contains INT values that indicate the connection state of a Terminal Services session. 
        /// </summary> 
        public enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit,
        }


        /**/
        /// <summary> 
        /// The WTS_SESSION_INFO structure contains information about a client session on a terminal server. 
        /// if the WTS_SESSION_INFO.SessionID==0, it means that the SESSION is the local logon user's session. 
        /// </summary> 
        public struct WTS_SESSION_INFO
        {
            public int SessionID;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pWinStationName;
            public WTS_CONNECTSTATE_CLASS state;
        }

        /**/
        /// <summary> 
        /// The SessionEnumeration function retrieves a list of 
        ///WTS_SESSION_INFO on a current terminal server. 
        /// </summary> 
        /// <returns>a list of WTS_SESSION_INFO on a current terminal server</returns> 
        public static WTS_SESSION_INFO[] SessionEnumeration()
        {
            //Set handle of terminal server as the current terminal server 
            int hServer = 0;
            bool RetVal;
            long lpBuffer = 0;
            int Count = 0;
            long p;
            WTS_SESSION_INFO Session_Info = new WTS_SESSION_INFO();
            WTS_SESSION_INFO[] arrSessionInfo;
            RetVal = WTSEnumerateSessions(hServer, 0, 1, ref lpBuffer, ref Count);
            arrSessionInfo = new WTS_SESSION_INFO[0];
            if (RetVal)
            {
                arrSessionInfo = new WTS_SESSION_INFO[Count];
                int i;
                p = lpBuffer;
                for (i = 0; i < Count; i++)
                {
                    arrSessionInfo[i] =
                        (WTS_SESSION_INFO)Marshal.PtrToStructure(new IntPtr(p),
                        Session_Info.GetType());
                    p += Marshal.SizeOf(Session_Info.GetType());
                }
                WTSFreeMemory(new IntPtr(lpBuffer));
            }
            else
            {
                //Insert Error Reaction Here 
            }
            return arrSessionInfo;
        }

        public TSControl()
        {
            // 
            // TODO: 在此处添加构造函数逻辑 
            // 

        }


    }
}
