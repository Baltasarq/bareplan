// Bareplan (c) 2015-17 MIT License <baltasarq@gmail.com>

namespace Bareplan.Core {
	using System.Text;

	/// <summary>
	/// A csv exporter generates a tsv (tab separated values) file.
	/// </summary>
	public class CsvExporter: DocumentExporter {
		public const string DefaultExt = "csv";
		
		/// <summary>
		/// Initializes a new <see cref="T:Bareplan.Core.CsvExporter"/>.
		/// </summary>
		/// <param name="info">A <see cref="T:ExportInfo"/>.</param>
		public CsvExporter(ExportInfo info)
			: base( info )
		{
		}
		
		protected override void WriteHeader(StringBuilder txt)
		{
			for(int i = 0; i < this.Info.ColumnNumber; ++i) {
				if ( !this.Info.VisibleColumn[ i ] ) {
					continue;
				}
				
				txt.Append( '"' );
				txt.Append( this.Info.GetColumnHeaders[ i ]() );
				txt.Append( '"' );
				
				if ( i < ( this.Info.ColumnNumber - 1 ) ) {
					txt.Append( "\t");
				}
			}
			
			txt.AppendLine();
			return;
		}
		
		protected override void WriteRow(StringBuilder txt, RowInfo rowInfo)
		{
			for(int i = 0; i < this.Info.ColumnNumber; ++i) {
				if ( !this.Info.VisibleColumn[ i ] ) {
					continue;
				}
				
				ExportInfo.Column column = (ExportInfo.Column) i;
				string columnValue = this.GetColumn( column, rowInfo );
				
				if ( column == ExportInfo.Column.Task
				  || column == ExportInfo.Column.Day )
				{
					txt.Append( '"' );
					txt.Append( columnValue );
					txt.Append( '"' );
				} else {
					txt.Append( columnValue );
				}
				
				if ( i < ( this.Info.ColumnNumber - 1 ) ) {
					txt.Append( "\t" );
				}
			}

			return;
		}
	}
}
