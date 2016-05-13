using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Bareplan.Gui {
	public class StepsBuilder: Form {
		public StepsBuilder(Form parent)
		{
			this.steps = new List<int>();
			this.Icon = parent.Icon;
			this.Build();
		}

		private void BuildPanelStep()
		{
			var pnlPanel = new FlowLayoutPanel() { Dock = DockStyle.Top };
			var spStep = new NumericUpDown() { Minimum = 0, Maximum = 365, Dock = DockStyle.Left };
			var btDelete = new Button() { Text = "X", Dock = DockStyle.Right };

			pnlPanel.Controls.Add( spStep );
			pnlPanel.Controls.Add( btDelete );
			spStep.MaximumSize = new Size( (int) this.fontSize.Width * 3, (int) this.fontSize.Height + 5 );
			btDelete.MaximumSize = spStep.MaximumSize;

			this.pnlMain.Controls.Add( pnlPanel );
		}

		private void BuildControlPanel() {
			this.btMore = new Button() { Text = "+", Dock = DockStyle.Left };
			var lblSteps = new Label() { Text = "Steps (in days)", Dock = DockStyle.Right };
			this.pnlMain = new TableLayoutPanel() { Dock = DockStyle.Fill, AutoScroll = true };
			var topPanel = new FlowLayoutPanel() { Dock = DockStyle.Top };

			topPanel.Controls.Add( this.btMore );
			topPanel.Controls.Add( lblSteps );

			this.pnlMain.Controls.Add( topPanel );
			this.Controls.Add( this.pnlMain );

			this.BuildPanelStep();
			this.BuildPanelStep();
		}

		private void Build()
		{
			// Sizes
			this.defaultFont = new Font( 
				System.Drawing.SystemFonts.DefaultFont.FontFamily,
				12,
				FontStyle.Regular );
			Graphics grf = this.CreateGraphics();
			this.fontSize = grf.MeasureString( "M", this.defaultFont );

			this.txtTest = new TextBox() {
					Multiline = true,
					Dock = DockStyle.Right,
					ReadOnly = true };
			this.btOk = new Button() {
					Text = "Ok",
					Dock = DockStyle.Bottom,
					DialogResult = DialogResult.OK };
			this.btCancel = new Button() {
					Text = "Cancel",
					Dock = DockStyle.Bottom,
					DialogResult = DialogResult.Cancel };

			this.BuildControlPanel();

			var pnlButtons = new FlowLayoutPanel(){ Dock = DockStyle.Bottom };
			pnlButtons.MinimumSize = new Size( int.MaxValue, this.btOk.Height + 10 );
			pnlButtons.FlowDirection = FlowDirection.RightToLeft;

			this.Controls.Add( txtTest );
			pnlButtons.Controls.Add( btOk );
			pnlButtons.Controls.Add( btCancel );
			this.Controls.Add( pnlButtons );
			this.Text = "Steps builder";
			this.StartPosition = FormStartPosition.CenterParent;
			this.MinimumSize = new Size( 320, 200 );
		}

		public String Result {
			get {
				StringBuilder toret = new StringBuilder();

				this.steps.ForEach( (x) => { toret.Append( x ); toret.Append( ',' ); } );

				if ( toret.Length == 0 ) {
					toret.Append( "0" );
				}

				return toret.ToString();
			}
		}

		private List<int> steps;

		private TextBox txtTest;
		private Button btOk;
		private Button btCancel;
		private Button btMore;
		private TableLayoutPanel pnlMain;
		private SizeF fontSize;
		private Font defaultFont;
	}
}

