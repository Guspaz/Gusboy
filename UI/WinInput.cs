namespace Gusboy
{
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public static class WinInput
    {
        private const int IS_DOWN = 0x8000;

        public static byte PollKeys10()
        {
            byte result = 0;

            // A button
            if (!IsKeyDown(Keys.A))
            {
                result |= 0x01;
            }

            // B button
            if (!IsKeyDown(Keys.B))
            {
                result |= 0x02;
            }

            // Select button
            if (!IsKeyDown(Keys.Space))
            {
                result |= 0x04;
            }

            // Select button
            if (!IsKeyDown(Keys.Enter))
            {
                result |= 0x08;
            }

            return result;
        }

        public static byte PollKeys20()
        {
            byte result = 0;

            // Right button
            if (!IsKeyDown(Keys.Right))
            {
                result |= 0x01;
            }

            // Left button
            if (!IsKeyDown(Keys.Left))
            {
                result |= 0x02;
            }

            // Up button
            if (!IsKeyDown(Keys.Up))
            {
                result |= 0x04;
            }

            // Down button
            if (!IsKeyDown(Keys.Down))
            {
                result |= 0x08;
            }

            return result;
        }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);

        private static bool IsKeyDown(Keys key) => (GetAsyncKeyState(key) & IS_DOWN) > 0;
    }
}
