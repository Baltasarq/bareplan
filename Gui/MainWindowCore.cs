// Bareplan (c) 2015-17 MIT License <baltasarq@gmail.com>

namespace Bareplan.Gui {
	using System;
	using System.IO;
	using System.Xml;
	using System.Drawing;
	using System.Globalization;
	using System.Windows.Forms;
	
	using Bareplan.Core;

	/// <summary>
	/// The main window in the application, a <see cref="T:System.Windows.Forms.Form"/>.
	/// </summary>
	public partial class MainWindow {
		/// <summary>
		/// Initializes a new <see cref="T:Bareplan.Gui.MainWindow"/> class.
		/// </summary>
		public MainWindow()
		{
			this.cfgFile = this.filePath = "";
			this.doc = null;
		
			this.Shown += (sender, e) => this.OnShow();
			this.Build();
			this.DeactivateGui();
			this.OnIncFont();
		}
		
		private void OnShow()
		{
			filePath = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
			this.ReadConfiguration();
		}

		private void OnToolbarButton(ToolBarButton bt)
		{
			switch ( this.tbBar.Buttons.IndexOf( bt ) ) {
			case 0:
				this.OnNew();
				break;
			case 1:
				this.OnOpen();
				break;
			case 2:
				this.OnSave();
				break;
			case 3:
				this.OnExport();
				break;
			case 4:
				this.OnAdd();
				break;
			case 5:
				this.OnProperties();
				break;
			case 6:
				this.OnSettings();
				break;
			default:
				throw new ArgumentException( "toolbar button not found" );
			}

			return;
		}

		private void ChangeUILanguage(CultureInfo locale)
		{
			L10n.SetLanguage( locale );
			
			// Context menu
			this.BuildContextMenu();

			// Menus
			this.mFile.Text = L10n.Get( L10n.Id.MnFile );
			this.mView.Text = L10n.Get( L10n.Id.MnView );
			this.mEdit.Text = L10n.Get( L10n.Id.MnEdit );
			this.mHelp.Text = L10n.Get( L10n.Id.MnHelp );

			this.opNew.Text = L10n.Get( L10n.Id.OpNew );
			this.opOpen.Text = L10n.Get( L10n.Id.OpOpen );
			this.opSave.Text = L10n.Get( L10n.Id.OpSave );
			this.opSaveAs.Text = L10n.Get( L10n.Id.OpSaveAs );
			this.opClose.Text = L10n.Get( L10n.Id.OpClose );
			this.opExport.Text = L10n.Get( L10n.Id.OpExport );
			this.opQuit.Text = L10n.Get( L10n.Id.OpQuit );

			this.opIncFont.Text = L10n.Get( L10n.Id.OpIncFont );
			this.opDecFont.Text = L10n.Get( L10n.Id.OpDecFont );

			this.opAdd.Text = L10n.Get( L10n.Id.OpAdd );
			this.opInsert.Text = L10n.Get( L10n.Id.OpInsert );
			this.opInsertDate.Text = L10n.Get( L10n.Id.OpInsertDate );
			this.opInsertTask.Text = L10n.Get( L10n.Id.OpInsertTask );
			this.opRemove.Text = L10n.Get( L10n.Id.OpRemove );
			this.opRemoveDate.Text = L10n.Get( L10n.Id.OpRemoveDate );
			this.opRemoveTask.Text = L10n.Get( L10n.Id.OpRemoveTask );
			this.opProperties.Text = L10n.Get( L10n.Id.OpProperties );
			this.opSettings.Text = L10n.Get( L10n.Id.OpSettings );

			this.opAbout.Text = L10n.Get( L10n.Id.OpAbout );

			// Planning table headers
			this.grdPlanning.Columns[ (int) ColsIndex.Num ].HeaderText =
				L10n.Get( L10n.Id.HdNum );
			this.grdPlanning.Columns[(int) ColsIndex.DoW ].HeaderText =
				L10n.Get( L10n.Id.HdDay );
			this.grdPlanning.Columns[ (int) ColsIndex.Date ].HeaderText =
				L10n.Get( L10n.Id.HdDate );
			this.grdPlanning.Columns[ (int) ColsIndex.Task ].HeaderText =
				L10n.Get( L10n.Id.HdTask );

			// Properties
			this.lblInitialDate.Text = L10n.Get( L10n.Id.LblInitialDate );
			this.lblSteps.Text = L10n.Get( L10n.Id.LblSteps );
			this.edInitialDate.CustomFormat = Core.Locale.CurrentLocale.DateTimeFormat.ShortDatePattern;

			this.SetStatus();
		}

