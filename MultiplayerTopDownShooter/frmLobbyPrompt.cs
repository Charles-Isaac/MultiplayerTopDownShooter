using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            m_Lobby = (Math.Abs(txtLobbyName.Text.GetHashCode()) % 16383) + 49152;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        public int Lobby
        {
            get { return m_Lobby; }
        }
    }
}
