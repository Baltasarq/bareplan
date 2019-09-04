// Bareplan (c) 2015-17 MIT License <baltasarq@gmail.com>

namespace Bareplan.Core {
	using System.Web;
	using System.Text;

	/// <summary>
	/// Exports the document to HTML format.
	/// </summary>
	public class HtmlExporter: DocumentExporter {
		public const string DefaultExt = "html";

		/// <summary>
		/// Initializes a new <see cref="T:Bareplan.Core.HtmlExporter"/>.
		/// </summary>
		/// <param name="info">An <see cref="T: ExportInfo"/>.</param>
		public HtmlExporter(ExportInfo info)
			: base( info )
		{
		}
		
		protected override void WriteHeader(StringBuilder txt)
		{
			txt.AppendLine( "<html><header>" );
			txt.AppendLine( "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">" );
			txt.AppendLine( "<title>"
			               + System.IO.Path.GetFileNameWithoutExtension( this.Info.FileName )
			               + "</title>"
			               );

			txt.AppendLine( "</header>\n<body>\n" );
		}
		
		protected override void WriteFooter(StringBuilder txt)
		{
			txt.AppendLine( "\n<!-- " + AppInfo.AppHeader + " -->" );
			txt.AppendLine( "</body>\n</html>" );
		}
		
		protected override void WriteTableHeader(StringBuilder txt)
		{
			txt.AppendLine( "<table border='0'>\n" );
			txt.AppendLine( "<tr style=\"color: white; background-color: black;\">" );
			
			for(int i = 0; i < this.Info.ColumnNumber; ++i ) {
				if ( !this.Info.VisibleColumn[ i ] ) {
					continue;
				}
				
				txt.AppendLine( "<td><b>"
									+ HttpUtility.HtmlEncode( this.Info.GetColumnHeaders[ i ]() )
									+ "</b></td>" );
			}
			
			txt.AppendLine( "</tr>\n" );
		}
		
		protected override void WriteTableFooter(StringBuilder txt)
		{
			txt.AppendLine( "\n</table>");
		}
		
		protected override void WriteRow(StringBuilder txt, RowInfo rowInfo)
		{
			// Set row style
			if ( rowInfo.Session % 2 != 0 ) {
				txt.AppendLine( "<tr style=\"color: black; background-color: rgb(204,204,204);\">" );
			} else {
				txt.AppendLine( "<tr>" );
			}
			
			// Row columns
			for(int i = 0;  i < this.Info.ColumnNumber; ++i ) {				
				if ( !this.Info.VisibleColumn[ i ] ) {
					continue;
				}
				
				var column = (ExportInfo.Column) i;				
				string columnValue = HttpUtility.HtmlEncode( this.GetColumn( column, rowInfo ) );
				string style = "";
				
				if ( !rowInfo.WeekNumberChanged
				  && column == ExportInfo.Column.Week )
				{
					txt.AppendLine( "<td style=\"background-color: white;\"></td>" );
					continue;
				}
				
				if ( column == ExportInfo.Column.Session
				  || column == ExportInfo.Column.Day
				  || column == ExportInfo.Column.Week )
				{
					columnValue = "<b>" + columnValue + "</b>";
					style = "text-align: center";
				}
				
				txt.Append( "<td"
					+ ( !string.IsNullOrEmpty( style ) ? " style=\"" + style + '"' : "" )
					+ ">");
				txt.Append( columnValue );
				txt.AppendLine( "</td>" );
			}
			
			txt.AppendLine( "</tr>\n");
		}		
	}
}
