// Bareplan - (c) 2017 Baltasar MIT License <baltasarq@gmail.com>

namespace Bareplan.Gui {
	using System;
	using System.Drawing;
	using System.Diagnostics;
	using System.Windows.Forms;
	
	using Core;

	/// <summary>
	/// Presents an export dialog to the user.
	/// </summary>
	public class ExportDlg: Form {
		/// <summary>
		/// Initializes a new <see cref="T:Bareplan.Gui.ExportDlg"/>.
		/// </summary>
		/// <param name="formIcon">The dialog's icon.</param>
		/// <param name="doc">The <see cref="T:Document"/>.</param>
		/// <param name="path">The path the user has navigated to.</param>
		public ExportDlg(Icon formIcon, Document doc, string path)
		{
			this.Document = doc;
			this.ExportInfo = new ExportInfo( doc );
			this.Icon = formIcon;
			this.Path = path;
			
			this.Build();
			this.edFileName.Text = System.IO.Path.GetFileNameWithoutExtension( this.ExportInfo.FileName );
			this.Shown += (sender, e) => this.OnExposed();
		}
		
		private void OnExposed()
		{
			this.OnChangeType(); 
			this.CenterToParent();
		}
		
		private void BuildIcons()
		{
			try {
				this.yesIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "Bareplan.Res.yes.png" ) );

