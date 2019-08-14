using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

// Include required namespaces.
using System.Windows.Input;
using GlobalHotKey;


namespace SFMRuner
{
    class SFMHotKey2
    {
        // Create the hotkey manager.
        HotKeyManager hotKeyManager = new HotKeyManager();

        public SFMHotKey2()
        {
            // Register Ctrl+Alt+F5 hotkey. Save this variable somewhere for the further unregistering.
            var hotKey = hotKeyManager.Register(Key.F5, ModifierKeys.Control | ModifierKeys.Alt);

            // Handle hotkey presses.
            hotKeyManager.KeyPressed += HotKeyManagerPressed;
        }

        ~SFMHotKey2()
        {
            // Unregister Ctrl+Alt+F5 hotkey.
            hotKeyManager.Unregister(hotKey);

            // Dispose the hotkey manager.
            hotKeyManager.Dispose();
        }

        private void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.HotKey.Key == Key.F5)
                MessageBox.Show("Hot key pressed!");
        }

    }
}