		/// <summary>
		/// Deactivates the GUI. Uses EnableGui.
		/// </summary>
		private void DeactivateGui()
		{
			this.EnableGui( false );
		}

		/// <summary>
		/// Activates the GUI. Uses EnableGui.
		/// </summary>
		private void ActivateGui()
		{
			this.EnableGui( true );
		}

		/// <summary>
		/// Enables or disables the GUI.
		/// </summary>
		/// <param name='active'>
		/// Indicates whether the Gui should be enabled or disabled
		/// </param>
		private void EnableGui(bool active)
		{
			// Widgets
			this.pnlPlanning.Visible = active;

			// Menus
			this.opExport.Enabled = active;
			this.opSave.Enabled = active;
			this.opSaveAs.Enabled = active;
			this.opClose.Enabled = active;
			this.opAdd.Enabled = active;
			this.opProperties.Enabled = active;
			this.opInsertTask.Enabled = active;
			this.opInsertDate.Enabled = active;
			this.opInsert.Enabled = active;
			this.opRemove.Enabled = active;
			this.opRemoveTask.Enabled = active;
			this.opRemoveDate.Enabled = active;
			this.mView.Enabled = active;
			this.pnlConfigContainer.Hide();

			// Toolbar
			this.tbbSave.Enabled = active;
			this.tbbProperties.Enabled = active;
			this.tbbExport.Enabled = active;
			this.tbbAddRow.Enabled = active;
		}

		/// <summary>
		/// Sets the visible status to the default "Ready"
		/// </summary>
		private void SetStatus()
		{
			this.SetStatus( L10n.Get( L10n.Id.StReady ) );
		}

		/// <summary>
		/// Sets the visible status to the message passed.
		/// </summary>
		/// <param name='msg'>
		/// The new message to set as status.
		/// </param>
		private void SetStatus(string msg)
		{
			this.stbStatus.Text = msg;
			Application.DoEvents();
		}

		private void ShowNoDocumentError()
		{
			MessageBox.Show(
				this,
				L10n.Get( L10n.Id.ErNoDocument ),
				AppInfo.Name,
				MessageBoxButtons.OK,
				MessageBoxIcon.Error
			);
		}

		private void OnIncFont()
		{
			float size = this.planningFont.Size + FontStep;

			this.planningFont = new Font( this.planningFont.FontFamily, size );
			this.UpdateFont( FontStep );
			this.ChangeToListTab();
		}

		private void OnDecFont()
		{
			float size = this.planningFont.Size - FontStep;

			this.planningFont = new Font( this.planningFont.FontFamily, size );
			this.UpdateFont( FontStep * -1 );
			this.ChangeToListTab();
		}

		/// <summary>
		/// Updates the font in the planning grid.
		/// </summary>
		/// <param name='fontStep'>
		/// The difference in size between the old font and the new font.
		/// </param>
		private void UpdateFont(int fontStep)
		{
			int delta = 0;

			// Determine delta factor
			if ( fontStep != 0 ) {
				delta = ( fontStep / Math.Max( Math.Abs( fontStep ), 1 ) );
			}

			// Change grid
			this.grdPlanning.Font = this.planningFont;
			this.grdPlanning.DefaultCellStyle.Font = this.planningFont;
			this.grdPlanning.ColumnHeadersDefaultCellStyle.Font = new Font( this.planningFont, FontStyle.Bold );
			this.grdPlanning.ColumnHeadersHeight = this.planningFont.Height * 2;

			foreach(DataGridViewTextBoxColumn col in this.grdPlanning.Columns) {
				col.Width += (int) ( col.Width * ( 0.10 * delta ) );

				if ( col.Index == 0 ) {
					col.DefaultCellStyle.Font = this.planningFont;
				}
			}

			foreach(DataGridViewRow row in this.grdPlanning.Rows) {
				row.Height += (int) ( row.Height * ( 0.10 * delta ) );
			}

			this.ResizeWindow();
			return;
		}

