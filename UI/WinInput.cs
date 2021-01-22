namespace Gusboy
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class WinInput
    {
        private const int IS_DOWN = 0x8000;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "It is used, it triggers the callback.")]
        private readonly System.Threading.Timer inputTimer;
        private readonly Dictionary<Keys, bool> subscribedKeys = new Dictionary<Keys, bool>();
        private readonly Action<Keys> keyDownCallback;
        private readonly Action<Keys> keyUpCallback;
        private readonly Control control;

        public WinInput(Control control, Action<Keys> keyDownCallback, Action<Keys> keyUpCallback, int interval)
        {
            this.keyDownCallback = keyDownCallback;
            this.keyUpCallback = keyUpCallback;
            this.control = control;

            this.inputTimer = new System.Threading.Timer(this.Tick, null, 0, interval);
        }

        public void Subscribe(Keys key)
        {
            this.subscribedKeys.Add(key, false);
        }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);

        private static bool IsKeyDown(Keys key) => (GetAsyncKeyState(key) & IS_DOWN) != 0;

        private void Tick(object stateInfo)
        {
            foreach (KeyValuePair<Keys, bool> key in this.subscribedKeys)
            {
                bool currentState = IsKeyDown(key.Key);

                if (currentState && !key.Value)
                {
                    this.control.Invoke(this.keyDownCallback, key.Key);
                    this.subscribedKeys[key.Key] = currentState;
                }
                else if (key.Value && !currentState)
                {
                    this.control.Invoke(this.keyUpCallback, key.Key);
                    this.subscribedKeys[key.Key] = currentState;
                }
            }
        }
    }
}
