// Bareplan (c) 2015-17 MIT License <baltasarq@gmail.com>

namespace Bareplan.Gui {
	using System;
	using System.Windows.Forms;
	
	using Bareplan.Core;

	public class BareplanApp {
		[STAThread]
		public static void Main()
		{
			try {
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
		}
	}
}