		/// <summary>
		/// Show info about the application
		/// </summary>
		private void OnAbout()
		{
			this.pnlAbout.Show();
		}

		private void ChangeToListTab() {
			this.tabbed.SelectedIndex = 0;
		}

		/// <summary>
		/// Raises the calendar date changed event.
		/// </summary>
		private void OnCalendarDateChanged() {
			int[] taskPositionsForDate = this.doc.LookForTasksIn( this.calendar.SelectionStart );

			this.txtDesc.Clear();
			foreach(int x in taskPositionsForDate) {
				this.txtDesc.AppendText( this.doc.Tasks[x] + '\n' );
			}

			return;
		}

		/// <summary>
		/// Activates the option for editing the steps for the document.
		/// </summary>
		private void OnProperties()
		{
			if ( this.doc != null ) {
				this.edSteps.Text = String.Join( ", ", this.doc.Steps );
				this.edSteps.SelectionLength = 0;
				this.edSteps.SelectionStart = this.edSteps.TextLength;
				this.edInitialDate.Value = this.doc.InitialDate;
				this.pnlConfigContainer.Visible = true;
				this.ChangeToListTab();
				this.edSteps.Focus();
			} else {
				this.ShowNoDocumentError();
			}
		}

		/// <summary>
		/// Activates saving the steps edited by the user.
		/// </summary>
		private void OnPropertiesPanelClosed()
		{
			if ( this.doc != null ) {
				this.pnlConfigContainer.Hide();
				this.doc.Steps.SetSteps( this.edSteps.Text );
				this.doc.InitialDate = this.edInitialDate.Value;
				this.UpdatePlanning();
			}

			return;
		}

		/// <summary>
		/// Inserts a new task without changing dates.
		/// </summary>
		private void OnInsertTask()
		{
			if ( this.doc != null ) {
				int rowNumber = 0;
				var row = this.grdPlanning.CurrentRow;

				// Prepare the affected row number
				this.grdPlanning.EndEdit();
				if ( row != null ) {
					rowNumber = row.Index;
				}

				// Now do it
				this.SetStatus( L10n.Get( L10n.Id.StInserting )
					+ ": " + this.doc.Dates[ rowNumber ].ToShortDateString() );
				this.doc.InsertTask( rowNumber );
				this.UpdatePlanning( rowNumber );
				this.ChangeToListTab();
				this.SetStatus();
			} else {
				this.ShowNoDocumentError();
			}

			return;
		}

		private void OnInsertRow()
		{
			if ( this.doc != null ) {
				int rowNumber = 0;
				var row = this.grdPlanning.CurrentRow;

				// Prepare the affected row number
				this.grdPlanning.EndEdit();
				if ( row != null ) {
					rowNumber = row.Index;
				}

				// Now do it
				this.SetStatus( L10n.Get( L10n.Id.StInserting )
					+ ": " + this.doc.Dates[ rowNumber ].ToShortDateString() );

				this.doc.InsertRow( rowNumber );
				this.UpdatePlanning( rowNumber );
				this.ChangeToListTab();
				this.SetStatus();
			} else {
				this.ShowNoDocumentError();
			}

			return;
		}

		private void OnInsertDate()
		{
			if ( this.doc != null ) {
				int rowNumber = 0;
				var row = this.grdPlanning.CurrentRow;

				// Prepare the affected row number
				this.grdPlanning.EndEdit();
				if ( row != null ) {
					rowNumber = row.Index;
				}

				// Now do it
				this.SetStatus( L10n.Get( L10n.Id.StInserting )
					+ ": " + this.doc.Dates[ rowNumber ].ToShortDateString() );

				this.doc.InsertDate( rowNumber );
				this.UpdatePlanning( rowNumber );
				this.ChangeToListTab();
				this.SetStatus();
			} else {
				this.ShowNoDocumentError();
			}

			return;
		}

