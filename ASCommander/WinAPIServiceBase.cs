﻿using System;
using System.Windows.Forms;

namespace ASCommander
{
    public abstract class WinApiServiceBase : IDisposable
    {
        public static string ListenerWindowCaption { get; protected set; }

        protected readonly IntPtr SpongeHandle;
        private readonly SpongeWindow Sponge;
        private static string _baseListenerWindowCaptionTitle = "AsMsgHandler";

        protected WinApiServiceBase()
        {
            ListenerWindowCaption = _baseListenerWindowCaptionTitle + "_" + Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 5);
            Sponge = new SpongeWindow();
            SpongeHandle = Sponge.Handle;
            Sponge.WndProced += LocalWndProced;
        }

        private void LocalWndProced(object sender, Message message)
        {
            WndProc(message);
        }

        protected virtual void WndProc(Message message)
        { }

        public virtual void Dispose()
        {
            Sponge.WndProced -= LocalWndProced;
        }

        private sealed class SpongeWindow : NativeWindow
        {
            public event EventHandler<Message> WndProced;

            private CreateParams windowParams;

            public SpongeWindow()
            {
                windowParams = new CreateParams();
                windowParams.Caption = ListenerWindowCaption;
                CreateHandle(windowParams);
            }

            [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert)]
            protected override void WndProc(ref Message m)
            {
                WndProced?.Invoke(this, m);
                base.WndProc(ref m);
            }
        }
    }
}
