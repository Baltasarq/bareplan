using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;

using Bareplan.Core;

namespace Bareplan.Gui {
	public partial class MainWindow: Form {
		public const int FontStep = 2;
		public enum ColsIndex { Num, DoW, Date, Task };
		public const string EtqWidth  = "width";
		public const string EtqHeight = "height";
		public const string EtqLocale = "locale";
		public const string CfgFileName = ".bareplan.cfg";

		public MainWindow()
		{
			this.Shown += (sender, e) => this.OnShow();
			this.Build();
			this.DeactivateGui();
			this.OnIncFont();
		}

		private void BuildPlanning()
		{
			// Prepare the list of events
			this.grdPlanning = new DataGridView();
			this.grdPlanning.AllowUserToResizeRows = false;
			this.grdPlanning.RowHeadersVisible = false;
			this.grdPlanning.AutoGenerateColumns = false;
			this.grdPlanning.MultiSelect = false;
			this.grdPlanning.AllowUserToAddRows = false;
			this.planningFont = new Font( this.grdPlanning.Font, FontStyle.Regular );
			var textCellTemplate0 = new DataGridViewTextBoxCell();
			var textCellTemplate1 = new DataGridViewTextBoxCell();
			var textCellTemplate2 = new DataGridViewTextBoxCell();
			var textCellTemplate3 = new DataGridViewTextBoxCell();
			textCellTemplate0.Style.BackColor = this.grdPlanning.RowHeadersDefaultCellStyle.BackColor;
			textCellTemplate0.Style.Font = new Font( this.planningFont, FontStyle.Bold );
			textCellTemplate1.Style.BackColor = this.grdPlanning.RowHeadersDefaultCellStyle.BackColor;
			textCellTemplate1.Style.Font = new Font( this.planningFont, FontStyle.Bold );
			textCellTemplate2.Style.BackColor = Color.Wheat;
			textCellTemplate3.Style.BackColor = Color.White;
			textCellTemplate3.Style.ForeColor = Color.Navy;
			var column0 = new DataGridViewTextBoxColumn();
			var column1 = new DataGridViewTextBoxColumn();
			var column2 = new DataGridViewTextBoxColumn();
			var column3 = new DataGridViewTextBoxColumn();
			column0.SortMode = DataGridViewColumnSortMode.NotSortable;
			column1.SortMode = DataGridViewColumnSortMode.NotSortable;
			column2.SortMode = DataGridViewColumnSortMode.NotSortable;
			column3.SortMode = DataGridViewColumnSortMode.NotSortable;
			column0.CellTemplate = textCellTemplate0;
			column1.CellTemplate = textCellTemplate1;
			column2.CellTemplate = textCellTemplate2;
			column3.CellTemplate = textCellTemplate3;
			column0.HeaderText = "#";
			column0.Width = 25;
			column0.ReadOnly = true;
			column1.HeaderText = "{*}";
			column1.Width = 25;
			column1.ReadOnly = true;
			column2.HeaderText = "Fecha";
			column2.Width = 75;
			column3.HeaderText = "Tarea";
			column3.Width = 250;
			
			this.grdPlanning.Columns.AddRange( new DataGridViewColumn[] {
				column0,
				column1,
				column2,
				column3,
			} );

			this.grdPlanning.CellEndEdit += delegate(object obj, DataGridViewCellEventArgs args) {
				this.OnCellEdited( args.RowIndex, args.ColumnIndex );
			};

			this.grdPlanning.Dock = DockStyle.Fill;
			this.grdPlanning.TabIndex = 3;
			this.grdPlanning.AllowUserToOrderColumns = false;

			this.pnlPlanning = new Panel();
			this.pnlPlanning.SuspendLayout();
			this.pnlPlanning.Dock = DockStyle.Fill;
			this.pnlPlanningContainer = new Panel();
			this.pnlPlanningContainer.Dock = DockStyle.Fill;
			this.pnlPlanningContainer.Controls.Add( this.grdPlanning );
			this.pnlPlanning.Controls.Add( this.pnlPlanningContainer );
		}

