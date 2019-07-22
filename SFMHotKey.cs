using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using gma.System.Windows;

namespace SFMRuner
{
    class SFMHotKey
    {        
        /// <summary>
        /// Обработка глобальных горячих клавишь Windows 
        /// </summary>

        private UserActivityHook actHook = new UserActivityHook();  // crate an instance with global hooks;
        private MainWindow _mainWindow;                             // object mainWindow

        public SFMHotKey(MainWindow mainWindow)
        {

            actHook.KeyDown += new System.Windows.Forms.KeyEventHandler(HotKey_Click);
        }

        private void HotKey_Click(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                _mainWindow.Show();
                _mainWindow.Activate();
            }
        }
    }
}
