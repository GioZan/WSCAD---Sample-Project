namespace Graphics_WSCAD
{
  partial class Form1
  {
    /// <summary>
    /// Variabile di progettazione necessaria.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Pulire le risorse in uso.
    /// </summary>
    /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Codice generato da Progettazione Windows Form

    /// <summary>
    /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
    /// il contenuto del metodo con l'editor di codice.
    /// </summary>
    private void InitializeComponent()
    {
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(12, 12);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(170, 48);
      this.button1.TabIndex = 0;
      this.button1.Text = "Draw Json";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(286, 12);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(170, 48);
      this.button2.TabIndex = 1;
      this.button2.Text = "Draw Xml";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // Form1
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.ClientSize = new System.Drawing.Size(468, 112);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.MinimumSize = new System.Drawing.Size(500, 200);
      this.Name = "Form1";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Form1";
      this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
      this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
      this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.Form1_Scroll);
      this.Resize += new System.EventHandler(this.Form1_Resize);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
  }
}

