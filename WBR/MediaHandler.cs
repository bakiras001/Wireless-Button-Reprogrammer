using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace WBR
{
    /// <summary>
    /// Handles the user defined key codes and custom volume up/down step
    /// </summary>
    public static class MediaHandler
    {
        // can be swapped by user to have a different functionality!
        public static byte NEXT = 0xB0;// keycode to jump to next track
        public static byte PLAY_PAUSE = 0x30;// keycode to play or pause a song
        public static byte PREV = 0xB1;// keycode to jump to prev track


        [DllImport("user32.dll")]
        public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);
        public static void first()
        {
            keybd_event(PLAY_PAUSE, 0, 1, IntPtr.Zero);
        }
        public static void second()
        {
            keybd_event(NEXT, 0, 1, IntPtr.Zero);
        }
        public static void third()
        {
            keybd_event(PREV, 0, 1, IntPtr.Zero);
        }

        private static int Clamp(int value, int min, int max)
        {
            if(value > max) {
                return max;
            } else if(value < min) {
                return min;
            } else {
                return value;
            }
        }
    }
}
