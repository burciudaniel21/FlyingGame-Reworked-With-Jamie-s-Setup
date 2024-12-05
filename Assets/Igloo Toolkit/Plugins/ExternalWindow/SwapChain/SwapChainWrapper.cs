using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using Igloo;
#if KLAK_SPOUT
using Klak.Spout;
#endif

public class SwapChainWrapper : MonoBehaviour
{
    const string dll = "HoneywellSwapChain";

    [DllImport(dll)]
    public static extern void CreateWin(int width, int height);

    [DllImport(dll)]
    public static extern void CloseWin();

    [DllImport(dll)]
    public static extern IntPtr CopyTexture(IntPtr texPtr);
}
