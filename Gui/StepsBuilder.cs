// Bareplan (c) 2015-17 MIT License <baltasarq@gmail.com>

namespace Bareplan.Gui {
	using System;
	using System.Diagnostics;
	using System.Drawing;
	using System.Collections.Generic;
	using System.Windows.Forms;

	using Core;

	public class StepsBuilder: Form {
		public StepsBuilder(Form parent, string currentSteps)
		{
			this.steps = new List<int>();
			this.Icon = parent.Icon;
			this.Build();
			
			Array.ForEach( currentSteps.Trim().Split( ',' ), (x) => this.steps.Add( int.Parse( x ) ) );
			this.UpdateSteps();
		}
		
		void BuildIcons()
		{
			try {
				this.addIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "Bareplan.Res.addIcon.png" ) );
						
				this.removeIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "Bareplan.Res.removeIcon.png" ) );
			} catch(Exception e)
			{
				Debug.WriteLine( "ERROR loading icons: " + e.Message);
			}

		}

		void BuildPanelNewStep()
		{
			var pnlNewStep = new Panel { Dock = DockStyle.Fill  };
			var pnlSteps = new Panel { Dock = DockStyle.Top };
			var lblNewStep = new Label { Text = L10n.Get( L10n.Id.LblNewStep ), Dock = DockStyle.Left };

			this.spStep = new NumericUpDown { Minimum = 0, Maximum = 365, Dock = DockStyle.Fill };
			this.spStep.MaximumSize = new Size( (int) this.fontSize.Width * 5, (int) this.fontSize.Height + 5 );
			this.spStep.MinimumSize = this.spStep.MaximumSize;

			pnlNewStep.Controls.Add( spStep );
			pnlNewStep.Controls.Add( lblNewStep );

			pnlSteps.Controls.Add( pnlNewStep );
			pnlSteps.Controls.Add( this.btMore );

			this.pnlMain.Controls.Add( pnlSteps );
			pnlSteps.MaximumSize = new Size( int.MaxValue, (int) this.fontSize.Height * 2 );
		}

		void BuildControlButtons()
		{
			var imgList = new ImageList { ImageSize = new Size( 16, 16 ) };

			imgList.Images.AddRange( new []{ this.addIcon, this.removeIcon } );
			this.btMore = new Button { ImageList = imgList, ImageIndex = 0, Dock = DockStyle.Right };
			this.btDelete = new Button { ImageList = imgList, ImageIndex = 1, Dock = DockStyle.Right };

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

			this.btMore.MaximumSize = new Size( int.MaxValue, (int) this.fontSize.Height * 2 );
			this.btDelete.MaximumSize = new Size( int.MaxValue, (int) this.fontSize.Height * 2 );
		}

		void BuildStepsText()
		{
			var pnlTxtTest = new Panel { Dock = DockStyle.Fill };

			this.txtTest = new TextBox {
				Multiline = true,
				Dock = DockStyle.Fill,
				ReadOnly = true
			};

			pnlTxtTest.Controls.Add( this.txtTest );
			pnlTxtTest.Controls.Add( this.btDelete );
			this.pnlMain.Controls.Add( pnlTxtTest );
		}

		void BuildOkCancel()
		{
			var pnlButtons = new FlowLayoutPanel { Dock = DockStyle.Bottom };
			pnlButtons.FlowDirection = FlowDirection.RightToLeft;

			this.btOk = new Button {
				Text = "Ok",
				Dock = DockStyle.Bottom,
				DialogResult = DialogResult.OK
			};

			this.btCancel = new Button {
				Text = "Cancel",
				Dock = DockStyle.Bottom,
				DialogResult = DialogResult.Cancel
			};

			pnlButtons.Controls.Add( btOk );
			pnlButtons.Controls.Add( btCancel );
			pnlButtons.MaximumSize = new Size( int.MaxValue, (int)this.fontSize.Height * 2 );
			this.Controls.Add( pnlButtons );
		}

		void Build()
		{
			this.BuildIcons();
			
			// Sizes
			this.defaultFont = new Font( 
				SystemFonts.DefaultFont.FontFamily,
				12,
				FontStyle.Regular );
			Graphics grf = this.CreateGraphics();
			this.fontSize = grf.MeasureString( "M", this.defaultFont );

			// Main panel
			this.pnlMain = new Panel() { Dock = DockStyle.Fill };
			this.pnlMain.SuspendLayout();

			this.BuildControlButtons();
			this.BuildStepsText();
			this.BuildOkCancel();
			this.BuildPanelNewStep();

			this.Text = "Steps builder";
			this.StartPosition = FormStartPosition.CenterParent;
			this.Controls.Add( this.pnlMain );
			this.pnlMain.ResumeLayout( true );
			this.MinimumSize = this.Size;
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
		
		void UpdateSteps()
		{
			this.txtTest.Text = String.Join( ", ", this.steps )
								+ " " + L10n.Get( L10n.Id.LblDays );
			this.txtTest.SelectionLength = 0;
			this.txtTest.SelectionStart = this.txtTest.TextLength;
		}

		List<int> steps;

		TextBox txtTest;
		NumericUpDown spStep;
		Button btOk;
		Button btCancel;
		Button btMore;
		Button btDelete;
		Panel pnlMain;
		SizeF fontSize;
		Font defaultFont;
		
		Bitmap addIcon;
		Bitmap removeIcon;
	}
}

