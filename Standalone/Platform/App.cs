﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Medical
{
    public abstract class App : IDisposable
    {
        delegate bool OnInitDelegate();
        delegate int OnExitDelegate();
        delegate bool OnIdleDelegate();

        OnInitDelegate onInitCB;
        OnExitDelegate onExitCB;
        OnIdleDelegate onIdleCB;

        IntPtr appPtr;

        public App()
        {
            appPtr = App_create();

            onInitCB = new OnInitDelegate(OnInit);
            onExitCB = new OnExitDelegate(OnExit);
            onIdleCB = new OnIdleDelegate(OnIdle);
            App_registerDelegates(appPtr, onInitCB, onExitCB, onIdleCB);
        }

        public virtual void Dispose()
        {
            App_delete(appPtr);
            appPtr = IntPtr.Zero;
        }

        public void run()
        {
            string[] args = Environment.GetCommandLineArgs();
            App_run(appPtr, args.Length, args);
        }

        public void exit()
        {
            App_exit(appPtr);
        }

        public abstract bool OnInit();

        public abstract int OnExit();

        public abstract bool OnIdle();

        [DllImport("OSHelper")]
        private static extern IntPtr App_create();

        [DllImport("OSHelper")]
        private static extern void App_delete(IntPtr app);

        [DllImport("OSHelper")]
        private static extern void App_registerDelegates(IntPtr app, OnInitDelegate onInitCB, OnExitDelegate onExitCB, OnIdleDelegate onIdleCB);

        [DllImport("OSHelper")]
        private static extern void App_run(IntPtr app, int argc, String[] argv);

        [DllImport("OSHelper")]
        private static extern void App_exit(IntPtr app);
    }
}
