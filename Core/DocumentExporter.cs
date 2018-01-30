// Bareplan (c) 2015-17 MIT License <baltasarq@gmail.com>

namespace Bareplan.Core {
	using System.IO;
	using System.Text;
	using System.Diagnostics;
	using System.Globalization;		
	
	using Pair = System.Collections.Generic.KeyValuePair<System.DateTime, string>;
	
	/// <summary>
	/// Base class for all document exporters.
	/// </summary>
	public abstract class DocumentExporter {
		public class RowInfo {
			/// <summary>
			/// Gets or sets the session.
			/// </summary>
			/// <value>The session.</value>
			public int Session {
				get; set;
			}
			
			/// <summary>
			/// Gets or sets the week number.
			/// </summary>
			/// <value>The week number.</value>
			public int WeekNumber {
				get; set;
			}
			
			/// <summary>
			/// Gets or sets a value indicating whether
			/// this <see cref="T:Bareplan.Core.DocumentExporter.RowInfo"/>'s week number changed.
			/// </summary>
			/// <value><c>true</c> if week number changed; otherwise, <c>false</c>.</value>
			public bool WeekNumberChanged {
				get; set;
			}
			
			/// <summary>
			/// Gets or sets the pair.
			/// </summary>
			/// <value>The pair of <see cref="T:System.DateTime"/> and task(string).</value>
			public Pair Pair {
				get; set;
			}
		}
	
	
		/// <summary>
		/// The base <see cref="Bareplan.Core.DocumentExporter"/> class for all exporters.
		/// </summary>
		/// <param name='info'>
		/// The <see cref="Bareplan.Core.ExportInfo"/> with the info to do the export.
		/// </param>
		protected DocumentExporter(ExportInfo info)
		{
			this.Info = info;
			this.Contents = "";
		}
		
		/// <summary>
		/// Writes the header.
		/// </summary>
		/// <param name="txt">A <see cref="T:System.StringBuilder"/>Text.</param>
		protected virtual void WriteHeader(StringBuilder txt)
		{
		}
		
		/// <summary>
		/// Writes the header for the table of date/task's rows.
		/// </summary>
		/// <param name="txt">A <see cref="T:System.StringBuilder"/>Text.</param>
		protected virtual void WriteTableHeader(StringBuilder txt)
		{
		}
		
		/// <summary>
		/// Writes the footer for the table of date/task's rows.
		/// </summary>
		/// <param name="txt">A <see cref="T:System.StringBuilder"/>Text.</param>
		protected virtual void WriteTableFooter(StringBuilder txt)
		{
		}
		
		/// <summary>
		/// Writes the footer.
		/// </summary>
		/// <param name="txt">A <see cref="T:System.StringBuilder"/>Text.</param>
		protected virtual void WriteFooter(StringBuilder txt)
		{
		}
		
		/// <summary>
		/// Writes a given date/task row.
		/// </summary>
		/// <param name="txt">A <see cref="T:System.StringBuilder"/>Text.</param>
		/// <param name="rowInfo">A <see cref="T:RowInfo"/> containing all row's info.</param>
		protected virtual void WriteRow(StringBuilder txt, RowInfo rowInfo)
		{
		}

		/// <summary>
		/// Actually exports the document to an HTML string.
		/// </summary>
		public void Export()
		{
			Document doc = this.Info.Document;
			DateTimeFormatInfo dtfo = CultureInfo.CurrentCulture.DateTimeFormat;
			StringBuilder toret = new StringBuilder();
			
			Trace.WriteLine( "Exporter.Export: Begin" );
			Trace.Indent();

			WriteHeader( toret );
			WriteTableHeader( toret );
			
			// Write each row
			Pair pair = doc.GotoFirst();
			int weekCount = 1;
			int session = 1;
			bool showWeek = true;
			int weekNumber = GetWeekNumberFor( pair.Key );
			while( !doc.IsEnd() ) {
				var rowInfo = new RowInfo {
					Session = session,
					WeekNumber = weekCount,
					Pair = pair,
					WeekNumberChanged = showWeek
				};

				this.WriteRow( toret, rowInfo );
				showWeek = false;

				// Next task/date pair
				toret.AppendLine();
				pair = doc.Next();
				++session;
				
				int newWeekNumber = GetWeekNumberFor( pair.Key );
				if ( newWeekNumber != weekNumber ) {
					++weekCount;
					weekNumber = newWeekNumber;
					showWeek = true;
				}
			}

			// End
			WriteTableFooter( toret );
			WriteFooter( toret );
			this.Contents = toret.ToString();
			
			Trace.Unindent();			
			Trace.WriteLine( "Exporter.Export: End" );
		}

