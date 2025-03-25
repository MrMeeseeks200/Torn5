using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using Zoom;

namespace Torn5.Controls
{
	/// <summary>
	/// UI to print and/or save a report template.
	/// </summary>
	/// 
	public partial class PrintReport : UserControl
	{
		[Browsable(true), Category("Data")]
		public DisplayReport DisplayReport { get; set; }

		bool fileNameSet = false;
		string fileName;
		[Browsable(false), Category("Data")]
		public string FileName
		{
			get { return fileName; }

			set
			{
				fileName = value;
				saveFileDialog.InitialDirectory = Path.GetDirectoryName(value);
				saveFileDialog.FileName = Path.GetFileName(value);
				if (value != null)
					fileNameSet = true;
			}
		}

		[Browsable(true), Category("Action"), Description("Occurs when user clicks Save when HTML SVG is selected.")]
		public event EventHandler SaveSvg;
		[Browsable(true), Category("Action"), Description("Occurs when user clicks Save when HTML table is selected.")]
		public event EventHandler SaveHtmlTable;
		[Browsable(true), Category("Action"), Description("Occurs when user clicks Save when TSV is selected.")]
		public event EventHandler SaveTsv;
		[Browsable(true), Category("Action"), Description("Occurs when user clicks Save when CSV is selected.")]
		public event EventHandler SaveCsv;
		[Browsable(true), Category("Action"), Description("Occurs when user clicks Save when PNG is selected.")]
		public event EventHandler SavePng;

		readonly SaveFileDialog saveFileDialog = new SaveFileDialog();
		readonly PrintDialog printDialog = new PrintDialog();
		readonly PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();

		public PrintReport()
		{
			InitializeComponent();
		}

		private void ButtonSaveClick(object sender, EventArgs e)
		{
			var outputFormat = radioSvg.Checked ? OutputFormat.Svg :
				radioTables.Checked ? OutputFormat.HtmlTable :
				radioTsv.Checked ? OutputFormat.Tsv :
				radioCsv.Checked ? OutputFormat.Csv :
				OutputFormat.Png;

			if (!fileNameSet && DisplayReport?.Report != null)
			{
				// Build a file name from report name and output type.
				fileName = DisplayReport.Report.Title.Replace('/', '-').Replace(' ', '_') + "." + outputFormat.ToExtension();  // Replace / with - so dates still look OK, and space with _ to make URLs easier if this file is uploaded to the web.
				fileName = Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c, '_'));  // Replace all other invalid chars with _.
				saveFileDialog.FileName = fileName;
			}

			string path = Path.GetDirectoryName(fileName);
			if (!string.IsNullOrWhiteSpace(path))
				Directory.CreateDirectory(path);

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				fileName = saveFileDialog.FileName;

				if (outputFormat == OutputFormat.Png)
				{
					if (this.SavePng != null)
						this.SavePng(this, new EventArgs());
					else if (checkBoxScale.Checked)
						DisplayReport.Report.ToBitmap((float)numericScale.Value).Save(fileName);
					else
						DisplayReport.BackgroundImage.Save(fileName);
				}
				else
				{
					if (this.SaveSvg != null && outputFormat == OutputFormat.Svg)
						this.SaveSvg(this, new EventArgs());
					else if (this.SaveHtmlTable != null && outputFormat == OutputFormat.HtmlTable)
						this.SaveHtmlTable(this, new EventArgs());
					else if (this.SaveTsv != null && outputFormat == OutputFormat.Tsv)
						this.SaveTsv(this, new EventArgs());
					else if (this.SaveCsv != null && outputFormat == OutputFormat.Csv)
						this.SaveCsv(this, new EventArgs());
					else
					{
						var reports = new ZoomReports() { DisplayReport.Report };

						using (StreamWriter sw = File.CreateText(fileName))
							sw.Write(reports.ToOutput(outputFormat));
					}
				}
				buttonShow.Enabled = true;
			}
		}

		private void ButtonShowClick(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(fileName);
		}

		private void ButtonPrintClick(object sender, EventArgs e)
		{
			var pd = DisplayReport.Report.ToPrint();
			if (printDialog.ShowDialog() == DialogResult.OK)
				pd.Print();
		}

		private void ButtonPrintPreviewClick(object sender, EventArgs e)
		{
			printPreviewDialog.Document = DisplayReport.Report.ToPrint();
			printPreviewDialog.ShowDialog();
		}

		private void CheckedChanged(object sender, EventArgs e)
		{
			checkBoxScale.Enabled = radioPng.Checked;
			numericScale.Enabled = radioPng.Checked && checkBoxScale.Checked;
		}

		private void ShowOnTBoardClicked(object sender, EventArgs e)
        {
			int TBOARD_SOCKET = 21570;

			UdpClient udp = new UdpClient();
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse("255.255.255.255"), TBOARD_SOCKET);

			string result = DisplayReport.Report.ToTBoard();

			List<string> strs = result.Split(510).ToList();

			foreach(string str in strs)
            {
				string index = strs.IndexOf(str).ToString().PadLeft(2, '0');
				string chunk = index + str + "\x00";
				byte[] sendBytes = Encoding.ASCII.GetBytes(chunk);
				udp.Send(sendBytes, sendBytes.Length, groupEP);
			}

			string emptyIndex = strs.Count().ToString().PadLeft(2, '0');
			byte[] sendBytesEnd = Encoding.ASCII.GetBytes(emptyIndex + "\x00");
			udp.Send(sendBytesEnd, sendBytesEnd.Length, groupEP);
		}
    }
	public static class Extensions
	{
		public static IEnumerable<string> Split(this string str, int n)
		{
			if (String.IsNullOrEmpty(str) || n < 1)
			{
				throw new ArgumentException();
			}

			for (int i = 0; i < str.Length; i += n)
			{
				if (str.Length - i > n)
				{
					yield return str.Substring(i, n);
				}
				else
				{
					yield return str.Substring(i, str.Length - i);
				}

			}
		}
	}
}
