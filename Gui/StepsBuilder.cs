// Bareplan (c) 2015-17 MIT License <baltasarq@gmail.com>

namespace Bareplan.Gui {
	using System;
	using System.Diagnostics;
	using System.Drawing;
	using System.Collections.Generic;
	using System.Windows.Forms;

	public class StepsBuilder: Form {
		public StepsBuilder(Form parent, string currentSteps)
		{
			this.steps = new List<int>();
			this.Icon = parent.Icon;
			this.Build();
			
			Array.ForEach( currentSteps.Trim().Split( ',' ), (x) => this.steps.Add( int.Parse( x ) ) );
			this.UpdateSteps();
		}
		
		private void BuildIcons()
		{
			try {
				this.addIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "bareplan.Res.addIcon.png" ) );
						
				this.removeIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "bareplan.Res.removeIcon.png" ) );
			} catch(Exception e)
			{
				Debug.WriteLine( "ERROR loading icons: " + e.Message);
			}

		}

		private void BuildPanelStep()
		{
			var pnlSteps = new FlowLayoutPanel() { Dock = DockStyle.Top };
			pnlSteps.SuspendLayout();
			var lblSteps = new Label() { Text = "Steps (in days)", Dock = DockStyle.Right };

			this.spStep = new NumericUpDown { Minimum = 0, Maximum = 365, Dock = DockStyle.Fill };
			this.spStep.MaximumSize = new Size( (int) this.fontSize.Width * 3, (int) this.fontSize.Height + 5 );

			pnlSteps.Controls.Add( spStep );
			pnlSteps.Controls.Add( lblSteps );
			pnlSteps.ResumeLayout( true );
			this.pnlMain.Controls.Add( pnlSteps );
			pnlSteps.MaximumSize = new Size( int.MaxValue, (int) this.fontSize.Height * 2 );
		}

		private void BuildControlButtons()
		{
			var imgList = new ImageList { ImageSize = new Size( 16, 16 ) };
			
			imgList.Images.AddRange( new []{ this.addIcon, this.removeIcon } );
			this.btMore = new Button { ImageList = imgList, ImageIndex = 0, Dock = DockStyle.Top };
			this.btDelete = new Button { ImageList = imgList, ImageIndex = 1, Dock = DockStyle.Top };
			
			var buttonsPanel = new TableLayoutPanel { Dock = DockStyle.Bottom };

			buttonsPanel.Controls.Add( this.btMore );
			buttonsPanel.Controls.Add( this.btDelete );

			this.pnlMain.Controls.Add( buttonsPanel );
			buttonsPanel.MaximumSize = new Size( int.MaxValue, (int) this.fontSize.Height * 4 );
			
			this.btMore.Click += (o, args) => {
				this.steps.Add( (int) this.spStep.Value );
				this.UpdateSteps();
			};
			this.btDelete.Click += (o, args) => {
				if ( this.steps.Count > 0 ) {
					this.steps.RemoveAt( this.steps.Count - 1 );
				}
				
				this.UpdateSteps();
			};
		}

		private void Build()
		{
			this.BuildIcons();
			
			// Sizes
			this.defaultFont = new Font( 
				System.Drawing.SystemFonts.DefaultFont.FontFamily,
				12,
				FontStyle.Regular );
			Graphics grf = this.CreateGraphics();
			this.fontSize = grf.MeasureString( "M", this.defaultFont );

			this.txtTest = new TextBox {
					Multiline = true,
					Dock = DockStyle.Right,
					ReadOnly = true };
			this.btOk = new Button {
					Text = "Ok",
					Dock = DockStyle.Bottom,
					DialogResult = DialogResult.OK };
			this.btCancel = new Button {
					Text = "Cancel",
					Dock = DockStyle.Bottom,
					DialogResult = DialogResult.Cancel };

			// Main panel
			this.pnlMain = new Panel() { Dock = DockStyle.Fill };
			this.pnlMain.SuspendLayout();

			this.BuildPanelStep();
			this.BuildControlButtons();

			var pnlButtons = new FlowLayoutPanel(){ Dock = DockStyle.Bottom };
			pnlButtons.FlowDirection = FlowDirection.RightToLeft;

			this.pnlMain.Controls.Add( txtTest );
			pnlButtons.Controls.Add( btOk );
			pnlButtons.Controls.Add( btCancel );
			this.Text = "Steps builder";
			this.StartPosition = FormStartPosition.CenterParent;
			this.Controls.Add( this.pnlMain );
			this.pnlMain.ResumeLayout( true );
			this.Controls.Add( pnlButtons );
			pnlButtons.MaximumSize = new Size( int.MaxValue, (int) this.fontSize.Height * 3 );
			this.pnlMain.MaximumSize = new Size( int.MaxValue, (int) this.fontSize.Height * 2 );
			this.MaximumSize = new Size( int.MaxValue, (int) ( ( (double) ( this.pnlMain.MaximumSize.Height + pnlButtons.MaximumSize.Height ) ) * 1.5 ) );
			this.MinimumSize = new Size( this.Width, this.MaximumSize.Height );
		}

		public String Result {
			get {
				string toret = String.Join( ", ", this.steps );

				if ( toret.Length == 0 ) {
					toret = "0";
				}

				return toret;
			}
		}
		
		private void UpdateSteps()
		{
			this.txtTest.Text = String.Join( ", ", this.steps );
			this.txtTest.SelectionLength = 0;
			this.txtTest.SelectionStart = this.txtTest.TextLength;
		}

		private List<int> steps;

		private TextBox txtTest;
		private NumericUpDown spStep;
		private Button btOk;
		private Button btCancel;
		private Button btMore;
		private Button btDelete;
		private Panel pnlMain;
		private SizeF fontSize;
		private Font defaultFont;
		
		private Bitmap addIcon;
		private Bitmap removeIcon;
	}
}

