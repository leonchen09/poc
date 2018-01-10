using System.Runtime.InteropServices;

namespace Pdw.Core
{
    public class NativeMethods
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        /// <summary>
        /// Send a key broad event to window API
        /// </summary>
        /// <param name="keyCode">Key code</param>
        /// <param name="dwFlags">0: release, 2: press</param>
        public static void SendKey(byte keyCode, uint dwFlags)
        {
            keybd_event(keyCode, 0, dwFlags, 0);
        }
    }
}
