// Bareplan (c) 2015-17 MIT License <baltasarq@gmail.com>

namespace Bareplan.Gui {
	using System;
	using System.Drawing;
	using System.Windows.Forms;
	using System.Globalization;
	using System.Diagnostics;
	
	using Bareplan.Core;
	
	public partial class MainWindow: Form {
		/// <summary>Fonts are incremented by 2 units each time.</summary>
		public const int FontStep = 2;
		/// <summary>Represents the order of columns in the gridview.</summary>
		public enum ColsIndex { Num, DoW, Date, Kind, Contents };
		/// <summary>Tag in the configuration file for the width of the window.</summary>
		public const string EtqWidth  = "width";
		/// <summary>Tag in the configuration file for the height of the window.</summary>
		public const string EtqHeight = "height";
		/// <summary>Tag in the configuration file for the locale of the app.</summary>
		public const string EtqLocale = "locale";
		/// <summary>The name of the confif file.</summary>
		public const string CfgFileName = ".bareplan.cfg";
		/// <summary>Strings for the context menu.</summary>
		public static readonly Func<string>[] GetContextItemCaptions = {
				() => L10n.Get( L10n.Id.OpAdd ),
		    	() => L10n.Get( L10n.Id.OpInsert ),
		    	() => L10n.Get( L10n.Id.OpInsertDate ),
		    	() => L10n.Get( L10n.Id.OpInsertTask ),
		    	() => "-",
		    	() => L10n.Get( L10n.Id.OpRemove ),
		    	() => L10n.Get( L10n.Id.OpRemoveDate ),
		    	() => L10n.Get( L10n.Id.OpRemoveTask )
		};
		/// <summary>Actions for the context menu.</summary>
		public static readonly Func<MainWindow, EventHandler>[] GetContextItemActions = {
				(MainWindow mw) => (sender, e) => mw.OnAdd(),
			    (MainWindow mw) => (sender, e) => mw.OnInsertRow(),
			    (MainWindow mw) => (sender, e) => mw.OnInsertDate(),
			    (MainWindow mw) => (sender, e) => mw.OnInsertTask(),
			    (MainWindow mw) => (sender, e) => {},
			    (MainWindow mw) => (sender, e) => mw.OnRemove(),
			    (MainWindow mw) => (sender, e) => mw.OnRemoveDate(),
			    (MainWindow mw) => (sender, e) => mw.OnRemoveTask()
		};

		private void BuildContextMenu()
		{
			// Menu item's captions
			if ( this.cntxtMenu == null ) {
				this.cntxtMenu = new ContextMenu();
				this.grdPlanning.ContextMenu = this.cntxtMenu;
				
				foreach(Func<string> getContextItemCaption in GetContextItemCaptions) {
					this.cntxtMenu.MenuItems.Add( getContextItemCaption() );
				}
			} else {
				for(int i = 0; i < GetContextItemCaptions.Length; ++i) {
					this.cntxtMenu.MenuItems[ i ].Text = GetContextItemCaptions[ i ]();
				}
			}
			
			// Menu Item's actions
			for(int i = 0; i < GetContextItemActions.Length; ++i) {
				this.cntxtMenu.MenuItems[ i ].Click += GetContextItemActions[ i ]( this );
			}
		}

		private void BuildCalendarTab(Panel panel)
		{
			this.calendar = new MonthCalendar() {
				Dock = DockStyle.Top,
				ShowToday = false,
				ShowTodayCircle = false,
				MaxSelectionCount = 1,
				ScrollChange = 1
			};

			this.calendar.DateSelected += (sender, e) => this.OnCalendarDateChanged();

			this.txtDesc = new TextBox() {
				Dock = DockStyle.Fill,
				ReadOnly = true,
				Multiline = true
			};

			panel.Dock = DockStyle.Fill;
			panel.Controls.Add( this.txtDesc );
			panel.Controls.Add( this.calendar );
		}

