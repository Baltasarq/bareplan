// Bareplan (c) 2015-17 MIT License <baltasarq@gmail.com>

namespace Bareplan.Core {
	using System;
	using System.Text;
	using System.Globalization;
	using System.Collections.Generic;

	/// <summary>
	/// Exports a class to HTML format.
	/// </summary>
	public class HtmlExporter: DocumentExporter {
		public const string DefaultExt = "html";

		public HtmlExporter(Document doc, string path): base( doc, path )
		{
		}
		
		private void WriteHtmlHeader(StringBuilder htmlDoc)
		{
			htmlDoc.AppendLine( "<html><header>" );
			htmlDoc.AppendLine( "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">" );
			htmlDoc.AppendLine( "<title>"
			               + System.IO.Path.GetFileNameWithoutExtension( this.Path )
			               + "</title>"
			               );

			htmlDoc.AppendLine( "</header>\n<body>\n" );
		}
		
		private void WriteHtmlFooter(StringBuilder htmlDoc)
		{
			htmlDoc.AppendLine( "\n<!-- " + AppInfo.AppHeader + " -->\n</body>\n</html>\n" );
		}
		
		private void WriteTableHeaders(StringBuilder toret)
		{
			toret.AppendLine( "<table border='0'>\n" );
			toret.AppendLine( "<tr>" );
			toret.AppendLine( "<td style=\"color: white; background-color: black;\"><b>Sesi&oacute;n</b></td>" );
			toret.AppendLine( "<td style=\"color: white; background-color: black;\"><b>Semana</b></td>" );
			toret.AppendLine( "<td style=\"color: white; background-color: black;\"><b>D&iacute;a</b></td>" );
			toret.AppendLine( "<td style=\"color: white; background-color: black;\"><b>Fecha</b></td>" );
			toret.AppendLine( "<td style=\"color: white; background-color: black;\"><b>Tarea</b></td>" );
			toret.AppendLine( "</tr>\n" );
		}
		
		private static int GetWeekNumberFor(DateTime date)
		{
			DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
			Calendar cal = dfi.Calendar;
	
			return cal.GetWeekOfYear( date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek );
		}

		public override void Export()
		{
			DateTimeFormatInfo dtfo = CultureInfo.CurrentCulture.DateTimeFormat;
			StringBuilder toret = new StringBuilder();

			this.WriteHtmlHeader( toret );
			this.WriteTableHeaders( toret );

			// Write each row
			KeyValuePair<DateTime, string> pair = this.Document.GotoFirst();
			int weekCount = 1;
			int session = 1;
			bool showWeek = true;
			int weekNumber = GetWeekNumberFor( pair.Key );
			while( !this.Document.IsEnd ) {
				toret.AppendLine( "<tr>" );

				// Session
				toret.AppendLine( "<td style=\"color: black; background-color: rgb(204,204,204);\"><b>"
				                 + Convert.ToString( session )
				                 + "</b></td>"
				);
				
				// Week count
				if ( showWeek ) {
					toret.AppendLine( "<td style=\"color: black; background-color: rgb(204,204,204); text-align: center\">"
				                 + "<b>" + Convert.ToString( weekCount )
				                 + "</b></td>" );
				                 
					showWeek = false;
				} else {
					toret.AppendLine( "<td style=\"background-color: white;\"></td>" );
				}

				// Day of week
				toret.Append( "<td" );
				if ( ( session % 2 ) == 0 ) {
					toret.Append( " style=\"color: black; background-color: rgb(204,204,204);\"" );
				}

				toret.AppendLine( ">" + dtfo.GetAbbreviatedDayName( pair.Key.DayOfWeek ) + "</td>" );

				// Date
				toret.Append( "<td" );
				if ( ( session % 2 ) == 0 ) {
					toret.Append( " style=\"color: black; background-color: rgb(204,204,204);\"" );
				}

				toret.AppendLine( ">" + pair.Key.ToShortDateString() + "</td>" );

				// Task
				toret.Append( "<td" );
				if ( ( session % 2 ) == 0 ) {
					toret.Append( " style=\"color: black; background-color: rgb(204,204,204);\"" );
				}

				toret.AppendLine( ">" + pair.Value + "</td>" );

				// Next task/date pair
				toret.AppendLine();
				pair = this.Document.Next();
				++session;
				
				int newWeekNumber = GetWeekNumberFor( pair.Key );
				if ( newWeekNumber != weekNumber ) {
					++weekCount;
					weekNumber = newWeekNumber;
					showWeek = true;
				}
			}

			// End
			toret.AppendLine( "\n</table>\n" );
			this.WriteHtmlFooter( toret );
			this.Contents = toret.ToString();
		}
	}
}

