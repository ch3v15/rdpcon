namespace rdpcon
{
    partial class RDPAPP
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RDPAPP));
            this.RDPConsole = new System.Windows.Forms.RichTextBox();
            this.Menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ClearConsole = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // RDPConsole
            // 
            this.RDPConsole.BackColor = System.Drawing.Color.Black;
            this.RDPConsole.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RDPConsole.ContextMenuStrip = this.Menu;
            this.RDPConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RDPConsole.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.RDPConsole.ForeColor = System.Drawing.Color.White;
            this.RDPConsole.Location = new System.Drawing.Point(0, 0);
            this.RDPConsole.Name = "RDPConsole";
            this.RDPConsole.ReadOnly = true;
            this.RDPConsole.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.RDPConsole.Size = new System.Drawing.Size(514, 341);
            this.RDPConsole.TabIndex = 0;
            this.RDPConsole.Text = "";
            // 
            // Menu
            // 
            this.Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ClearConsole});
            this.Menu.Name = "Menu";
            this.Menu.Size = new System.Drawing.Size(148, 26);
            // 
            // ClearConsole
            // 
            this.ClearConsole.Name = "ClearConsole";
            this.ClearConsole.Size = new System.Drawing.Size(147, 22);
            this.ClearConsole.Text = "Clear Console";
            this.ClearConsole.Click += new System.EventHandler(this.ClearConsole_Click);
            // 
            // RDPAPP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(514, 341);
            this.Controls.Add(this.RDPConsole);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RDPAPP";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RDPAPP (Goods - 0, Left - 0)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RDPAPP_FormClosing);
            this.Menu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox RDPConsole;
        private System.Windows.Forms.ContextMenuStrip Menu;
        private System.Windows.Forms.ToolStripMenuItem ClearConsole;
    }
}