		private void BuildPlanning()
		{
			// Prepare the list of events
			this.grdPlanning = new DataGridView {
				AllowUserToResizeRows = false,
				RowHeadersVisible = false,
				AutoGenerateColumns = false,
				MultiSelect = false,
				AllowUserToAddRows = false,
			};
			this.grdPlanning.RowHeadersDefaultCellStyle.BackColor = Color.LightGray;
			this.grdPlanning.RowHeadersDefaultCellStyle.ForeColor = Color.Black;
			this.grdPlanning.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
			this.grdPlanning.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
			this.planningFont = new Font( this.grdPlanning.Font, FontStyle.Regular );

			var textCellTemplate0 = new DataGridViewTextBoxCell();
			var textCellTemplate1 = new DataGridViewTextBoxCell();
			var textCellTemplate2 = new DataGridViewTextBoxCell();
			var textCellTemplate3 = new DataGridViewTextBoxCell();
			var textCellTemplate4 = new DataGridViewTextBoxCell();

			textCellTemplate0.Style.BackColor = this.grdPlanning.RowHeadersDefaultCellStyle.BackColor;
			textCellTemplate0.Style.ForeColor = this.grdPlanning.RowHeadersDefaultCellStyle.ForeColor;
			textCellTemplate0.Style.Font = new Font( this.planningFont, FontStyle.Bold );
			textCellTemplate1.Style.BackColor = this.grdPlanning.RowHeadersDefaultCellStyle.BackColor;
			textCellTemplate1.Style.ForeColor = this.grdPlanning.RowHeadersDefaultCellStyle.ForeColor;
			textCellTemplate1.Style.Font = new Font( this.planningFont, FontStyle.Bold );
			textCellTemplate2.Style.BackColor = Color.Wheat;
			textCellTemplate2.Style.ForeColor = Color.Black;
			textCellTemplate3.Style.BackColor = Color.White;
			textCellTemplate3.Style.ForeColor = Color.Navy;
			textCellTemplate4.Style.BackColor = Color.White;
			textCellTemplate4.Style.ForeColor = Color.Navy;
			var column0 = new DataGridViewTextBoxColumn {
				HeaderText = "#",
				Width = 25,
				ReadOnly = true,
				CellTemplate = textCellTemplate0,
				SortMode = DataGridViewColumnSortMode.NotSortable
			};
			var column1 = new DataGridViewTextBoxColumn {
				HeaderText = "{*}",
				Width = 25,
				ReadOnly = true,
				CellTemplate = textCellTemplate1,
				SortMode = DataGridViewColumnSortMode.NotSortable
			};
			var column2 = new DataGridViewTextBoxColumn {
				HeaderText = "Fecha",
				Width = 75,
				CellTemplate = textCellTemplate2,
				SortMode = DataGridViewColumnSortMode.NotSortable
			};
			var column3 = new DataGridViewTextBoxColumn {
				HeaderText = "Tipo",
				Width = 250,
				CellTemplate = textCellTemplate3,
				SortMode = DataGridViewColumnSortMode.NotSortable
			};
			var column4 = new DataGridViewTextBoxColumn {
				HeaderText = "Tarea",
				Width = 250,
				CellTemplate = textCellTemplate4,
				SortMode = DataGridViewColumnSortMode.NotSortable
			};
			
			this.grdPlanning.Columns.AddRange( new DataGridViewColumn[] {
				column0,
				column1,
				column2,
				column3,
				column4
			} );

			this.grdPlanning.CellEndEdit += (object dest, DataGridViewCellEventArgs args) => 
				this.OnRowEdited( args.RowIndex );

			this.grdPlanning.Dock = DockStyle.Fill;
			this.grdPlanning.TabIndex = 3;
			this.grdPlanning.AllowUserToOrderColumns = false;

			// Create tabbed control
			this.tabbed = new TabControl { Dock = DockStyle.Fill, Alignment = TabAlignment.Top };
			this.tabbed.ImageList = new ImageList();
			this.tabbed.ImageList.Images.AddRange( new Image[]{ this.listIcon, this.calendarViewIcon } );
			this.tabbed.ImageList.ImageSize = new Size( 16, 16 );
			var tabPlanning = new TabPage { Text = L10n.Get( L10n.Id.HdSession ), ImageIndex = 0 };
			var tabCalendar = new TabPage { Text = L10n.Get (L10n.Id.HdDate), ImageIndex = 1 };
			tabPlanning.Controls.Add( this.grdPlanning );
			var pnlCalendar = new Panel();
			this.BuildCalendarTab( pnlCalendar );
			tabCalendar.Controls.Add( pnlCalendar );
			this.tabbed.TabPages.Add( tabPlanning );
			this.tabbed.TabPages.Add( tabCalendar );
			this.tabbed.SelectedIndex = 0;
			this.tabbed.SelectedIndexChanged += (obj, evt) => {
				this.OnTabChanged();
			};

			// Layout
			this.pnlPlanning = new Panel { Dock = DockStyle.Fill };
			this.pnlPlanning.SuspendLayout();
			this.pnlPlanning.Controls.Add( this.tabbed );
		}

