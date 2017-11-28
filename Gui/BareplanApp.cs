// Bareplan (c) 2015-17 MIT License <baltasarq@gmail.com>

namespace Bareplan.Gui {
	using System;
	using System.IO;
	using System.Diagnostics;
	using System.Windows.Forms;
	
	using Bareplan.Core;

	public class BareplanApp {
		public static void CreateLog()
		{
			var now = DateTime.Now;
			string cfgDir = Environment.GetFolderPath(
										Environment.SpecialFolder.UserProfile );
			string logFileName = Path.Combine( cfgDir, "." + AppInfo.Name + ".log" );
			Trace.Listeners.Add( new TextWriterTraceListener( logFileName ) );
			Trace.WriteLine( AppInfo.AppHeader );
			Trace.WriteLine( now.ToShortDateString() + " " + now.ToShortTimeString() );
			Trace.WriteLine( logFileName + "\n==\n" );
		}
		
		[STAThread]
		public static void Main()
		{
			try {
				CreateLog();
				var mainWindow = new MainWindow();
				Application.Run( mainWindow );
			}
			catch(Exception exc)
			{
				MessageBox.Show(
					null,
					"Critical ERROR: " + exc.Message,
					AppInfo.Name,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}
			finally {
				Trace.Close();
			}
		}
	}
}
