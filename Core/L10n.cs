// Bareplan (c) 2015-17 MIT License <baltasarq@gmail.com>

namespace Bareplan.Core {
	using System.Globalization;
	using System.Collections.ObjectModel;

	/// <summary>
	/// The localization 
	/// </summary>
	public static class L10n {
		/// <summary>An identifier for localized strings.</summary>
		public enum Id {
			MnFile,
			MnEdit,
			MnView,
			MnHelp,
			OpQuit,
			OpAbout,
			OpOpen,
			OpSave,
			OpSaveAs,
			OpClose,
			OpExport,
			OpSettings,
			OpProperties,
			OpIncFont,
			OpDecFont,
			OpInsert,
			OpInsertDate,
			OpInsertTask,
			OpAdd,
			OpRemove,
			OpRemoveDate,
			OpRemoveTask,
			OpNew,
			LblLanguage,
			LblInitialDate,
			LblSteps,
			StReady,
			StReadingConfig,
			StWritingConfig,
			StInserting,
			StRemoving,
			HdNum,
			HdSession,
			HdWeek,
			HdDay,
			HdDate,
			HdTask,
			ErNoDocument,
			ErOutOfRange,
			ErManagingXML,
			ErAccessingFile,
		};

		///<summary>The collection of localized Spanish strings.</summary>
		public static readonly ReadOnlyCollection<string> StringsEN =
				new ReadOnlyCollection<string>( new string[] {
				"File",
				"Edit",
				"View",
				"Help",
				"Quit",
				"About...",
				"Open",
				"Save",
				"Save as...",
				"Close",
				"Export",
				"Preferences",
				"Properties",
				"Increment font size",
				"Decrement font size",
				"Insert row",
				"Insert date",
				"Insert task",
				"Add row",
				"Remove row",
				"Remove date",
				"Remove task",
				"New",
				"Language",
				"Initial date",
				"Steps",
				"Ready",
				"Error while reading configuration file",
				"Error while writing configuration file",
				"Insertando",
				"Eliminando",
				"#",
				"Session",
				"Week",
				"Day",
				"Date",
				"Task",
				"ERROR: unexisting document",
				"ERROR: out of range",
				"ERROR: managing XML",
				"ERROR: accessing file",
			}
		);

		///<summary>The collection of localized Spanish strings.</summary>
		public static readonly ReadOnlyCollection<string> StringsES =
			new ReadOnlyCollection<string>( new string[] {
				"Archivo",
				"Editar",
				"Ver",
				"Ayuda",
				"Salir",
				"Acerca de...",
				"Abrir",
				"Guardar",
				"Guardar como...",
				"Cerrar",
				"Exportar",
				"Preferencias",
				"Propiedades",
				"Incrementar medida de fuente",
				"Decrement medida de fuente",
				"Inserta fila",
				"Inserta fecha",
				"Insertar tarea",
				"Agregar fila",
				"Eliminar fila",
				"Eliminar fecha",
				"Eliminar tarea",
				"Nuevo",
				"Idioma",
				"Fecha de inicio",
				"Pasos",
				"Preparado",
				"Error leyendo archivo de preferencias",
				"Error escribiendo archivo de preferencias",
				"Inserting",
				"Removing",
				"#",
				"Sesión",
				"Semana",
				"Día",
				"Fecha",
				"Tarea",
				"ERROR: no existe documento",
				"ERROR: fuera de rango",
				"ERROR: manejando XML",
				"ERROR: accediendo a archivo",
			}
		);

		private static ReadOnlyCollection<string> strings = StringsEN;

		/// <summary>Sets the language for this app.</summary>
		/// <param name="locale">A <see cref="CultureInfo"/> locale.</param>
		public static void SetLanguage(CultureInfo locale)
		{

			if ( locale.TwoLetterISOLanguageName.ToUpper() == "ES" ) {
				strings = StringsES;
			}
			else
			if ( locale.TwoLetterISOLanguageName.ToUpper() == "EN" ) {
				strings = StringsEN;
			}

			return;
		}

		/// <summary>Gets the localized string for the given id.</summary>
		/// <returns>The localized string.</returns>
		/// <param name="id">An identifier, as an <see cref="T:Id"/>.</param>
		public static string Get(Id id)
		{
			string toret = null;
			int numId = (int) id;

			if ( numId < strings.Count ) {
				toret = strings[ numId ];
			}

			return toret;
		}
	}
}