		private void BuildPropertiesContainer()
		{
			// Sizes
			Graphics grf = this.CreateGraphics();
			SizeF fontSize = grf.MeasureString( "M", this.defaultFont );
			int charSize = (int) fontSize.Width + 5;

			// Panel
			this.pnlConfigContainer = new Panel {
				Dock = DockStyle.Bottom,
				BackColor = Color.FloralWhite,
				ForeColor = Color.Black
			};
			this.pnlConfigContainer.SuspendLayout();

			// Inner panel
			var pnlInner = new Panel { Dock = DockStyle.Fill };
			pnlInner.SuspendLayout();
			var somePadding = new Padding { All = 10 };
			pnlInner.Padding = somePadding;

			// Steps editing
			var pnlSteps = new Panel { Dock = DockStyle.Left };
			pnlSteps.SuspendLayout();
			var btBuilder = new Button { Text = "...", Dock = DockStyle.Right, FlatStyle = FlatStyle.Flat };
			btBuilder.FlatAppearance.BorderSize = 0;
			this.lblSteps = new Label { Text = "Pasos:", Dock = DockStyle.Left} ;
			this.edSteps = new TextBox { Dock = DockStyle.Fill, ReadOnly = true };

			// End when pressing enter
			this.edSteps.KeyDown += (object obj, KeyEventArgs args) => {
				if ( args.KeyCode == Keys.Enter ) {
					this.OnPropertiesPanelClosed();
				}
			};

			// Opens a GUI helper
			btBuilder.Click += (sender, e) => {
				var dlg = new StepsBuilder( this, this.edSteps.Text );
				if ( dlg.ShowDialog() == DialogResult.OK ) {
					this.edSteps.Text = dlg.Result;
				}
			};

			pnlSteps.Controls.Add( this.edSteps );
			pnlSteps.Controls.Add( this.lblSteps );
			pnlSteps.Controls.Add( btBuilder );
			pnlSteps.ResumeLayout( true );

			// Initial date
			var pnlDate = new Panel { Dock = DockStyle.Right };
			pnlDate.SuspendLayout();
			this.lblInitialDate = new Label { Text = "Fecha inicial:", Dock = DockStyle.Left };
			this.edInitialDate = new DateTimePicker {
				Dock = DockStyle.Fill,
				Format = DateTimePickerFormat.Custom,
				CustomFormat = Locale.CurrentLocale.DateTimeFormat.ShortDatePattern
			};
			pnlDate.Controls.Add( this.edInitialDate );
			pnlDate.Controls.Add( this.lblInitialDate );
			pnlDate.ResumeLayout( false );

			// Button for hiding the panel
			var btCloseConfigContainer = new Button {
				Text = "X",
				Dock = DockStyle.Right,
				Font = new Font( this.defaultFont, FontStyle.Bold ),
				Width = charSize * 5,
				FlatStyle = FlatStyle.Flat
			};
			btCloseConfigContainer.FlatAppearance.BorderSize = 0;
			btCloseConfigContainer.Click += (object obj, EventArgs args) => {
				this.OnPropertiesPanelClosed();
			};

			// Adding controls
			pnlInner.Controls.Add( pnlSteps );
			pnlInner.Controls.Add( pnlDate );
			pnlInner.ResumeLayout( true );
			this.pnlConfigContainer.Controls.Add( pnlInner );
			this.pnlConfigContainer.Controls.Add( btCloseConfigContainer );
			this.pnlConfigContainer.ResumeLayout( true );

			// Finishing
			this.pnlPlanning.Controls.Add( this.pnlConfigContainer );
			this.pnlConfigContainer.MaximumSize = new Size( int.MaxValue, (int) fontSize.Height * 2 );
			btBuilder.Size = btBuilder.MaximumSize = new Size(
					(int) fontSize.Width * btBuilder.Text.Length,
					this.edSteps.Height
			);
			lblSteps.Size = new Size( (int) fontSize.Width * this.lblSteps.Text.Length, (int) fontSize.Height * 2 );
			this.pnlConfigContainer.Hide();
		}

