﻿namespace Torn.UI
{
	partial class FormAdhoc
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAdhoc));
			this.timerRedraw = new System.Windows.Forms.Timer(this.components);
			this.groupBoxOutputFormat = new System.Windows.Forms.GroupBox();
			this.buttonPrintPreview = new System.Windows.Forms.Button();
			this.buttonPrint = new System.Windows.Forms.Button();
			this.buttonShow = new System.Windows.Forms.Button();
			this.radioCsv = new System.Windows.Forms.RadioButton();
			this.buttonSave = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.radioTsv = new System.Windows.Forms.RadioButton();
			this.radioTables = new System.Windows.Forms.RadioButton();
			this.radioSvg = new System.Windows.Forms.RadioButton();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.panelDisplay = new System.Windows.Forms.Panel();
			this.printDialog = new System.Windows.Forms.PrintDialog();
			this.printPreviewDialog = new System.Windows.Forms.PrintPreviewDialog();
			this.groupBoxOutputFormat.SuspendLayout();
			this.SuspendLayout();
			// 
			// timerRedraw
			// 
			this.timerRedraw.Interval = 1000;
			this.timerRedraw.Tick += new System.EventHandler(this.TimerRedrawTick);
			// 
			// groupBoxOutputFormat
			// 
			this.groupBoxOutputFormat.Controls.Add(this.buttonPrintPreview);
			this.groupBoxOutputFormat.Controls.Add(this.buttonPrint);
			this.groupBoxOutputFormat.Controls.Add(this.buttonShow);
			this.groupBoxOutputFormat.Controls.Add(this.radioCsv);
			this.groupBoxOutputFormat.Controls.Add(this.buttonSave);
			this.groupBoxOutputFormat.Controls.Add(this.label2);
			this.groupBoxOutputFormat.Controls.Add(this.label1);
			this.groupBoxOutputFormat.Controls.Add(this.radioTsv);
			this.groupBoxOutputFormat.Controls.Add(this.radioTables);
			this.groupBoxOutputFormat.Controls.Add(this.radioSvg);
			this.groupBoxOutputFormat.Dock = System.Windows.Forms.DockStyle.Right;
			this.groupBoxOutputFormat.Location = new System.Drawing.Point(720, 0);
			this.groupBoxOutputFormat.Name = "groupBoxOutputFormat";
			this.groupBoxOutputFormat.Size = new System.Drawing.Size(64, 689);
			this.groupBoxOutputFormat.TabIndex = 7;
			this.groupBoxOutputFormat.TabStop = false;
			this.groupBoxOutputFormat.Text = "Save As";
			// 
			// buttonPrintPreview
			// 
			this.buttonPrintPreview.Location = new System.Drawing.Point(6, 432);
			this.buttonPrintPreview.Name = "buttonPrintPreview";
			this.buttonPrintPreview.Size = new System.Drawing.Size(53, 23);
			this.buttonPrintPreview.TabIndex = 9;
			this.buttonPrintPreview.Text = "Preview";
			this.buttonPrintPreview.UseVisualStyleBackColor = true;
			this.buttonPrintPreview.Click += new System.EventHandler(this.buttonPrintPreviewClick);
			// 
			// buttonPrint
			// 
			this.buttonPrint.Location = new System.Drawing.Point(6, 403);
			this.buttonPrint.Name = "buttonPrint";
			this.buttonPrint.Size = new System.Drawing.Size(52, 23);
			this.buttonPrint.TabIndex = 8;
			this.buttonPrint.Text = "&Print";
			this.buttonPrint.UseVisualStyleBackColor = true;
			this.buttonPrint.Click += new System.EventHandler(this.buttonPrintClick);
			// 
			// buttonShow
			// 
			this.buttonShow.Enabled = false;
			this.buttonShow.Location = new System.Drawing.Point(6, 321);
			this.buttonShow.Name = "buttonShow";
			this.buttonShow.Size = new System.Drawing.Size(52, 76);
			this.buttonShow.TabIndex = 7;
			this.buttonShow.Text = "Show saved report in web browser";
			this.buttonShow.UseVisualStyleBackColor = true;
			this.buttonShow.Click += new System.EventHandler(this.ButtonShowClick);
			// 
			// radioCsv
			// 
			this.radioCsv.Location = new System.Drawing.Point(6, 228);
			this.radioCsv.Name = "radioCsv";
			this.radioCsv.Size = new System.Drawing.Size(56, 32);
			this.radioCsv.TabIndex = 3;
			this.radioCsv.Tag = 3;
			this.radioCsv.Text = "CSV";
			this.radioCsv.UseVisualStyleBackColor = true;
			// 
			// buttonSave
			// 
			this.buttonSave.Location = new System.Drawing.Point(6, 292);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(52, 23);
			this.buttonSave.TabIndex = 6;
			this.buttonSave.Text = "&Save";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new System.EventHandler(this.ButtonSaveClick);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 185);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(44, 48);
			this.label2.TabIndex = 5;
			this.label2.Text = "(best for data export)";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 67);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(55, 23);
			this.label1.TabIndex = 4;
			this.label1.Text = "(prettiest)";
			// 
			// radioTsv
			// 
			this.radioTsv.Location = new System.Drawing.Point(6, 158);
			this.radioTsv.Name = "radioTsv";
			this.radioTsv.Size = new System.Drawing.Size(56, 24);
			this.radioTsv.TabIndex = 2;
			this.radioTsv.Tag = 2;
			this.radioTsv.Text = "TSV";
			this.radioTsv.UseVisualStyleBackColor = true;
			// 
			// radioTables
			// 
			this.radioTables.Location = new System.Drawing.Point(6, 93);
			this.radioTables.Name = "radioTables";
			this.radioTables.Size = new System.Drawing.Size(56, 32);
			this.radioTables.TabIndex = 1;
			this.radioTables.Tag = 1;
			this.radioTables.Text = "HTML tables";
			this.radioTables.UseVisualStyleBackColor = true;
			// 
			// radioSvg
			// 
			this.radioSvg.Checked = true;
			this.radioSvg.Location = new System.Drawing.Point(6, 32);
			this.radioSvg.Name = "radioSvg";
			this.radioSvg.Size = new System.Drawing.Size(56, 32);
			this.radioSvg.TabIndex = 0;
			this.radioSvg.TabStop = true;
			this.radioSvg.Tag = 0;
			this.radioSvg.Text = "HTML SVG";
			this.radioSvg.UseVisualStyleBackColor = true;
			// 
			// panelDisplay
			// 
			this.panelDisplay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.panelDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelDisplay.Location = new System.Drawing.Point(0, 0);
			this.panelDisplay.Name = "panelDisplay";
			this.panelDisplay.Size = new System.Drawing.Size(720, 689);
			this.panelDisplay.TabIndex = 8;
			// 
			// printDialog
			// 
			this.printDialog.UseEXDialog = true;
			// 
			// printPreviewDialog
			// 
			this.printPreviewDialog.AutoScrollMargin = new System.Drawing.Size(0, 0);
			this.printPreviewDialog.AutoScrollMinSize = new System.Drawing.Size(0, 0);
			this.printPreviewDialog.ClientSize = new System.Drawing.Size(400, 300);
			this.printPreviewDialog.Enabled = true;
			this.printPreviewDialog.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog.Icon")));
			this.printPreviewDialog.Name = "printPreviewDialog";
			this.printPreviewDialog.Visible = false;
			// 
			// FormAdhoc
			// 
			this.AcceptButton = this.buttonSave;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(784, 689);
			this.Controls.Add(this.panelDisplay);
			this.Controls.Add(this.groupBoxOutputFormat);
			this.Name = "FormAdhoc";
			this.Text = "FormAdhoc";
			this.Resize += new System.EventHandler(this.FormAdhoc_Resize);
			this.groupBoxOutputFormat.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer timerRedraw;
		private System.Windows.Forms.GroupBox groupBoxOutputFormat;
		private System.Windows.Forms.Button buttonSave;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton radioCsv;
		private System.Windows.Forms.RadioButton radioTsv;
		private System.Windows.Forms.RadioButton radioTables;
		private System.Windows.Forms.RadioButton radioSvg;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.Panel panelDisplay;
		private System.Windows.Forms.Button buttonShow;
		private System.Windows.Forms.Button buttonPrint;
		private System.Windows.Forms.Button buttonPrintPreview;
		private System.Windows.Forms.PrintDialog printDialog;
		private System.Windows.Forms.PrintPreviewDialog printPreviewDialog;
	}
}