				this.noIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "Bareplan.Res.no.png" ) );


			} catch(Exception e)
			{
				Debug.WriteLine( "ERROR loading icons: " + e.Message);
			}

			return;
		}
		
		private Panel BuildFileNamePanel()
		{
			var toret = new TableLayoutPanel { Dock = DockStyle.Top };

			// Directory name			
			var pnlDir = new TableLayoutPanel { Dock = DockStyle.Top };
			this.lblDir = new Label { Text = this.Path, Dock = DockStyle.Top };
			pnlDir.Controls.Add( this.lblDir );
			pnlDir.MaximumSize = new Size( int.MaxValue, this.lblDir.Height );

			
			// File name & browse button
			var pnlFile = new Panel { Dock = DockStyle.Top };
			this.edFileName = new TextBox { Text = this.Document.FileName, Dock = DockStyle.Fill };
			this.edFileName.Font = new Font( this.edFileName.Font.FontFamily, this.edFileName.Font.Size + 4 );
			this.btBrowse = new Button { Text = "...", Dock = DockStyle.Right };
			pnlFile.Controls.Add( this.edFileName );
			pnlFile.Controls.Add( this.btBrowse );
			pnlFile.MaximumSize = new Size( int.MaxValue, this.edFileName.Height );

			toret.Controls.Add( pnlFile );
			toret.Controls.Add( pnlDir );
			
			this.btBrowse.Click += (sender, e) => this.OnBrowse();
			this.edFileName.TextChanged += (sender, e) => this.OnFileNameChanged();
			return toret;
		}
		
		private CheckedListBox BuildColumnsPanel()
		{
			var toret = new CheckedListBox { Dock = DockStyle.Fill };
			toret.Font = new Font( toret.Font.FontFamily, toret.Font.Size + 4 );

			
			toret.Items.Add( L10n.Get( L10n.Id.HdSession ), true );
			toret.Items.Add( L10n.Get( L10n.Id.HdWeek ), true );
			toret.Items.Add( L10n.Get( L10n.Id.HdDay ), true );
			toret.Items.Add( L10n.Get( L10n.Id.HdDate ), true );
			toret.Items.Add( L10n.Get( L10n.Id.HdKind ), true );
			toret.Items.Add( L10n.Get( L10n.Id.HdContents ), true );
			
			toret.CheckOnClick = true;
			toret.ItemCheck += (object o, ItemCheckEventArgs e) => this.OnColumnClicked( e );
			
			return toret;
		}
		
		private FlowLayoutPanel BuildButtonsPanel()
		{
			var images = new ImageList();
			var toret = new FlowLayoutPanel {
				Dock = DockStyle.Bottom,
				FlowDirection = FlowDirection.RightToLeft
			};
			
			images.Images.AddRange( new []{ this.yesIcon, this.noIcon } );
			this.btCancel = new Button{ ImageIndex = 1, ImageList = images, Dock = DockStyle.Right };			
			this.btOk = new Button { ImageIndex = 0, ImageList = images, Dock = DockStyle.Right };

			toret.Controls.Add( this.btOk );			
			toret.Controls.Add( this.btCancel );
			toret.MaximumSize = new Size( int.MaxValue, (int) ( this.btOk.Height * 1.5 ) );
			
			this.btOk.Click += (sender, e) => this.DialogResult = DialogResult.OK;
			this.btCancel.Click += (sender, e) => this.DialogResult = DialogResult.Cancel;
			
			return toret;
		}
		
		private ComboBox BuildTypeChooser()
		{
			var toret = new ComboBox {
				Dock = DockStyle.Top,
				DropDownStyle = ComboBoxStyle.DropDownList
			};
			
			foreach(string strType in Enum.GetNames( typeof( ExportInfo.FileType ) ) )
			{
				toret.Items.Add( strType );
			}
			
			toret.SelectedIndex = 0;
			toret.SelectedIndexChanged += (sender, e) => this.OnChangeType();
			return toret;
		}
		
		private void Build()
		{
			this.pnlMain = new Panel { Dock = DockStyle.Fill };
			this.chkList = this.BuildColumnsPanel();
			this.cmbType = this.BuildTypeChooser();
			
			this.BuildIcons();
			this.pnlMain.Controls.Add( this.chkList );	
			this.pnlMain.Controls.Add( this.cmbType );
			this.pnlMain.Controls.Add( this.BuildFileNamePanel() );
			this.pnlMain.Controls.Add( this.BuildButtonsPanel() );
			
			this.AcceptButton = this.btOk;
			this.CancelButton = this.btCancel;
			
			this.Controls.Add( this.pnlMain );
			this.MinimumSize = new Size( 320, 200 );
			
			var text = L10n.Get( L10n.Id.OpExport );
		}
		
		private void OnColumnClicked(ItemCheckEventArgs args)
		{
			this.ExportInfo.VisibleColumn[ args.Index ] = ( args.NewValue == CheckState.Checked );
		}
		
		private void OnFileNameChanged()
		{
			this.ExportInfo.FileName = 
					System.IO.Path.Combine( this.Path, this.edFileName.Text );
		}
		
		private void OnChangeType()
		{
			int selection = Math.Max( 0, this.cmbType.SelectedIndex );
			string fileName = this.edFileName.Text.Trim();
			string fileExt = ExportInfo.FileExt[ selection ];
			
			if ( string.IsNullOrWhiteSpace( fileName ) ) {
				fileName = "out";
			}
			
			// Update export options
			this.ExportInfo.Type = (ExportInfo.FileType) selection;
			this.edFileName.Text = System.IO.Path.ChangeExtension( fileName, fileExt );
			this.OnFileNameChanged();
 		}
		
		private void OnBrowse()
		{
			var dlgBrowse = new FolderBrowserDialog();
			
			if ( dlgBrowse.ShowDialog() == DialogResult.OK ) {
				this.Path = this.lblDir.Text = dlgBrowse.SelectedPath;
				this.OnFileNameChanged();
			}
			
			return;
		}
		
		/// <summary>
		/// Gets the name of the file the user chose.
		/// </summary>
		/// <value>The name of the file, as a string.</value>
		public string FileName {
			get; private set;
		}
		
		/// <summary>
		/// Gets or sets the path in which the file is going to be stored.
		/// </summary>
		/// <value>The path, as a string.</value>
		public string Path {
			get; set;
		}
		
		/// <summary>
		/// Gets the document the user wants to export.
		/// </summary>
		/// <value>The <see cref="Document"/>.</value>
		public Document Document {
			get; private set;
		}
		
		/// <summary>
		/// Gets the export options.
		/// </summary>
		/// <value>The <see cref="ExportInfo"/>.</value>
		public ExportInfo ExportInfo {
			get; private set;
		}
		
		private Button btCancel;
		private Button btOk;
		private Button btBrowse;
		private TextBox edFileName;
		private Label lblDir;
		private ComboBox cmbType;
		
		private Bitmap yesIcon;
		private Bitmap noIcon;
		
		private CheckedListBox chkList;
		private Panel pnlMain;
	}
}
