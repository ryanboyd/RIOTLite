namespace WindowsFormsApplication1
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.BgWorker = new System.ComponentModel.BackgroundWorker();
            this.ScanSubfolderCheckbox = new System.Windows.Forms.CheckBox();
            this.StartButton = new System.Windows.Forms.Button();
            this.FolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.PunctuationBox = new System.Windows.Forms.TextBox();
            this.FilenameLabel = new System.Windows.Forms.Label();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.EncodingDropdown = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.LoadDictionaryButton = new System.Windows.Forms.Button();
            this.RawWCCheckbox = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // BgWorker
            // 
            this.BgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BgWorkerClean_DoWork);
            this.BgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BgWorker_RunWorkerCompleted);
            // 
            // ScanSubfolderCheckbox
            // 
            this.ScanSubfolderCheckbox.AutoSize = true;
            this.ScanSubfolderCheckbox.Location = new System.Drawing.Point(89, 282);
            this.ScanSubfolderCheckbox.Name = "ScanSubfolderCheckbox";
            this.ScanSubfolderCheckbox.Size = new System.Drawing.Size(108, 17);
            this.ScanSubfolderCheckbox.TabIndex = 2;
            this.ScanSubfolderCheckbox.Text = "Scan subfolders?";
            this.ScanSubfolderCheckbox.UseVisualStyleBackColor = true;
            // 
            // StartButton
            // 
            this.StartButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartButton.Location = new System.Drawing.Point(67, 244);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(152, 32);
            this.StartButton.TabIndex = 3;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // FolderBrowser
            // 
            this.FolderBrowser.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.FolderBrowser.ShowNewFolderButton = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(191, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Characters to Remove from Texts:";
            // 
            // PunctuationBox
            // 
            this.PunctuationBox.AcceptsTab = true;
            this.PunctuationBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PunctuationBox.Location = new System.Drawing.Point(6, 42);
            this.PunctuationBox.Name = "PunctuationBox";
            this.PunctuationBox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.PunctuationBox.Size = new System.Drawing.Size(248, 22);
            this.PunctuationBox.TabIndex = 4;
            this.PunctuationBox.Text = ";:\"@#$%^&\t*(){}\\|,/<>`~[].?!";
            // 
            // FilenameLabel
            // 
            this.FilenameLabel.AutoEllipsis = true;
            this.FilenameLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FilenameLabel.Location = new System.Drawing.Point(12, 313);
            this.FilenameLabel.Name = "FilenameLabel";
            this.FilenameLabel.Size = new System.Drawing.Size(260, 15);
            this.FilenameLabel.TabIndex = 6;
            this.FilenameLabel.Text = "Waiting to analyze texts...";
            this.FilenameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.FileName = "Repeatalizer.csv";
            this.saveFileDialog.Filter = "CSV Files|*.csv";
            this.saveFileDialog.Title = "Please choose where to save your output";
            // 
            // EncodingDropdown
            // 
            this.EncodingDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EncodingDropdown.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EncodingDropdown.FormattingEnabled = true;
            this.EncodingDropdown.Location = new System.Drawing.Point(6, 95);
            this.EncodingDropdown.Name = "EncodingDropdown";
            this.EncodingDropdown.Size = new System.Drawing.Size(248, 23);
            this.EncodingDropdown.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(9, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(198, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "Encoding of Dictionary && Text Files:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.RawWCCheckbox);
            this.groupBox2.Controls.Add(this.PunctuationBox);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.EncodingDropdown);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(260, 167);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Text Reading Controls";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "DictionaryFile.txt";
            this.openFileDialog.Filter = "Dictionary Files|*.dic";
            // 
            // LoadDictionaryButton
            // 
            this.LoadDictionaryButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadDictionaryButton.Location = new System.Drawing.Point(67, 198);
            this.LoadDictionaryButton.Name = "LoadDictionaryButton";
            this.LoadDictionaryButton.Size = new System.Drawing.Size(152, 32);
            this.LoadDictionaryButton.TabIndex = 20;
            this.LoadDictionaryButton.Text = "Load Dictionary";
            this.LoadDictionaryButton.UseVisualStyleBackColor = true;
            this.LoadDictionaryButton.Click += new System.EventHandler(this.LoadDictionaryButton_Click);
            // 
            // RawWCCheckbox
            // 
            this.RawWCCheckbox.AutoSize = true;
            this.RawWCCheckbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RawWCCheckbox.Location = new System.Drawing.Point(6, 133);
            this.RawWCCheckbox.Name = "RawWCCheckbox";
            this.RawWCCheckbox.Size = new System.Drawing.Size(208, 19);
            this.RawWCCheckbox.TabIndex = 21;
            this.RawWCCheckbox.Text = "Raw Category Word Count Output";
            this.RawWCCheckbox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.ClientSize = new System.Drawing.Size(284, 336);
            this.Controls.Add(this.LoadDictionaryButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.FilenameLabel);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.ScanSubfolderCheckbox);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(300, 375);
            this.MinimumSize = new System.Drawing.Size(300, 375);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RIOTLite";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.ComponentModel.BackgroundWorker BgWorker;
        private System.Windows.Forms.CheckBox ScanSubfolderCheckbox;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowser;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox PunctuationBox;
        private System.Windows.Forms.Label FilenameLabel;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ComboBox EncodingDropdown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button LoadDictionaryButton;
        private System.Windows.Forms.CheckBox RawWCCheckbox;
    }
}