		/// <summary>
		/// Saves the Contents. Invokes Export() if Contents is empty.
		/// </summary>
		public void Save()
		{
			string fileName = this.Info.FileName;
			
			Trace.WriteLine( "Exporter.Save: Begin" );
			Trace.Indent();
			
			if ( string.IsNullOrWhiteSpace( this.Contents ) ) {
				this.Export();
			}

			Trace.WriteLine( "Writing to: " + fileName );
			Trace.WriteLine( "Writing " + this.Contents.Length + " chrs." );

			using (StreamWriter outfile = new StreamWriter( fileName ) )
	        {
	            outfile.WriteLine( this.Contents );
	        }
	        
	        Trace.Unindent();			
			Trace.WriteLine( "Exporter.Save: End" );
		}
		
		/// <summary>
		/// Creates the specified exporter, given the export info.
		/// </summary>
		/// <returns>An appropriate <see cref="DocumentExporter"/>.</returns>
		/// <param name="info">The <see cref="ExportInfo"/>.</param>
		public static DocumentExporter Create(ExportInfo info)
		{
			DocumentExporter toret = null;
			
			switch( info.Type ) {
				case ExportInfo.FileType.Csv:
					toret = new CsvExporter( info );
					break;
				case ExportInfo.FileType.Html:
					toret = new HtmlExporter( info );
					break;
				case ExportInfo.FileType.Text:
					toret = new TextExporter( info );
					break;
				case ExportInfo.FileType.GCal:
					toret = new GCalExporter( info );
					break;
			}
			
			Debug.Assert( toret != null, "Exporter.Create: No exporter found for: " + info.Type );
			return toret;
		}
		
		/// <summary>
		/// Gets the week number for the given date.
		/// </summary>
		/// <returns>The correpsonding week number.</returns>
		/// <param name="date">A <see cref="T:System.DateTime"/>.</param>
		protected static int GetWeekNumberFor(System.DateTime date)
		{
			DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
			Calendar cal = dfi.Calendar;
	
			return cal.GetWeekOfYear( date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek );
		}
		
		/// <summary>
		/// Gets the column info, converted into a string.
		/// </summary>
		/// <returns>The column, as a string.</returns>
		/// <param name="column">A <see cref="T:ExportInfo.Column"/>.</param>
		/// <param name="rowInfo">A <see cref="T:DocumentExporter.RowInfo"/>.</param>
		protected string GetColumn(ExportInfo.Column column, RowInfo rowInfo)
		{
			string toret = null;
			
			switch ( column ) {
				case ExportInfo.Column.Session:
					toret = rowInfo.Session.ToString();
					break;
				case ExportInfo.Column.Week:
					toret = rowInfo.WeekNumber.ToString();
					break;
				case ExportInfo.Column.Day:
					DateTimeFormatInfo dtfo = CultureInfo.CurrentCulture.DateTimeFormat;
					toret = dtfo.GetAbbreviatedDayName( rowInfo.Pair.Key.DayOfWeek );
					break;
				case ExportInfo.Column.Date:
					toret = rowInfo.Pair.Key.ToShortDateString();
					break;
				case ExportInfo.Column.Task:
					toret = rowInfo.Pair.Value;
					break;
			}
			
			Debug.Assert( toret != null, "no column's value for: " + column );
			return toret;
		}

		/// <summary>
		/// The contents to be stored in the file.
		/// This is normally generated by <see cref="M:DocumentExporter.Export"/>.
		/// </summary>
		/// <value>
		/// The contents, as a string.
		/// </value>
		public string Contents {
			get; protected set;
		}
		
		/// <summary>
		/// Gets or sets the export info.
		/// </summary>
		/// <value>The <see cref="Info"/>.</value>
		public ExportInfo Info {
			get; private set;
		}
	}
}