		private void BuildMainMenu()
		{
			// Menu options
			this.mMain = new MainMenu();
			this.mFile = new MenuItem( "Archivo" );
			this.mHelp = new MenuItem( "Ayuda" );
			this.mEdit = new MenuItem( "Editar" );
			this.opQuit = new MenuItem( "Salir" );
			this.opAbout = new MenuItem( "Acerca de..." );
			this.mView = new MenuItem( "Ver" );
			this.opExport = new MenuItem( "Exportar" );
			this.opOpen = new MenuItem( "Abrir" );
			this.opNew = new MenuItem( "Nuevo" );
			this.opSave = new MenuItem( "Guardar" );
			this.opSaveAs = new MenuItem( "Guardar como..." );
			this.opClose = new MenuItem( "Cerrar" );
			this.opAdd = new MenuItem( "Agregar fila" );
			this.opProperties = new MenuItem( "Propiedades" );
			this.opInsertTask = new MenuItem( "Insertar tarea" );
			this.opInsert = new MenuItem( "Insertar fila" );
			this.opInsertDate = new MenuItem( "Insertar fecha" );
			this.opRemove = new MenuItem( "Eliminar fila" );
			this.opRemoveTask = new MenuItem( "Eliminar tarea" );
			this.opRemoveDate = new MenuItem( "Eliminar fecha" );
			this.opIncFont = new MenuItem( "Incrementar fuente" );
			this.opDecFont = new MenuItem( "Decrementar fuente" );
			this.opViewCalendar = new MenuItem( "Ver calendario" );
			this.opViewSessions = new MenuItem( "Ver sesiones" );
			this.opSettings = new MenuItem( "Preferencias" );

			// Build the menu
			this.mMain.MenuItems.Add( this.mFile );
			this.mMain.MenuItems.Add( this.mView );
			this.mMain.MenuItems.Add( this.mEdit );
			this.mMain.MenuItems.Add( this.mHelp );
			this.mFile.MenuItems.Add ( this.opNew );
			this.mFile.MenuItems.Add ( this.opOpen );
			this.mFile.MenuItems.Add ( this.opSave );
			this.mFile.MenuItems.Add ( this.opSaveAs );
			this.mFile.MenuItems.Add ( this.opExport );
			this.mFile.MenuItems.Add ( this.opClose );
			this.mFile.MenuItems.Add( this.opQuit );
			this.mView.MenuItems.Add( this.opViewSessions );
			this.mView.MenuItems.Add( this.opViewCalendar );
			this.mView.MenuItems.Add( "-" );
			this.mView.MenuItems.Add( this.opDecFont );
			this.mView.MenuItems.Add( this.opIncFont );
			this.mEdit.MenuItems.Add( this.opAdd );
			this.mEdit.MenuItems.Add( "-" );
			this.mEdit.MenuItems.Add( this.opInsert );
			this.mEdit.MenuItems.Add( this.opInsertTask );
			this.mEdit.MenuItems.Add( this.opInsertDate );
			this.mEdit.MenuItems.Add( "-" );
			this.mEdit.MenuItems.Add( this.opRemove );
			this.mEdit.MenuItems.Add( this.opRemoveTask );
			this.mEdit.MenuItems.Add( this.opRemoveDate );
			this.mEdit.MenuItems.Add( "-" );
			this.mEdit.MenuItems.Add( this.opProperties );
			this.mEdit.MenuItems.Add( this.opSettings );
			this.mHelp.MenuItems.Add( this.opAbout );

			// Shortcuts
			this.opQuit.Shortcut = Shortcut.CtrlQ;
			this.opNew.Shortcut = Shortcut.CtrlN;
			this.opOpen.Shortcut = Shortcut.CtrlO;
			this.opSave.Shortcut = Shortcut.CtrlS;
			this.opAdd.Shortcut = Shortcut.CtrlIns;
			this.opProperties.Shortcut = Shortcut.F2;

			// Events
			this.opQuit.Click += ( obj, evt ) => this.OnQuit();
			this.opAbout.Click += ( obj, evt ) => this.OnAbout();
			this.opExport.Click += ( obj, evt ) => this.OnExport();
			this.opNew.Click += ( obj, evt ) => this.OnNew();
			this.opOpen.Click += ( obj, evt ) => this.OnOpen();
			this.opSave.Click += ( obj, evt ) => this.OnSave();
			this.opSaveAs.Click += ( obj, evt ) => this.OnSaveAs();
			this.opClose.Click += ( obj, evt ) => this.OnClose();
			this.opAdd.Click += ( obj, evt ) => this.OnAdd();
			this.opProperties.Click += ( obj, evt ) => this.OnProperties();
			this.opInsert.Click += (sender, e) => this.OnInsertRow();
			this.opInsertTask.Click += ( obj, evt ) => this.OnInsertTask();
			this.opInsertDate.Click += (sender, e) => this.OnInsertDate();
			this.opRemove.Click += ( obj, evt ) => this.OnRemove();
			this.opRemoveTask.Click += ( obj, evt ) => this.OnRemoveTask();
			this.opRemoveDate.Click += ( obj, evt ) => this.OnRemoveDate();
			this.opIncFont.Click += ( obj, evt ) => this.OnIncFont();
			this.opDecFont.Click += ( obj, evt ) => this.OnDecFont();
			this.opViewSessions.Click += ( obj, evt ) => this.ChangeToSessionsTab();
			this.opViewCalendar.Click += ( obj, evt ) => this.ChangeToCalendarTab();
			this.opSettings.Click += ( obj, evt ) => this.OnSettings();

			this.Menu = this.mMain;
		}

