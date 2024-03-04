namespace Assignment1
{
    partial class ClientForm
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
            listBoxClient = new ListBox();
            BtnConnect = new Button();
            BtnExit = new Button();
            SuspendLayout();
            // 
            // panelMoveCharacter
            // 
            panelMoveCharacter.BackColor = SystemColors.ActiveCaptionText;
            panelMoveCharacter.Location = new Point(244, 57);
            panelMoveCharacter.Name = "panelMoveCharacter";
            panelMoveCharacter.Size = new Size(554, 392);
            panelMoveCharacter.TabIndex = 0;
            // 
            // listBoxClient
            // 
            listBoxClient.FormattingEnabled = true;
            listBoxClient.ItemHeight = 15;
            listBoxClient.Location = new Point(12, 89);
            listBoxClient.Name = "listBoxClient";
            listBoxClient.Size = new Size(226, 349);
            listBoxClient.TabIndex = 1;
            // 
            // BtnConnect
            // 
            BtnConnect.Location = new Point(259, 12);
            BtnConnect.Name = "BtnConnect";
            BtnConnect.Size = new Size(141, 39);
            BtnConnect.TabIndex = 3;
            BtnConnect.Text = "Connect";
            BtnConnect.UseVisualStyleBackColor = true;
            BtnConnect.Click += BtnConnect_Click_1;
            // 
            // BtnExit
            // 
            BtnExit.Location = new Point(437, 12);
            BtnExit.Name = "BtnExit";
            BtnExit.Size = new Size(130, 39);
            BtnExit.TabIndex = 4;
            BtnExit.Text = "Exit";
            BtnExit.UseVisualStyleBackColor = true;
            BtnExit.Click += BtnExit_Click_1;
            // 
            // ClientForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(BtnExit);
            Controls.Add(BtnConnect);
            Controls.Add(listBoxClient);
            Controls.Add(panelMoveCharacter);
            Name = "ClientForm";
            Text = "ClientForm";
            ResumeLayout(false);
        }

        #endregion

        private Panel panelMoveCharacter;
        private ListBox listBoxClient;
        private Button BtnConnect;
        private Button BtnExit;
    }
}