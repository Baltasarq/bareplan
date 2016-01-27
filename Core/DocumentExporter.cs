using System;
using System.IO;

namespace Bareplan.Core {
	/// <summary>
	/// Base class for all document exporters.
	/// </summary>
	public abstract class DocumentExporter {
		/// <summary>
		/// The base <see cref="Bareplan.Core.DocumentExporter"/> class for all exporters.
		/// </summary>
		/// <param name='doc'>
		/// The <see cref="Bareplan.Core.Document"/> to export.
		/// </param>
		public DocumentExporter(Document doc, string path)
		{
			this.Document = doc;
			this.Path = path;
			this.Contents = "";
		}

		/// <summary>
		/// Actually exports the document to an HTML string.
		/// </summary>
		public abstract void Export();

		/// <summary>
		/// Saves the Contents. Invokes Export() if Contents is empty.
		/// </summary>
		public void Save()
		{
			if ( this.Contents.Trim().Length == 0 ) {
				this.Export();
			}

			using (StreamWriter outfile = new StreamWriter( this.Path ) )
	        {
	            outfile.WriteLine( this.Contents );
	        }
		}

		/// <summary>
		/// The contents to be stored in the file.
		/// This is normally generated by Export().
		/// </summary>
		/// <value>
		/// The contents, as a string.
		/// </value>
		public string Contents {
			get; set;
		}

		/// <summary>
		/// Gets or sets the document.
		/// </summary>
		/// <value>
		/// The document, as a <see cref="Bareplan.Core.Document"/> object.
		/// </value>
		public Document Document {
			get; set;
		}

		/// <summary>
		/// Gets or sets the path to the file to export.
		/// </summary>
		/// <value>
		/// The path, as a string object.
		/// </value>
		public string Path {
			get; set;
		}
	}
}
