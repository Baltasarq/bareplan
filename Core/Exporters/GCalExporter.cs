// Bareplan (c) 2015-17 MIT License <baltasarq@gmail.com>

namespace Bareplan.Core {
	using System.Text;
	using System.IO;

	/// <summary>
	/// A GCal exporter generates a csv (comma separated values) file,
	/// ready to be imported by Google Calendar.
	/// </summary>
	public class GCalExporter: DocumentExporter {
		public const string DefaultExt = "csv";
		
		/// <summary>
		/// Initializes a new <see cref="T:Bareplan.Core.GCalExporter"/>.
		/// </summary>
		/// <param name="info">A <see cref="T:ExportInfo"/>.</param>
		public GCalExporter(ExportInfo info)
			: base( info )
		{
		}
		
		protected override void WriteHeader(StringBuilder txt)
		{
			txt.AppendLine( "Subject,Start Date,All Day Event,Description" );
		}
		
		protected override void WriteRow(StringBuilder txt, RowInfo rowInfo)
		{
			string date = this.GetColumn( ExportInfo.Column.Date, rowInfo );
			string subject = Path.GetFileNameWithoutExtension( this.Info.FileName );
			string description = this.GetColumn( ExportInfo.Column.Kind, rowInfo )
			                                     + ": "
			                                     + this.GetColumn( ExportInfo.Column.Contents, rowInfo );

			// Add double quotes where needed
			subject = '"' + subject + '"';
			description = '"' + description + '"';
			
			// Add data
			txt.Append( subject );
			txt.Append( ',' );
			txt.Append( date );
			txt.Append( ',' );
			txt.Append( "True" );
			txt.Append( ',' );
			txt.AppendLine( description );
		}
	}
}
