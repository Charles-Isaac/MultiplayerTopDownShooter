using System;
using System.Windows.Forms;

namespace ClonesEngine
{
    public partial class frmLobbyPrompt : Form
    {
        public frmLobbyPrompt()
        {
            InitializeComponent();
        }
        private int m_Lobby;
        private void btnOK_Click(object sender, EventArgs e)
        {
            m_Lobby = (Math.Abs(txtLobbyName.Text.GetHashCode()) % 0x3FFF/*16383*/) + 0xC000 /*49152*/;
            DialogResult = DialogResult.OK;
            Close();
        } 

        public int Lobby
        {
            get { return m_Lobby; }
        }

        private void txtLobbyName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Return)
            {
                m_Lobby = (Math.Abs(txtLobbyName.Text.GetHashCode()) % 16383) + 49152;
                DialogResult = DialogResult.OK;
                Close();
            }
            
        }
        
    }
}