		private void BuildAboutPanel()
		{
			// Sizes
			Graphics grf = this.CreateGraphics();
			SizeF fontSize = grf.MeasureString( "M", this.defaultFont );
			int charSize = (int) fontSize.Width + 5;

			// Panel for about info
			this.pnlAbout = new Panel() {
				Dock = DockStyle.Bottom,
				BackColor = Color.LightYellow,
				ForeColor = Color.Black
			};
			this.pnlAbout.SuspendLayout();
			this.lblAbout = new Label {
				Text = AppInfo.Name + " v" + AppInfo.Version + ", " + AppInfo.Author,
				Dock = DockStyle.Left,
				TextAlign = ContentAlignment.MiddleCenter,
				AutoSize = true,
				Font = new Font( this.defaultFont, FontStyle.Bold )
			};
			var btCloseAboutPanel = new Button() {
				Text = "X",
				Dock = DockStyle.Right,
				Width = charSize * 5,
				FlatStyle = FlatStyle.Flat,
				Font = new Font( this.defaultFont, FontStyle.Bold )
			};
			btCloseAboutPanel.FlatAppearance.BorderSize = 0;
			btCloseAboutPanel.Click += (o, evt) => this.pnlAbout.Hide();
			this.pnlAbout.Controls.Add( lblAbout );
			this.pnlAbout.Controls.Add( btCloseAboutPanel );
			this.pnlAbout.Hide();
			this.pnlAbout.MinimumSize = new Size( this.Width, this.lblAbout.Height + 5 );
			this.pnlAbout.MaximumSize = new Size( Int32.MaxValue, this.lblAbout.Height + 5 );

			this.pnlAbout.ResumeLayout( false );
		}

		private void BuildSettingsPanel()
		{
			this.pnlSettings = new TableLayoutPanel {
				BackColor = Color.White,
				ForeColor = Color.Black,
				Dock = DockStyle.Bottom,
				ColumnCount = 1,
				GrowStyle = TableLayoutPanelGrowStyle.AddRows
			};
			this.pnlSettings.SuspendLayout();

			// Button
			var btClose = new Button {
				BackColor = Color.White,
				Text = "X",
				Anchor = AnchorStyles.Right,
				Font = new Font( Font, FontStyle.Bold ),
				FlatStyle = FlatStyle.Flat
			};
			btClose.FlatAppearance.BorderSize = 0;
			btClose.Click += (sender, e) => this.ChangeSettings();
			this.pnlSettings.Controls.Add( btClose );

			// Locale
			var pnlLocales = new Panel {
				Margin = new Padding( 5 ),
				Dock = DockStyle.Top
			};
			this.lblLocales = new Label {
				Text = L10n.Get( L10n.Id.LblLanguage ),
				Dock = DockStyle.Left
			};

			this.cbLocales = new ComboBox() {
				ForeColor = Color.Black,
				BackColor = Color.White,
				Dock = DockStyle.Fill,
				DropDownStyle = ComboBoxStyle.DropDownList,
				Text = Locale.CurrentLocale.ToString()
			};
			CultureInfo[] locales = CultureInfo.GetCultures( CultureTypes.SpecificCultures );
			Array.Sort( locales,
				((CultureInfo x, CultureInfo y) =>
					String.Compare( x.ToString(), y.ToString(), false, CultureInfo.InvariantCulture ) )
			);

			this.cbLocales.Items.Add( "<local>" );
			foreach(CultureInfo locale in locales ) {
				this.cbLocales.Items.Add( locale.NativeName + ": " + locale.ToString() );
			}

			pnlLocales.Controls.Add( this.cbLocales );
			pnlLocales.Controls.Add( this.lblLocales );
			pnlLocales.MaximumSize = new Size( Int32.MaxValue, cbLocales.Height );
			this.pnlSettings.Controls.Add( pnlLocales );

			this.pnlSettings.ResumeLayout( false );
			this.pnlSettings.Hide();
			return;
		}

