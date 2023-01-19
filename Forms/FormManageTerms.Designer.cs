
namespace Torn5.Forms
{
    partial class FormManageTerms
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
            this.termList = new System.Windows.Forms.ListView();
            this.timeCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.typeCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.penaltyCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.reasonCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.addButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // termList
            // 
            this.termList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.termList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.timeCol,
            this.typeCol,
            this.penaltyCol,
            this.reasonCol});
            this.termList.HideSelection = false;
            this.termList.Location = new System.Drawing.Point(12, 12);
            this.termList.Name = "termList";
            this.termList.Size = new System.Drawing.Size(528, 397);
            this.termList.TabIndex = 0;
            this.termList.UseCompatibleStateImageBehavior = false;
            this.termList.View = System.Windows.Forms.View.Details;
            this.termList.SelectedIndexChanged += new System.EventHandler(this.termList_SelectedIndexChanged);
            this.termList.DoubleClick += new System.EventHandler(this.termList_DoubleClick);
            // 
            // timeCol
            // 
            this.timeCol.Text = "Time";
            this.timeCol.Width = 150;
            // 
            // typeCol
            // 
            this.typeCol.Text = "Type";
            // 
            // penaltyCol
            // 
            this.penaltyCol.Text = "Penalty";
            // 
            // reasonCol
            // 
            this.reasonCol.Text = "Reason";
            this.reasonCol.Width = 250;
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addButton.Location = new System.Drawing.Point(12, 421);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 1;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // editButton
            // 
            this.editButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.editButton.Enabled = false;
            this.editButton.Location = new System.Drawing.Point(93, 421);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(75, 23);
            this.editButton.TabIndex = 2;
            this.editButton.Text = "Edit";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // FormManageTerms
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 456);
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.termList);
            this.Name = "FormManageTerms";
            this.Text = "ManageTerms";
            this.Shown += new System.EventHandler(this.ManageTerms_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView termList;
        private System.Windows.Forms.ColumnHeader timeCol;
        private System.Windows.Forms.ColumnHeader typeCol;
        private System.Windows.Forms.ColumnHeader penaltyCol;
        private System.Windows.Forms.ColumnHeader reasonCol;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button editButton;
    }
}