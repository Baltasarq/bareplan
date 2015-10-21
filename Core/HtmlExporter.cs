using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace Bareplan.Core {
	/// <summary>
	/// Exports a class to HTML format.
	/// </summary>
	public class HtmlExporter: DocumentExporter {
		public const string DefaultExt = "html";

		public HtmlExporter(Document doc, string path): base( doc, path )
		{
		}

		public override void Export()
		{
			DateTimeFormatInfo dtfo = CultureInfo.CurrentCulture.DateTimeFormat;
			StringBuilder toret = new StringBuilder();

			// Write html header
			toret.AppendLine( "<html><header>" );
			toret.AppendLine( "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">" );
			toret.AppendLine( "<title>"
			               + System.IO.Path.GetFileNameWithoutExtension( this.Path )
			               + "</title>"
			               );

			toret.AppendLine( "</header>\n<body><table border='0'>\n" );

			// Write headers
			toret.AppendLine( "<tr>" );
			toret.AppendLine( "<td style=\"color: white; background-color: black;\"><b>Semana</b></td>" );
			toret.AppendLine( "<td style=\"color: white; background-color: black;\"><b>D&iacute;a</b></td>" );
			toret.AppendLine( "<td style=\"color: white; background-color: black;\"><b>Fecha</b></td>" );
			toret.AppendLine( "<td style=\"color: white; background-color: black;\"><b>Tarea</b></td>" );
			toret.AppendLine( "</tr>\n" );

			// Write each row
			KeyValuePair<DateTime, string> pair = this.Document.GotoFirst();
			int i = 1;
			while( !this.Document.IsEnd ) {
				toret.AppendLine( "<tr>" );

				// Row number
				toret.AppendLine( "<td style=\"color: black; background-color: rgb(204,204,204);\"><b>"
				                 + Convert.ToString( i )
				                 + "</b></td>"
				);

				// Day of week
				toret.Append( "<td" );
				if ( ( i % 2 ) == 0 ) {
					toret.Append( " style=\"color: black; background-color: rgb(204,204,204);\"" );
				}

				toret.AppendLine( ">" + dtfo.GetAbbreviatedDayName( pair.Key.DayOfWeek ) + "</td>" );

				// Date
				toret.Append( "<td" );
				if ( ( i % 2 ) == 0 ) {
					toret.Append( " style=\"color: black; background-color: rgb(204,204,204);\"" );
				}

				toret.AppendLine( ">" + pair.Key.ToShortDateString() + "</td>" );

				// Task
				toret.Append( "<td" );
				if ( ( i % 2 ) == 0 ) {
					toret.Append( " style=\"color: black; background-color: rgb(204,204,204);\"" );
				}

				toret.AppendLine( ">" + pair.Value + "</td>" );

				// Next task/date pair
				toret.AppendLine();
				pair = this.Document.Next();
				++i;
			}

			// End
			toret.AppendLine( "</table></body></html>\n" );
			this.Contents = toret.ToString();
		}
	}
}

