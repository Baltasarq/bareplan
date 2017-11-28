// Bareplan (c) 2015-17 MIT License <baltasarq@gmail.com>

namespace Bareplan.Core {
	using System;
	using System.Diagnostics;
	using System.Collections.ObjectModel;
	
	/// <summary>The export options center.</summary>
	public class ExportInfo {
		public const string DefaultExportFileName = "out";
		/// <summary>The file type to create.</summary>
		public enum FileType { Csv, Html, Text };
		/// <summary>File extensions for each file type.</summary>
		public static readonly ReadOnlyCollection<string> FileExt = new ReadOnlyCollection<string>(
			new []{ "csv", ".html", "txt" } );
		/// <summary>The columns to dump.</summary>
		public enum Column { Session, Week, Day, Date, Task };
		/// <summary>Obtains the localized column headers.</summary>
		public readonly Func<string>[] GetColumnHeaders = {
			() => L10n.Get( L10n.Id.HdSession ),
			() => L10n.Get( L10n.Id.HdWeek ),
			() => L10n.Get( L10n.Id.HdDay ),
			() => L10n.Get( L10n.Id.HdDate ),
			() => L10n.Get( L10n.Id.HdTask ),
		};
		
		/// <summary>
		/// Initializes a new <see cref="T:Bareplan.Core.ExportOptions"/>.
		/// </summary>
		/// <param name="doc">The <see cref="Document"/>.</param>
		public ExportInfo(Document doc)
		{			
			this.Document = doc;
			this.ColumnNumber = Enum.GetNames( typeof( Column ) ).Length;
			this.VisibleColumn = new bool[ this.ColumnNumber ];
			
			Debug.Assert( GetColumnHeaders.Length == this.ColumnNumber );			
			
			// All columns are visible by default
			for(int i = 0; i < this.VisibleColumn.Length; ++i) {
				this.VisibleColumn[ i ] = true;
			}
			
			// Export name
			this.FileName = this.Document.FileName;
			if ( string.IsNullOrWhiteSpace( this.FileName ) ) {
				this.FileName = DefaultExportFileName;
			}
		}

		///<summary>Converts the info into a single string.</summary>
		public override string ToString()
		{
			string[] visibleColumns =
				Array.ConvertAll( this.VisibleColumn, Convert.ToString );
			string visibility = string.Join( ", ", visibleColumns );

			return string.Format( "[ExportOptions: Type={0}, "
									+ "VisibleColumn={1}, ExportFileName={2}, "
									+ "ColumnNumber={3}]",
									this.Type,
									visibility,
									this.FileName,
									this.ColumnNumber);
		}
		
		/// <summary>
		/// Gets or sets the type of the future type.
		/// </summary>
		/// <value>The <see cref="FileType"/>.</value>
		public FileType Type {
			get; set;
		}
		
		/// <summary>Gets the document to export.</summary>
		/// <value>The <see cref="Document"/>.</value>
		public Document Document {
			get; private set;
		}
		
		public bool[] VisibleColumn {
			get; private set;
		}
		
		/// <summary>
		/// Gets or sets the name of the file to export.
		/// </summary>
		/// <value>The name of the file.</value>
		public string FileName {
			get; set;
		}
		
		/// <summary>
		/// Gets the column number.
		/// </summary>
		/// <value>The column number.</value>
		public int ColumnNumber {
			get; private set;
		}
	}
}