		private void BuildPropertiesContainer()
		{
			// Sizes
			Graphics grf = this.CreateGraphics();
			SizeF fontSize = grf.MeasureString( "M", this.defaultFont );
			int charSize = (int) fontSize.Width + 5;

			// Panel
			this.pnlConfigContainer = new Panel();
			this.pnlConfigContainer.SuspendLayout();
			this.pnlConfigContainer.Dock = DockStyle.Bottom;
			this.pnlConfigContainer.BackColor = Color.FloralWhite;

			// Inner panel
			var pnlInner = new Panel();
			pnlInner.SuspendLayout();
			pnlInner.Dock = DockStyle.Fill;
			var somePadding = new Padding();
			somePadding.All = 10;
			pnlInner.Padding = somePadding;

			// Steps editing
			var pnlSteps = new Panel();
			pnlSteps.SuspendLayout();
			pnlSteps.Dock = DockStyle.Left;
			this.lblSteps = new Label();
			this.lblSteps.Text = "Pasos:";
			this.lblSteps.Dock = DockStyle.Left;
			this.edSteps = new TextBox();
			this.edSteps.Dock = DockStyle.Fill;
			this.edSteps.KeyDown += delegate(object obj, KeyEventArgs args) {
				if ( args.KeyCode == Keys.Enter ) {
					this.OnPropertiesPanelClosed();
				}
			};
			pnlSteps.Controls.Add( this.edSteps );
			pnlSteps.Controls.Add( this.lblSteps );
			pnlSteps.ResumeLayout( false );

			// Initial date
			var pnlDate = new Panel();
			pnlDate.SuspendLayout();
			pnlDate.Dock = DockStyle.Right;
			this.lblInitialDate = new Label();
			this.lblInitialDate.Text = "Fecha inicial:";
			this.lblInitialDate.Dock = DockStyle.Left;
			this.edInitialDate = new DateTimePicker();
			this.edInitialDate.Dock = DockStyle.Fill;
			this.edInitialDate.Format = DateTimePickerFormat.Custom;
			this.edInitialDate.CustomFormat = Core.Locale.CurrentLocale.DateTimeFormat.ShortDatePattern;
			pnlDate.Controls.Add( this.edInitialDate );
			pnlDate.Controls.Add( this.lblInitialDate );
			pnlDate.ResumeLayout( false );

			// Button for hiding the panel
			var btCloseConfigContainer = new Button();
			btCloseConfigContainer.Text = "X";
			btCloseConfigContainer.Dock = DockStyle.Right;
			btCloseConfigContainer.Font = new Font( this.defaultFont, FontStyle.Bold );
			btCloseConfigContainer.Width = charSize * 5;
			btCloseConfigContainer.FlatStyle = FlatStyle.Flat;
			btCloseConfigContainer.FlatAppearance.BorderSize = 0;
			btCloseConfigContainer.Click += delegate(object obj, EventArgs args) {
				this.OnPropertiesPanelClosed();
			};

			// Adding controls
			pnlInner.Controls.Add( pnlDate );
			pnlInner.Controls.Add( pnlSteps );
			pnlInner.ResumeLayout( true );
			this.pnlConfigContainer.Controls.Add( pnlInner );
			this.pnlConfigContainer.Controls.Add( btCloseConfigContainer );
			this.pnlConfigContainer.ResumeLayout( true );

			// Finishing
			this.pnlConfigContainer.MinimumSize = 
				new Size( this.Width, ( this.edSteps.Height * 2 ) + 5 );
			this.pnlConfigContainer.MaximumSize = 
				new Size( Int32.MaxValue, ( this.edSteps.Height * 2 ) + 5 );
			this.pnlConfigContainer.Hide();
			this.pnlPlanning.Controls.Add( this.pnlConfigContainer );
		}

