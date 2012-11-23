namespace LibClassicBot_GUI
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.button1 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.passwordBox = new System.Windows.Forms.TextBox();
			this.userBox = new System.Windows.Forms.TextBox();
			this.addBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.logBox = new System.Windows.Forms.RichTextBox();
			this.txtMessage = new System.Windows.Forms.TextBox();
			this.btnMessage = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(12, 238);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(364, 23);
			this.progressBar1.TabIndex = 0;
			// 
			// button1
			// 
			this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button1.Location = new System.Drawing.Point(301, 87);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 4;
			this.button1.Text = "Connect";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Button1Click);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(12, 217);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(364, 18);
			this.label1.TabIndex = 9;
			this.label1.Text = "Map loading progress";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// passwordBox
			// 
			this.passwordBox.Location = new System.Drawing.Point(113, 35);
			this.passwordBox.MaxLength = 64;
			this.passwordBox.Name = "passwordBox";
			this.passwordBox.PasswordChar = '*';
			this.passwordBox.Size = new System.Drawing.Size(263, 20);
			this.passwordBox.TabIndex = 2;
			// 
			// userBox
			// 
			this.userBox.Location = new System.Drawing.Point(113, 9);
			this.userBox.MaxLength = 16;
			this.userBox.Name = "userBox";
			this.userBox.Size = new System.Drawing.Size(263, 20);
			this.userBox.TabIndex = 1;
			// 
			// addBox
			// 
			this.addBox.Location = new System.Drawing.Point(113, 61);
			this.addBox.MaxLength = 70;
			this.addBox.Name = "addBox";
			this.addBox.Size = new System.Drawing.Size(263, 20);
			this.addBox.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 12);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 23);
			this.label2.TabIndex = 6;
			this.label2.Text = "Username";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 38);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 23);
			this.label3.TabIndex = 7;
			this.label3.Text = "Password";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(12, 64);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 23);
			this.label4.TabIndex = 8;
			this.label4.Text = "Address";
			// 
			// logBox
			// 
			this.logBox.BackColor = System.Drawing.Color.Black;
			this.logBox.Location = new System.Drawing.Point(382, 9);
			this.logBox.Name = "logBox";
			this.logBox.ReadOnly = true;
			this.logBox.Size = new System.Drawing.Size(346, 252);
			this.logBox.TabIndex = 5;
			this.logBox.Text = "";
			// 
			// txtMessage
			// 
			this.txtMessage.Location = new System.Drawing.Point(12, 194);
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.Size = new System.Drawing.Size(262, 20);
			this.txtMessage.TabIndex = 10;
			// 
			// btnMessage
			// 
			this.btnMessage.Location = new System.Drawing.Point(280, 194);
			this.btnMessage.Name = "btnMessage";
			this.btnMessage.Size = new System.Drawing.Size(96, 23);
			this.btnMessage.TabIndex = 11;
			this.btnMessage.Text = "Send message";
			this.btnMessage.UseVisualStyleBackColor = true;
			this.btnMessage.Click += new System.EventHandler(this.BtnMessageClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(740, 273);
			this.Controls.Add(this.btnMessage);
			this.Controls.Add(this.txtMessage);
			this.Controls.Add(this.logBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.addBox);
			this.Controls.Add(this.userBox);
			this.Controls.Add(this.passwordBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.progressBar1);
			this.Name = "MainForm";
			this.Text = "LibClassicBotGUI";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Button btnMessage;
		private System.Windows.Forms.TextBox txtMessage;
		private System.Windows.Forms.RichTextBox logBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox addBox;
		private System.Windows.Forms.TextBox userBox;
		private System.Windows.Forms.TextBox passwordBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ProgressBar progressBar1;
	}
}
