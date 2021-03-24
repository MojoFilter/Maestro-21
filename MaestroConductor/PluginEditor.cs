﻿using Jacobi.Vst.Core;
using Jacobi.Vst.Plugin.Framework;
using System;
using System.Drawing;

namespace Maestro.Plugin
{
    //internal interface I
    internal sealed class PluginEditor : IVstPluginEditor
    {
        public PluginEditor()
        {
            _window = new MaestroConductorWindow();
        }

        public VstKnobMode KnobMode { get; set; }

        public Rectangle Bounds
        {
            get
            {
                var wpfLoc = _window.RestoreBounds.Location;
                var loc = new Point((int)wpfLoc.X, (int)wpfLoc.Y);

                var wpfSize = _window.RestoreBounds.Size;
                var size = new Size((int)wpfSize.Width, (int)wpfSize.Height);
                return new Rectangle(loc, size);
            }
        }

        public void Close()
        {
            _window.Close();
        }

        public bool KeyDown(byte ascii, VstVirtualKey virtualKey, VstModifierKeys modifers)
        {
            return false;
        }

        public bool KeyUp(byte ascii, VstVirtualKey virtualKey, VstModifierKeys modifers)
        {
            return false;
        }

        public void Open(IntPtr hWnd)
        {
            _window.Show();
        }

        public void ProcessIdle()
        {
        }

        private MaestroConductorWindow _window;
    }
}
