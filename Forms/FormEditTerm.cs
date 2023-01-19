using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Torn;

namespace Torn5.Forms
{
    public partial class FormEditTerm : Form
    {
        public TermRecord Term { get; set; }
        //TODO Move this to configure per league
        private decimal yellowTerm = -1000;
        private decimal redTerm = -2000;
        private decimal verbalTerm = 0;
        private decimal otherTerm = 0;

        public FormEditTerm()
        {
            InitializeComponent();
        }

        public FormEditTerm(TermRecord term)
        {
            InitializeComponent();
            Term = term;
        }

        private void EditTerm_Load(object sender, EventArgs e)
        {
            typeSelector.DataSource = Enum.GetNames(typeof(TermType));
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            TermType.TryParse(typeSelector.Text, out TermType termType);
            Term = new TermRecord(termType, Term?.Time, (int)penalty.Value, reason.Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void typeSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(typeSelector.Text == TermType.Yellow.ToString())
            {
                Console.WriteLine(typeSelector.Tag);
                penalty.Value = yellowTerm;
                penalty.Enabled = false;
            }
            if (typeSelector.Text == TermType.Red.ToString())
            {
                penalty.Value = redTerm;
                penalty.Enabled = false;
            }
            if (typeSelector.Text == TermType.Verbal.ToString())
            {
                penalty.Value = verbalTerm;
                penalty.Enabled = false;
            }
            if (typeSelector.Text == TermType.Other.ToString())
            {
                penalty.Value = otherTerm;
                penalty.Enabled = true;
            }
        }

        private void FormEditTerm_Shown(object sender, EventArgs e)
        {
            this.CenterToParent();
            if (Term != null)
            {
                penalty.Value = Term.Value;
                typeSelector.Text = Term.Type.ToString();
                reason.Text = Term.Reason;
                Time.Text = Term.Time != null ? Term.Time.ToString() : "Post Game Term";
            }
        }
    }
}