		private void OnTabChanged()
		{
			if ( this.tabbed.SelectedIndex == 1 ) {
				// Show the days in the calendar
				DateTime[] boldedDates = new DateTime[ this.doc.CountDates ];

				Console.WriteLine( "Preparing calendar" );
				this.calendar.RemoveAllBoldedDates();
				this.doc.Dates.CopyTo( boldedDates, 0 );
				this.calendar.BoldedDates = boldedDates;
				this.calendar.UpdateBoldedDates();
			}

			return;
		}

		/// <summary>
		/// Remove a whole row
		/// </summary>
		private void OnRemove()
		{
			if ( this.doc != null ) {
				var row = this.grdPlanning.CurrentRow;				
				int rowNumber = 0;
				string strDate = this.doc.InitialDate.ToShortDateString();
				string strTask = Document.TaskTag;

				this.grdPlanning.EndEdit();	
				
				if ( row != null ) {
					rowNumber = row.Index;
				}
				
				if ( this.grdPlanning.Rows.Count > 0 ) {
					strDate = this.grdPlanning.Rows[ rowNumber ].Cells[ (int) ColsIndex.Date ].ToString();
					strTask = this.grdPlanning.Rows[ rowNumber ].Cells[ (int) ColsIndex.Task ].ToString();
					
					this.SetStatus( L10n.Get( L10n.Id.StRemoving )
									+ ": " + strDate + "/" + strTask );
					this.doc.Remove( rowNumber );
					this.UpdatePlanning( rowNumber );
					this.ChangeToListTab();
					this.SetStatus();
				}
			} else {
				this.ShowNoDocumentError();
			}

			return;
		}

		private void OnRemoveTask()
		{
			if ( this.doc != null ) {
				int rowNumber = 0;
				var row = this.grdPlanning.CurrentRow;
				string strDate = this.doc.InitialDate.ToShortDateString();

				this.grdPlanning.EndEdit();

				if ( row != null ) {
					rowNumber = row.Index;
					strDate = this.grdPlanning.Rows[ rowNumber ].Cells[ (int) ColsIndex.Date ].ToString();
				}

				if ( this.grdPlanning.Rows.Count > 0 ) {
					this.SetStatus(  L10n.Get( L10n.Id.StRemoving ) 
						+ ": " + strDate );
					this.doc.RemoveTask( rowNumber );
					this.UpdatePlanning( rowNumber );
					this.ChangeToListTab();
					this.SetStatus();
				}
			} else {
				this.ShowNoDocumentError();
			}

			return;
		}

		private void OnRemoveDate()
		{
			if ( this.doc != null ) {
				int rowNumber = 0;
				var row = this.grdPlanning.CurrentRow;
				string strDate = this.doc.InitialDate.ToShortDateString();

				this.grdPlanning.EndEdit();

				if ( row != null ) {
					rowNumber = row.Index;
					strDate = this.grdPlanning.Rows[ rowNumber ].Cells[ (int) ColsIndex.Date ].ToString();
				}

				if ( grdPlanning.Rows.Count > 0 ) {
					this.SetStatus( L10n.Get( L10n.Id.StRemoving )
						+ ": " + strDate );
					this.doc.RemoveDate( rowNumber );
					this.UpdatePlanning( rowNumber );
					this.ChangeToListTab();
					this.SetStatus();
				}
			} else {
				this.ShowNoDocumentError();
			}

			return;
		}

		/// <summary>
		/// Adds a new row to the planning
		/// </summary>
		private void OnAdd()
		{
			if ( this.doc != null ) {
				int rowNumber = this.doc.CountDates;
				this.SetStatus( L10n.Get( L10n.Id.StInserting )
					+ ": " + ( rowNumber + 1 ).ToString() );

				// Prepare the document
				this.doc.AddLast();

				// Prepare the UI
				this.grdPlanning.Rows.Add();
				this.UpdatePlanningRow( rowNumber );
				this.ChangeToListTab();
				this.SetStatus();
			} else {
				this.ShowNoDocumentError();
			}

			return;
		}

		private void UpdatePlanning()
		{
			this.UpdatePlanning( 0 );
		}

