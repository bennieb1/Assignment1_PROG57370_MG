namespace Assignment1
{
    partial class ServerForm
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
            panelMoveCharacter = new Panel();
            listBoxServer = new ListBox();
            SuspendLayout();
            // 
            // panelMoveCharacter
            // 
            panelMoveCharacter.BackColor = SystemColors.ActiveCaptionText;
            panelMoveCharacter.Location = new Point(271, 49);
            panelMoveCharacter.Name = "panelMoveCharacter";
            panelMoveCharacter.Size = new Size(517, 389);
            panelMoveCharacter.TabIndex = 1;
            // 
            // listBoxServer
            // 
            listBoxServer.FormattingEnabled = true;
            listBoxServer.ItemHeight = 15;
            listBoxServer.Location = new Point(12, 49);
            listBoxServer.Name = "listBoxServer";
            listBoxServer.Size = new Size(226, 349);
            listBoxServer.TabIndex = 4;
            // 
            // ServerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(listBoxServer);
            Controls.Add(panelMoveCharacter);
            Name = "ServerForm";
            Text = "ServerForm";
            ResumeLayout(false);
        }

        #endregion
        private Panel panelMoveCharacter;
        private ListBox listBoxServer;
    }
}