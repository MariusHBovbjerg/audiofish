using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Audio_based_trove_fisher;

public class WindowInteracter
{
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;

    private const uint MAPVK_VK_TO_VSC_EX = 0x04;
    
    private Process _process;
    
    public WindowInteracter(string processName)
    {
        var processList = Process.GetProcessesByName(processName);

        _process = processList.First();
    }
    
    public void SendKeystroke(VKeys key)
    {
        PostMessage(_process.MainWindowHandle, WM_KEYDOWN, (int)VKeys.KEY_F, GetLParam(1, key, 0, 0, 0, 0));
        PostMessage(_process.MainWindowHandle, WM_KEYUP, (int)VKeys.KEY_F, GetLParam(1, key, 0, 0, 0, 0));
    }
    
    private static uint GetLParam(short repeatCount, VKeys key, byte extended, byte contextCode, byte previousState,
        byte transitionState)
    {
        var lParam = (uint)repeatCount;
        var scanCode = GetScanCode(key);
        lParam += scanCode * 0x10000;
        lParam += (uint)(extended * 0x1000000);
        lParam += (uint)(contextCode * 2 * 0x10000000);
        lParam += (uint)(previousState * 4 * 0x10000000);
        lParam += (uint)(transitionState * 8 * 0x10000000);
        return lParam;
    }
    
    public Task<bool> AwaitKeyPress(VKeys key)
    {
        short state;
        var keyPressed = false;

        while (true)
        {
            state = GetKeyState((int)key);

            var hWnd = GetForegroundWindow();
            uint processId;
            GetWindowThreadProcessId(hWnd, out processId);

            var process = Process.GetProcessById((int)processId);
            
            if (process.ProcessName == _process.ProcessName && (state & 0x8000) == 0x8000)
            {
                if (keyPressed) continue;
                return Task.FromResult(true);
            }
        }
    }

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);

    [DllImport("user32.dll")]
    static extern short GetKeyState(int nVirtKey);

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    private static extern uint MapVirtualKey(uint uCode, uint uMapType);

    private static uint GetScanCode(VKeys key)
    {
        return MapVirtualKey((uint)key, MAPVK_VK_TO_VSC_EX);
    }
}