		private void UpdatePlanning(int numRow)
		{
			if ( this.doc != null ) {
				// Creates & updates rows
				for (int i = numRow; i < this.doc.CountDates; ++i) {
					if ( this.grdPlanning.Rows.Count <= i ) {
						this.grdPlanning.Rows.Add();
					}

					this.UpdatePlanningRow( i );
				}

				// Remove padding rows
				try {
					int numExtraRows = this.grdPlanning.Rows.Count - this.doc.CountDates;
					for(; numExtraRows > 0 ; --numExtraRows) {					
						this.grdPlanning.Rows.RemoveAt( this.grdPlanning.Rows.Count - 1 );
					}
				} catch(ArgumentException exc) {
					System.Diagnostics.Debug.WriteLine( "Removing: " + numRow + ": " + exc.Message );
				}
			}

			return;
		}

		private void UpdatePlanningRow(int rowIndex)
		{
			DateTimeFormatInfo dtfo = Locale.CurrentLocale.DateTimeFormat;

			if ( rowIndex < 0
			  || rowIndex > this.grdPlanning.Rows.Count )
			{
				throw new ArgumentException( L10n.Get( L10n.Id.ErOutOfRange )
					+ ": " + rowIndex );
			}

			DataGridViewRow row = this.grdPlanning.Rows[ rowIndex ];

			row.Cells[ (int) ColsIndex.Num ].Value =
				( rowIndex + 1 ).ToString().PadLeft( 4, ' ' );
			row.Cells[ (int) ColsIndex.DoW ].Value =
				dtfo.GetAbbreviatedDayName( this.doc.GetDate( rowIndex ).DayOfWeek );
			row.Cells[ (int) ColsIndex.Date ].Value =
				this.doc.GetDate( rowIndex ).ToShortDateString();
			row.Cells[ (int) ColsIndex.Task ].Value =
				this.doc.GetTask( rowIndex );

			return;
		}

		/// <summary>
		/// Gets the keyed value for date and task, and prepares and assigns them.
		/// </summary>
		/// <param name="row">The row index the edited cell sits in.</param>
		private void OnRowEdited(int row)
		{
			string strDate = (string)this.grdPlanning.Rows [row].Cells [(int)ColsIndex.Date].Value;
			string strTask = (string) this.grdPlanning.Rows[ row ].Cells[ (int) ColsIndex.Task ].Value;

			// Convert date
			if ( strDate == null
			  || strDate.Trim().Length == 0 )
			{
				strDate = "";
			}

			if ( strTask == null
			  || strTask.Trim().Length == 0 )
			{
				strTask = Document.TaskTag + ( row + 1 ).ToString();
			}

			if ( !DateTime.TryParse( strDate, out DateTime date ) ) {
				date = this.doc.InitialDate;
			}

			// Update
			strDate = date.ToShortDateString();
			this.grdPlanning.Rows[ row ].Cells[ (int) ColsIndex.Date ].Value = strDate;
			this.grdPlanning.Rows[ row ].Cells[ (int) ColsIndex.Task ].Value = strTask;

			// Store
			this.doc.Modify( row, date, strTask );
			return;
		}

		/// <summary>
		/// Exports the data to other formats.
		/// </summary>
		private void OnExport()
		{
			if ( this.doc != null ) {		
				var dlgExport = new ExportDlg( this.Icon, this.doc, filePath );
			
				DialogResult result = dlgExport.ShowDialog( this );
				filePath = dlgExport.Path;
				
				if ( result == DialogResult.OK ) {
					DocumentExporter.Create( dlgExport.ExportInfo ).Save();
				}
			} else {
				this.ShowNoDocumentError();
			}

			return;
		}

		private void PrepareNewDocument()
		{
			this.doc = new Document();
		}

		private void OnNew()
		{
			this.OnClose();
			this.ActivateGui();
			this.PrepareNewDocument();
			this.ChangeToListTab();
			this.SetStatus();
			this.SetWindowTitle( "nonamed.bar" );
		}

		private void OnClose()
		{
			if ( this.doc != null
			  && this.doc.NeedsSaving )
			{
				this.OnSave();
			}
			
			this.DeactivateGui();
			this.doc = null;
			this.grdPlanning.Rows.Clear();
			this.ChangeToListTab();
			this.SetStatus();
			this.SetWindowTitle( "" );
		}