		private void BuildIcons()
		{
			try {
				this.bmpAppIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "Bareplan.Res.appIcon.png" ) );

				this.addRowIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "Bareplan.Res.addIcon.png" ) );

				this.calendarViewIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "Bareplan.Res.calendarViewIcon.png" ) );

				this.exportIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "Bareplan.Res.exportIcon.png" ) );

				this.listIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "Bareplan.Res.listIcon.png" ) );

				this.newIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "Bareplan.Res.newIcon.png" ) );

				this.openIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "Bareplan.Res.openIcon.png" ) );

				this.propertiesIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "Bareplan.Res.propertiesIcon.png" ) );

				this.saveIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "Bareplan.Res.saveIcon.png" ) );

				this.settingsIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "Bareplan.Res.settingsIcon.png" ) );
			} catch(Exception e)
			{
				Debug.WriteLine( "ERROR loading icons: " + e.Message);
			}

			return;
		}

		private void BuildToolbar()
		{
			this.tbBar = new ToolBar();

			// Create image list
			var imgList = new ImageList{ ImageSize = new Size( 24, 24 ) };
			imgList.Images.AddRange( new Image[]{
				this.newIcon, this.openIcon,
				this.saveIcon,
				this.exportIcon, this.addRowIcon,
				this.propertiesIcon, this.settingsIcon,
			});

			// Buttons
			this.tbbNew = new ToolBarButton 		{ ImageIndex = 0 };
			this.tbbOpen = new ToolBarButton 		{ ImageIndex = 1 };
			this.tbbSave = new ToolBarButton 		{ ImageIndex = 2 };
			this.tbbExport = new ToolBarButton 		{ ImageIndex = 3 };
			this.tbbAddRow = new ToolBarButton 		{ ImageIndex = 4 };
			this.tbbProperties = new ToolBarButton 	{ ImageIndex = 5 };
			this.tbbSettings = new ToolBarButton 	{ ImageIndex = 6 };

			// Triggers
			this.tbBar.ButtonClick += (object sender, ToolBarButtonClickEventArgs e)
				=> this.OnToolbarButton( e.Button );

			// Polishing
			this.tbBar.ShowToolTips = true;
			this.tbBar.ImageList = imgList;
			this.tbBar.Dock = DockStyle.Top;
			this.tbBar.BorderStyle = BorderStyle.None;
			this.tbBar.Appearance = ToolBarAppearance.Flat;
			this.tbBar.Buttons.AddRange( new ToolBarButton[] {
				this.tbbNew, this.tbbOpen, this.tbbSave,
				this.tbbExport, this.tbbAddRow,
				this.tbbProperties, this.tbbSettings
			});
		}
		
		private void Build()
		{
			this.defaultFont = new Font( 
			                            System.Drawing.SystemFonts.DefaultFont.FontFamily,
			                            12,
			                            FontStyle.Regular );

			// Start
			this.StartPosition = FormStartPosition.CenterScreen;
			this.SuspendLayout();

			// Status bar
			this.stbStatus = new StatusBar();

			this.BuildIcons();
			this.BuildToolbar();
			this.BuildPlanning();
			this.BuildMainMenu();
			this.BuildAboutPanel();
			this.BuildPropertiesContainer();
			this.BuildSettingsPanel();

			// Add all to the UI
			this.Controls.Add( this.pnlPlanning );
			this.Controls.Add( this.tbBar );
			this.Controls.Add( this.pnlSettings );
			this.Controls.Add( this.pnlAbout );
			this.Controls.Add( this.stbStatus );

			// Apply configuration
			this.Width = this.CfgWidth;
			this.Height = this.CfgHeight;
			this.ChangeUILanguage( Locale.CurrentLocale );
			
			// Polish UI
			this.Text = AppInfo.Name;
			this.Icon = Icon.FromHandle( this.bmpAppIcon.GetHicon() );
			this.Cursor = Cursors.Arrow;
			this.MinimumSize = new Size( 620, 460 );
			this.Resize += ( obj, args) => this.ResizeWindow();
			this.Closed += ( obj, e) => this.OnQuit();

			// End
			this.pnlPlanning.ResumeLayout( false );
			this.ResumeLayout( true );
			this.ResizeWindow();
			return;			
		}

		/// <summary>
		/// Resizes the window, also the planning.
		/// The width of the columns of the planning
		/// is calculated by using percentages.
		/// </summary>
		private void ResizeWindow()
		{
			// Get the new measures
			int width = this.pnlPlanning.ClientRectangle.Width;
			
			// Resize the table of events
			this.grdPlanning.Width = width;

			this.grdPlanning.Columns[ (int) ColsIndex.Num ].Width =
										(int) Math.Floor( width *.07 );	// Num
			this.grdPlanning.Columns[ (int) ColsIndex.DoW ].Width =
										(int) Math.Floor( width *.07 );	// DoW
			this.grdPlanning.Columns[ (int) ColsIndex.Date ].Width =
										(int) Math.Floor( width *.16 ); // Date
			this.grdPlanning.Columns[ (int) ColsIndex.Kind ].Width =
										(int) Math.Floor( width *.16 ); // Kind
			this.grdPlanning.Columns[ (int) ColsIndex.Contents ].Width =
				    					(int) Math.Floor( width *.54 ); // Contents
		}

		private Bitmap bmpAppIcon;
		private Bitmap addRowIcon;
		private Bitmap calendarViewIcon;
		private Bitmap exportIcon;
		private Bitmap listIcon;
		private Bitmap newIcon;
		private Bitmap openIcon;
		private Bitmap propertiesIcon;
		private Bitmap saveIcon;
		private Bitmap settingsIcon;

		private ToolBarButton tbbAddRow;
		private ToolBarButton tbbExport;
		private ToolBarButton tbbNew;
		private ToolBarButton tbbOpen;
		private ToolBarButton tbbProperties;
		private ToolBarButton tbbSave;
		private ToolBarButton tbbSettings;

		private DataGridView grdPlanning;
		private Panel pnlConfigContainer;
		private TableLayoutPanel pnlSettings;
		private StatusBar stbStatus;
		private Panel pnlAbout;
		private Panel pnlPlanning;
		private Label lblLocales;
		private ComboBox cbLocales;
		private MonthCalendar calendar;
		private Label lblAbout;
		private Label lblInitialDate;
		private Label lblSteps;
		private TextBox edSteps;
		private DateTimePicker edInitialDate;
		private Font defaultFont;
		private Font planningFont;
		private MenuItem opInsertTask;
		private MenuItem opInsertDate;
		private MenuItem opInsert;
		private MenuItem opRemove;
		private MenuItem opRemoveTask;
		private MenuItem opRemoveDate;
		private MenuItem opIncFont;
		private MenuItem opDecFont;
		private MenuItem opViewSessions;
		private MenuItem opViewCalendar;
		private ToolBar tbBar;
		private TabControl tabbed;
		private TextBox txtDesc;
		
		private MainMenu mMain;		
		private MenuItem mFile;
		private MenuItem mEdit;
		private MenuItem mView;
		private MenuItem mHelp;
		private MenuItem opExport;
		private MenuItem opProperties;
		private MenuItem opNew;
		private MenuItem opOpen;
		private MenuItem opSave;
		private MenuItem opSaveAs;
		private MenuItem opClose;
		private MenuItem opQuit;
		private MenuItem opAdd;
		private MenuItem opAbout;
		private MenuItem opSettings;
		
		private ContextMenu cntxtMenu;
	}
}

