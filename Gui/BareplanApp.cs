using System;
using System.Windows.Forms;

using Bareplan.Gui;
using Bareplan.Core;

namespace Bareplan.Gui
{
	public class BareplanApp
	{
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

