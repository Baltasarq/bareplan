using System;

namespace Bareplan.Core {
	public abstract class DocumentPersistence {

		/// <summary>
		/// Creates a new instance, prepared to load and save documents.
		/// </summary>
		/// <param name="doc">The document to save. Pass null if you are going to load a new one.</param>
		public DocumentPersistence(Document doc)
		{
			this.Document = doc;
		}

		/// <summary>
		/// Ignoring the stored document, if it exists, loads a new document
		/// </summary>
		/// <param name="f">The file to load the document from.</param>
		public abstract Document Load(string f);

		/// <summary>
		/// Save the specified f.
		/// </summary>
		/// <param name="f">The file to save the document into.</param>
		public abstract void Save(string f);

		/// <summary>
		/// The document saved or loaded.
		/// </summary>
		public Document Document {
			get; set;
		}
	}
}

