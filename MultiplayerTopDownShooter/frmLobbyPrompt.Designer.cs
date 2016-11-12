namespace ClonesEngine
{
    partial class frmLobbyPrompt
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtLobbyName = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtLobbyName
            // 
            this.txtLobbyName.Location = new System.Drawing.Point(73, 68);
            this.txtLobbyName.Name = "txtLobbyName";
            this.txtLobbyName.Size = new System.Drawing.Size(100, 20);
            this.txtLobbyName.TabIndex = 0;
            this.txtLobbyName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtLobbyName_KeyUp);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(84, 138);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmLobbyPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtLobbyName);
            this.Name = "frmLobbyPrompt";
            this.Text = "LobbyPrompt";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLobbyName;
        private System.Windows.Forms.Button btnOK;
    }
}