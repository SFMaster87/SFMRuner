using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using gma.System.Windows;

namespace SFMRuner
{
    class SFMHotKey
    {
        /// <summary>
        /// Обработка глобальных горячих клавишь Windows 
        /// </summary>

        //constants Key Codes
        private const int ALT_KEY_CODE = 164;
        private const int CTRL_KEY_CODE = 162;
                
        private UserActivityHook actHook = new UserActivityHook();  // crate an instance with global hooks;
        private MainWindow _mainWindow;                             // object mainWindow

        private bool _modifyKeyTrigger = false;        

        public SFMHotKey(MainWindow mainWindow)
        {
            this._mainWindow = mainWindow;
            actHook.KeyDown += new System.Windows.Forms.KeyEventHandler(HotKey_Click);
        }

        private void HotKey_Click(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            int KeyCode = (int)e.KeyCode;
            if (e.KeyCode == Keys.Space && _modifyKeyTrigger == true)
            {
                ActivateSFMRuner();
                _modifyKeyTrigger = false;
            }

            if (KeyCode == CTRL_KEY_CODE)
            {
                _modifyKeyTrigger = true;
            }
            else
            {
                _modifyKeyTrigger = false;
            }
        }

        private void ActivateSFMRuner()
        {            
            
            while (!_mainWindow.IsVisible)
            {
                _mainWindow.Show();
            }

            while (!_mainWindow.IsActive)
            {
                _mainWindow.Activate();
            }

            
        }
    }
}
