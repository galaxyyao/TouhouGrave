using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TouhouSpring.PInvokes
{
    static class D3d9
    {
        [DllImport("d3d9.dll", EntryPoint = "D3DPERF_BeginEvent", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern int BeginPixEvent(uint col, string wszName);

        [DllImport("d3d9.dll", EntryPoint = "D3DPERF_EndEvent", CallingConvention = CallingConvention.Winapi)]
        public static extern int EndPixEvent();
    }
}