		private void BuildMainMenu()
		{
			// Menu
			this.mMain = new MainMenu();
			this.mFile = new MenuItem( "&Archivo" );
			this.mHelp = new MenuItem( "&Ayuda" );
			this.mEdit = new MenuItem( "&Editar" );
			this.opQuit = new MenuItem( "&Salir" );
			this.opAbout = new MenuItem( "&Acerca de..." );
			this.mView = new MenuItem( "&Ver" );
			this.opExport = new MenuItem( "&Exportar" );
			this.opOpen = new MenuItem( "&Abrir" );
			this.opNew = new MenuItem( "&Nuevo" );
			this.opSave = new MenuItem( "&Guardar" );
			this.opSaveAs = new MenuItem( "G&uardar como..." );
			this.opClose = new MenuItem( "&Cerrar" );
			this.opAdd = new MenuItem( "&Agregar fila" );
			this.opProperties = new MenuItem( "&Propiedades" );
			this.opInsertTask = new MenuItem( "&Insertar tarea" );
			this.opRemove = new MenuItem( "Eliminar &fila" );
			this.opRemoveTask = new MenuItem( "Eliminar &tarea" );
			this.opRemoveDate = new MenuItem( "Eliminar f&echa" );
			this.opIncFont = new MenuItem( "&Incrementar fuente" );
			this.opDecFont = new MenuItem( "&Decrementar fuente" );
			this.opSettings = new MenuItem( "&Preferencias" );
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
			this.mView.MenuItems.Add( this.opDecFont );
			this.mView.MenuItems.Add( this.opIncFont );
			this.mEdit.MenuItems.Add( this.opAdd );
			this.mEdit.MenuItems.Add( this.opInsertTask );
			this.mEdit.MenuItems.Add( this.opRemove );
			this.mEdit.MenuItems.Add( this.opRemoveTask );
			this.mEdit.MenuItems.Add( this.opRemoveDate );
			this.mEdit.MenuItems.Add( this.opProperties );
			this.mEdit.MenuItems.Add( this.opSettings );
			this.mHelp.MenuItems.Add( this.opAbout );
			this.opQuit.Shortcut = Shortcut.CtrlQ;
			this.opNew.Shortcut = Shortcut.CtrlN;
			this.opOpen.Shortcut = Shortcut.CtrlO;
			this.opSave.Shortcut = Shortcut.CtrlS;
			this.opAdd.Shortcut = Shortcut.CtrlIns;
			this.opProperties.Shortcut = Shortcut.F2;
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
			this.opInsertTask.Click += ( obj, evt ) => this.OnInsertTask();
			this.opRemove.Click += ( obj, evt ) => this.OnRemove();
			this.opRemoveTask.Click += ( obj, evt ) => this.OnRemoveTask();
			this.opRemoveDate.Click += ( obj, evt ) => this.OnRemoveDate();
			this.opIncFont.Click += ( obj, evt ) => this.OnIncFont();
			this.opDecFont.Click += ( obj, evt ) => this.OnDecFont();
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
			this.pnlAbout = new Panel();
			this.pnlAbout.SuspendLayout();
			this.pnlAbout.Dock = DockStyle.Bottom;
			this.pnlAbout.BackColor = Color.LightYellow;
			this.lblAbout = new Label();
			this.lblAbout.Text = AppInfo.Name + " v" + AppInfo.Version + ", " + AppInfo.Author;
			this.lblAbout.Dock = DockStyle.Left;
			this.lblAbout.TextAlign = ContentAlignment.MiddleCenter;
			this.lblAbout.AutoSize = true;
			this.lblAbout.Font = new Font( this.defaultFont, FontStyle.Bold );
			var btCloseAboutPanel = new Button();
			btCloseAboutPanel.Text = "X";
			btCloseAboutPanel.Font = new Font( this.defaultFont, FontStyle.Bold );
			btCloseAboutPanel.Dock = DockStyle.Right;
			btCloseAboutPanel.Width = charSize * 5;
			btCloseAboutPanel.FlatStyle = FlatStyle.Flat;
			btCloseAboutPanel.FlatAppearance.BorderSize = 0;
			btCloseAboutPanel.Click += (o, evt) => this.pnlAbout.Hide();
			this.pnlAbout.Controls.Add( lblAbout );
			this.pnlAbout.Controls.Add( btCloseAboutPanel );
			this.pnlAbout.Hide();
			this.pnlAbout.MinimumSize = new Size( this.Width, this.lblAbout.Height +5 );
			this.pnlAbout.MaximumSize = new Size( Int32.MaxValue, this.lblAbout.Height +5 );

			this.pnlAbout.ResumeLayout( false );
		}

