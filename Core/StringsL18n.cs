using System;
using System.Globalization;
using System.Collections.ObjectModel;

namespace Bareplan.Core {
	public static class StringsL18n {

		public enum StringId {
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
			HdDay,
			HdDate,
			HdTask,
			ErNoDocument,
			ErOutOfRange,
			ErManagingXML,
			ErAccessingFile,
		};

		public static readonly ReadOnlyCollection<string> StringsEN =
				new ReadOnlyCollection<string>( new string[] {
				"&File",
				"&Edit",
				"&View",
				"&Help",
				"&Quit",
				"&About...",
				"&Open",
				"&Save",
				"&Save as...",
				"&Close",
				"&Export",
				"&Preferences",
				"Pr&operties",
				"&Increment font size",
				"&Decrement font size",
				"&Insert row",
				"Insert date",
				"Insert task",
				"&Add row",
				"&Remove row",
				"Remove date",
				"Remove task",
				"&New",
				"Language",
				"Initial date",
				"Steps",
				"Ready",
				"Error while reading configuration file",
				"Error while writing configuration file",
				"Insertando",
				"Eliminando",
				"#",
				"Day",
				"Date",
				"Task",
				"ERROR: unexisting document",
				"ERROR: out of range",
				"ERROR: managing XML",
				"ERROR: accessing file",
			}
		);

		public static readonly ReadOnlyCollection<string> StringsES =
			new ReadOnlyCollection<string>( new string[] {
				"&Archivo",
				"&Editar",
				"&Ver",
				"&Ayuda",
				"&Salir",
				"&Acerca de...",
				"&Abrir",
				"&Guardar",
				"&Guardar como...",
				"&Cerrar",
				"&Exportar",
				"&Preferencias",
				"Pr&opiedades",
				"&Incrementar medida de fuente",
				"&Decrement medida de fuente",
				"&Inserta fila",
				"Inserta fecha",
				"Insertar tarea",
				"Agregar &fila",
				"&Eliminar fila",
				"Eliminar fecha",
				"Eliminar tarea",
				"&Nuevo",
				"Idioma",
				"Fecha de inicio",
				"Pasos",
				"Preparado",
				"Error leyendo archivo de preferencias",
				"Error escribiendo archivo de preferencias",
				"Inserting",
				"Removing",
				"#",
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

		public static string Get(StringId id)
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
