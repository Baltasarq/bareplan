// Bareplan (c) 2015-17 MIT License <baltasarq@gmail.com>

namespace Bareplan.Core {
	using System.Text;

	/// <summary>
	/// A text exporter generates a text file.
	/// </summary>
	public class TextExporter: DocumentExporter {
		public const string DefaultExt = "txt";
		
		public TextExporter(ExportInfo info)
			: base( info )
		{
		}
		
		protected override void WriteHeader(StringBuilder txt)
		{
			for(int i = 0; i < this.Info.ColumnNumber; ++i) {
				if ( !this.Info.VisibleColumn[ i ] ) {
					continue;
				}
				
				txt.Append( this.Info.GetColumnHeaders[ i ]() );
				
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
				
				txt.Append( this.GetColumn( (ExportInfo.Column) i, rowInfo ) );
				
				if ( i < ( this.Info.ColumnNumber - 1 ) ) {
					txt.Append( "\t" );
				}
			}
			
			return;
		}
	}
}