		private void BuildSettingsPanel()
		{
			this.pnlSettings = new TableLayoutPanel();
			this.pnlSettings.BackColor = Color.White;
			this.pnlSettings.Dock = DockStyle.Bottom;
			this.pnlSettings.ColumnCount = 1;
			this.pnlSettings.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
			this.pnlSettings.SuspendLayout();

			// Button
			var btClose = new Button();
			btClose.BackColor = Color.White;
			btClose.Text = "X";
			btClose.Anchor = AnchorStyles.Right;
			btClose.Font = new Font( btClose.Font, FontStyle.Bold );
			btClose.FlatStyle = FlatStyle.Flat;
			btClose.FlatAppearance.BorderSize = 0;
			btClose.Click += (sender, e) => this.ChangeSettings();
			this.pnlSettings.Controls.Add( btClose );

			// Locale
			var pnlLocales = new Panel();
			pnlLocales.Margin = new Padding( 5 );
			pnlLocales.Dock = DockStyle.Top;
			this.lblLocales = new Label();
			this.lblLocales.Text = StringsL18n.Get( StringsL18n.StringId.LblLanguage );
			this.lblLocales.Dock = DockStyle.Left;

			this.cbLocales = new ComboBox();
			this.cbLocales.Dock = DockStyle.Fill;
			this.cbLocales.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbLocales.Text = Locale.CurrentLocale.ToString();

			CultureInfo[] locales = CultureInfo.GetCultures( CultureTypes.SpecificCultures );
			Array.Sort( locales,
				((CultureInfo x, CultureInfo y) => x.ToString().CompareTo( y.ToString() ) )
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
						GetManifestResourceStream( "bareplan.Res.appIcon.png" ) );

				this.addRowIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "bareplan.Res.addRowIcon.png" ) );

