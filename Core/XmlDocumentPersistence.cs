// Bareplan (c) 2015-17 MIT License <baltasarq@gmail.com>
	
namespace Bareplan.Core {
	using System;
	using System.Xml;
	using System.Text;
	using System.Collections.Generic;
	using System.Globalization;

	public class XmlDocumentPersistence: DocumentPersistence {
		public const string DefaultExt = "bar";
		public const string TasksTag = "tasks";
		public const string TaskTag = "task";
		public const string StepsTag = "steps";
		public const string InitialDateTag = "initialdate";
		public const string DateTag = "date";

		/// <summary>
		/// Build a new document saver/loader by XML <see cref="XmlDocumentPersistence"/> class.
		/// </summary>
		/// <param name='doc'>
		/// The document, that can be null if its going to be loaded.
		/// </param>
		public XmlDocumentPersistence(Document doc): base( doc )
		{
		}

		/// <summary>
		/// Save the specified f.
		/// </summary>
		/// <param name="f">The file to save the document into.</param>
		public override void Save(string f)
		{
			// Document encoding
			var settings = new XmlWriterSettings {
				Encoding = Encoding.UTF8,
				Indent = true
			};

			// Start writing
			var writer = XmlWriter.Create( f, settings );
			writer.WriteStartElement( TasksTag );

			// Write initial date
			writer.WriteStartElement( InitialDateTag );
			writer.WriteString( this.Document.InitialDate.ToString( "yyyy-MM-dd" ) );
			writer.WriteEndElement();

			// Write steps
			writer.WriteStartElement( StepsTag );
			writer.WriteString( String.Join( ",", this.Document.Steps ) );
			writer.WriteEndElement();

			// Write date, task pairs
			KeyValuePair<DateTime, string> pair = this.Document.GotoFirst();
			while( !this.Document.IsEnd ) {
				// Tasks
				writer.WriteStartElement( TaskTag );
				writer.WriteAttributeString( DateTag,  pair.Key.ToString( "yyyy-MM-dd" ) );
				writer.WriteString( pair.Value );
				writer.WriteEndElement();

				pair = this.Document.Next();
			}

			// Close main node
			writer.WriteEndElement();
			writer.Close();
			this.Document.NeedsSaving = false;
			return;
		}

		/// <summary>
		/// Ignoring the stored document, if it exists, loads a new document
		/// </summary>
		/// <param name="f">The file to load the document from.</param>
		public override Document Load(string f)
		{
			var xmlDoc = new XmlDocument();
			var doc = new Document();
			bool initialDateSet = false;

			xmlDoc.Load( f );

			// Interpret nodes
			var mainNode = xmlDoc.DocumentElement;

			if ( mainNode.Name.ToLower() == TasksTag ) {
				foreach(XmlNode node in mainNode.ChildNodes) {
					var element = ( node as XmlElement );
					string elementName = element.Name.ToLower();

					if ( element != null ) {
						if ( elementName == TaskTag ) {
							string task;
							var date = element.Attributes.GetNamedItem( DateTag );

							if ( date != null ) {
								task = element.InnerText;
							} else {
								throw new XmlException( "missing date in task" );
							}

							doc.AddLast();
							doc.Modify( doc.CountDates - 1,
							           DateTime.Parse( date.InnerText ),
							           task
							);
						}
						else
						if ( elementName == StepsTag ) {
							doc.Steps.SetSteps( element.InnerText );
						}
						else
						if ( elementName == InitialDateTag ) {
							initialDateSet = DateTime.TryParseExact(
														element.InnerText,
														"yyyy-MM-dd",
														CultureInfo.InvariantCulture,
														DateTimeStyles.None,
														out DateTime result );
							if ( initialDateSet ) {
								doc.InitialDate = result;
							}
						}
					}
					
				}
			} else {
				throw new XmlException( "missing main node: " + TasksTag );
			}

			// Finish document configuration
			if ( !initialDateSet ) {
				if ( doc.CountDates > 0 ) {
					doc.InitialDate = doc.GetDate( 0 );
				} else {
					doc.InitialDate = DateTime.Now;
				}
			}

			this.Document = doc;
			this.Document.NeedsSaving = false;
			return doc;
		}
	}
}