		private void OnOpen()
		{
			var dlgOpenFile = new OpenFileDialog();

			this.OnClose();

			try {
				dlgOpenFile.Title = AppInfo.Name;
				dlgOpenFile.DefaultExt = XmlDocumentPersistence.DefaultExt;
				dlgOpenFile.CheckPathExists = true;
				dlgOpenFile.CheckFileExists = true;
				dlgOpenFile.Filter = AppInfo.Name
					+ " (*." + XmlDocumentPersistence.DefaultExt + ")"
					+ "|*." + XmlDocumentPersistence.DefaultExt;
				dlgOpenFile.InitialDirectory = filePath;

				if ( dlgOpenFile.ShowDialog() == DialogResult.OK ) {
					this.filePath = Path.GetDirectoryName( dlgOpenFile.FileName );
					DocumentPersistence docLoader = new XmlDocumentPersistence( null );
					this.doc = docLoader.Load( dlgOpenFile.FileName );
					this.doc.FileName = dlgOpenFile.FileName;
					this.SetWindowTitle( this.doc.FileName );

					this.UpdatePlanning();
					this.ChangeToListTab();
					this.ActivateGui();
				}
			}
			catch(XmlException exc)
			{
				MessageBox.Show(
					this,
					L10n.Get( L10n.Id.ErManagingXML )
					+ ": " + exc.Message,
					AppInfo.Name,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}
			catch(IOException exc)
			{
				MessageBox.Show(
					this,
					L10n.Get( L10n.Id.ErAccessingFile )
					+ ": " + exc.Message,
					AppInfo.Name,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}
			finally {
				this.SetStatus();
			}

			return;
		}
		
		private void SetWindowTitle(string fileName)
		{
			if ( !string.IsNullOrWhiteSpace( fileName ) ) {
				this.Text = Path.GetFileName( fileName ) + " - " + AppInfo.Name;
			} else {
				this.Text = AppInfo.Name;
			}
			
			return;
		}

		private void Save(string fileName)
		{
			try {
				if ( this.doc != null ) {
					this.SetWindowTitle( fileName );
					this.doc.FileName = fileName;
					DocumentPersistence docSaver = new XmlDocumentPersistence( this.doc );
					docSaver.Save( fileName );
					this.ActivateGui();
				}
			}
			catch(XmlException exc)
			{
				MessageBox.Show(
					this,
					L10n.Get( L10n.Id.ErManagingXML )
					+ ": " + exc.Message,
					AppInfo.Name,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}
			catch(IOException exc)
			{
				MessageBox.Show(
					this,
					L10n.Get( L10n.Id.ErAccessingFile )
					+ ": " + exc.Message,
					AppInfo.Name,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}
			finally {
				this.SetStatus();
			}

			return;
		}

		private void OnSave()
		{
			if ( this.doc != null ) {
				bool saveIt = true;
				string fileName = this.doc.FileName;

				if ( !this.doc.HasName ) {
					saveIt = false;
					var dlgSaveFile = new SaveFileDialog () {
						Title = AppInfo.Name,
						DefaultExt = XmlDocumentPersistence.DefaultExt,
						CheckPathExists = true,
						CheckFileExists = false,
						Filter = AppInfo.Name
						+ " (*." + XmlDocumentPersistence.DefaultExt + ")"
						+ "|*." + XmlDocumentPersistence.DefaultExt,
						InitialDirectory = filePath
					};
					
					if ( dlgSaveFile.ShowDialog( ) == DialogResult.OK ) {
						saveIt = true;
						fileName = dlgSaveFile.FileName;
					}
				}

				if ( saveIt ) {
					this.Save( fileName );
				}
			}

			return;
		}

		private void OnSaveAs()
		{
			if ( this.doc != null ) {
				var dlgSaveFile = new SaveFileDialog () {
					Title = AppInfo.Name,
					DefaultExt = XmlDocumentPersistence.DefaultExt,
					CheckPathExists = true,
					CheckFileExists = false,
					Filter = AppInfo.Name
					+ " (*." + XmlDocumentPersistence.DefaultExt + ")"
					+ "|*." + XmlDocumentPersistence.DefaultExt,
					InitialDirectory = filePath
				};
				
				if (dlgSaveFile.ShowDialog() == DialogResult.OK ) {
					this.filePath = dlgSaveFile.FileName;
					this.Save( this.filePath );
				}
			}

			return;
		}

		/// <summary>
		/// Quit the application
		/// </summary>
		private void OnQuit()
		{
			this.OnClose();
			this.WriteConfiguration();
			Application.Exit();
		}

		private void OnSettings()
		{
			this.cbLocales.Text = Locale.CurrentLocaleToDescription();

			this.pnlSettings.Show();
		}

		private void ChangeSettings()
		{
			Locale.SetLocaleFromDescription( this.cbLocales.Text );
			this.ChangeUILanguage( Locale.CurrentLocale );
			this.pnlSettings.Hide();
			this.UpdatePlanning();
		}

		protected void ReadConfiguration()
		{
			string line;
			StreamReader file = null;
			Size currentSize = this.Size;

			// Set current values for height and width, in case they are not set.
			this.CfgWidth = currentSize.Width;
			this.CfgHeight = currentSize.Height;

			try {
				try {
					file = new StreamReader( this.CfgFile );
				} catch(Exception) {
					var fileCreate = new StreamWriter( this.CfgFile );
					fileCreate.Close();
					file = new StreamReader( this.CfgFile );
				}

				line = file.ReadLine();
				while( !file.EndOfStream ) {
					int pos = line.IndexOf( '=' );
					string arg = line.Substring( pos + 1 ).Trim();

					if ( line.ToLower().StartsWith( EtqWidth, StringComparison.CurrentCultureIgnoreCase ) )
					{
						if ( pos > 0 ) {
							this.CfgWidth = Convert.ToInt32( arg );
						}
					}
					else
						if ( line.ToLower().StartsWith( EtqHeight, StringComparison.CurrentCultureIgnoreCase ) )
						{
							if ( pos > 0 ) {
								this.CfgHeight = Convert.ToInt32( arg );
							}
						}
						else
							if ( line.ToLower().StartsWith( EtqLocale, StringComparison.CurrentCultureIgnoreCase ) )
							{
								if ( pos > 0 ) {
									Locale.SetLocale( arg );
								}
							}

					line = file.ReadLine();
				}

				file.Close();
			} catch(Exception exc)
			{
				MessageBox.Show(
					this,
					string.Format( "{0}:\n{1}",
						L10n.Get( L10n.Id.StReadingConfig ),
						exc.Message ),
					AppInfo.Name,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}
		}

		protected void WriteConfiguration()
		{
			Size currentSize = this.Size;
			int width = currentSize.Width;
			int height = currentSize.Height;

			// Write configuration
			try {
				var file = new StreamWriter( this.CfgFile );
				file.WriteLine( "{0}={1}", EtqWidth, width );
				file.WriteLine( "{0}={1}", EtqHeight, height );
				file.WriteLine( "{0}={1}", EtqLocale, Locale.GetCurrentLocaleCode() );
				file.WriteLine();
				file.Close();
			} catch(Exception exc)
			{
				MessageBox.Show(
					this,
					string.Format( "{0}:\n{1}",
						L10n.Get( L10n.Id.StWritingConfig ),
						exc.Message ),
					AppInfo.Name,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}
		}

		/// <summary>
		/// Returns the path to the config file in the user profile's directory.
		/// </summary>
		/// <value>The complete path.</value>
		public string CfgFile
		{
			get {
				if ( this.cfgFile.Trim().Length == 0 ) {
					var cfgPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
					this.cfgFile = Path.Combine( cfgPath, CfgFileName );
				}

				return cfgFile;
			}
		}

		/// <summary>
		/// Gets or sets the height of the window to be stored in the cfg file.
		/// </summary>
		/// <value>The height of the main window.</value>
		public int CfgHeight {
			get; set;
		}

		/// <summary>
		/// Gets or sets the width of the window to be stored in the cfg file.
		/// </summary>
		/// <value>The width of the main window.</value>
		public int CfgWidth {
			get; set;
		}

		private Document doc;
		private string filePath;
		private string cfgFile;

	}
}