				this.calendarViewIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "bareplan.Res.calendarViewIcon.png" ) );

				this.exportIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "bareplan.Res.exportIcon.png" ) );

				this.listIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "bareplan.Res.listIcon.png" ) );

				this.newIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "bareplan.Res.newIcon.png" ) );

				this.openIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "bareplan.Res.openIcon.png" ) );

				this.propertiesIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "bareplan.Res.propertiesIcon.png" ) );

				this.saveIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "bareplan.Res.saveIcon.png" ) );

				this.saveAsIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "bareplan.Res.saveAsIcon.png" ) );

				this.settingsIcon = new Bitmap(
					System.Reflection.Assembly.GetEntryAssembly( ).
						GetManifestResourceStream( "bareplan.Res.settingsIcon.png" ) );
			} catch(Exception)
			{
				// ignored: icons could not be loaded
			}

			return;
		}

		private void BuildToolbar()
		{
			this.tbBar = new ToolBar();

			// Create image list
			var imgList = new ImageList();
			imgList.ImageSize = new Size( 24, 24 );
			imgList.Images.AddRange( new Image[]{
				this.newIcon, this.openIcon,
				this.saveIcon, this.saveAsIcon,
				this.exportIcon, this.addRowIcon,
				this.propertiesIcon, this.settingsIcon,
			});

			// Buttons
			this.tbbNew = new ToolBarButton();
			this.tbbNew.ImageIndex = 0;
			this.tbbOpen = new ToolBarButton();
			this.tbbOpen.ImageIndex = 1;
			this.tbbSave = new ToolBarButton();
			this.tbbSave.ImageIndex = 2;
			this.tbbSaveAs = new ToolBarButton();
			this.tbbSaveAs.ImageIndex = 3;
			this.tbbExport = new ToolBarButton();
			this.tbbExport.ImageIndex = 4;
			this.tbbAddRow = new ToolBarButton();
			this.tbbAddRow.ImageIndex = 5;
			this.tbbProperties = new ToolBarButton();
			this.tbbProperties.ImageIndex = 6;
			this.tbbSettings = new ToolBarButton();
			this.tbbSettings.ImageIndex = 7;

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
				this.tbbSaveAs, this.tbbExport, this.tbbAddRow,
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
			this.pnlPlanningContainer.ResumeLayout( false );
			this.pnlPlanning.ResumeLayout( false );
			this.ResumeLayout( true );
			this.ResizeWindow();
			return;			
		}

		/// <summary>
		/// Resizes the window, also the planning.
		/// Date				 20%
		/// Task				 80%
		/// </summary>
		private void ResizeWindow()
		{
			// Get the new measures
			int width = this.pnlPlanningContainer.ClientRectangle.Width;
			
			// Resize the table of events
			this.grdPlanning.Width = width;

			this.grdPlanning.Columns[ (int) ColsIndex.Num ].Width =
										(int) Math.Floor( width *.07 );	// Num
			this.grdPlanning.Columns[ (int) ColsIndex.DoW ].Width =
										(int) Math.Floor( width *.07 );	// DoW
			this.grdPlanning.Columns[ (int) ColsIndex.Date ].Width =
										(int) Math.Floor( width *.16 ); // Date
			this.grdPlanning.Columns[ (int) ColsIndex.Task ].Width =
										(int) Math.Floor( width *.70 ); // Task
		}

		private Bitmap bmpAppIcon = null;
		private Bitmap addRowIcon = null;
		private Bitmap calendarViewIcon = null;
		private Bitmap exportIcon = null;
		private Bitmap listIcon = null;
		private Bitmap newIcon = null;
		private Bitmap openIcon = null;
		private Bitmap propertiesIcon = null;
		private Bitmap saveAsIcon = null;
		private Bitmap saveIcon = null;
		private Bitmap settingsIcon = null;

		private ToolBarButton tbbAddRow;
		private ToolBarButton tbbExport;
		private ToolBarButton tbbNew;
		private ToolBarButton tbbOpen;
		private ToolBarButton tbbProperties;
		private ToolBarButton tbbSaveAs;
		private ToolBarButton tbbSave;
		private ToolBarButton tbbSettings;

		private DataGridView grdPlanning = null;
		private Panel pnlPlanningContainer = null;
		private Panel pnlConfigContainer = null;
		private TableLayoutPanel pnlSettings = null;
		private MainMenu mMain = null;
		private StatusBar stbStatus = null;
		private Panel pnlAbout = null;
		private Panel pnlPlanning = null;
		private Label lblLocales = null;
		private ComboBox cbLocales = null;
		private MenuItem mFile = null;
		private MenuItem mEdit = null;
		private MenuItem mView = null;
		private MenuItem mHelp = null;
		private MenuItem opExport = null;
		private MenuItem opProperties = null;
		private MenuItem opNew = null;
		private MenuItem opOpen = null;
		private MenuItem opSave = null;
		private MenuItem opSaveAs = null;
		private MenuItem opClose = null;
		private MenuItem opQuit = null;
		private MenuItem opAdd = null;
		private MenuItem opAbout = null;
		private MenuItem opSettings = null;
		private Label lblAbout = null;
		private Label lblInitialDate = null;
		private Label lblSteps = null;
		private TextBox edSteps = null;
		private DateTimePicker edInitialDate = null;
		private Font defaultFont = null;
		private Font planningFont = null;
		private MenuItem opInsertTask = null;
		private MenuItem opRemove = null;
		private MenuItem opRemoveTask = null;
		private MenuItem opRemoveDate = null;
		private MenuItem opIncFont = null;
		private MenuItem opDecFont = null;
		private ToolBar tbBar;
	}
}

