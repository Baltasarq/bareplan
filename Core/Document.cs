// Bareplan (c) 2015-17 MIT License <baltasarq@gmail.com>

namespace Bareplan.Core {
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	
	using Pair = System.Collections.Generic.KeyValuePair<System.DateTime, Document.Task>;

	/// <summary>
	/// Represents documents with dates and tasks.
	/// The document is ordered by date, which has a corresponding task.
	/// </summary>
	public class Document {
		/// <summary>A task, associated with a date.</summary>
		public class Task {
			/// <summary>The default task one is first created.</summary>
			public const string KindTag = "exercises";

			/// <summary>The default task one is first created.</summary>
			public const string ContentsTag = "thema";

			/// <summary>Creates a new task with default kind and contents = contentags + i.
			/// Initializes a new instance of the <see cref="T:Bareplan.Core.Document.Task"/> class.
			/// </summary>
			/// <param name="i">The index.</param>
			public Task(int i)
				: this( KindTag, ContentsTag + i.ToString() )
			{				
			}

			public Task(string kind = KindTag, string contents = ContentsTag)
			{
				this.Kind = kind;
				this.Contents = contents;
			}

			/// <summary>
			/// Gets or sets the contents for this task.
			/// </summary>
			/// <value>The contents, as a string.</value>
			public string Contents {
				get; set;
			}

			/// <summary>
			/// Gets or sets the kind of task.
			/// </summary>
			/// <value>The kind, as a string.</value>
			public string Kind {
				get; set;
			}

			public override string ToString()
			{
				return this.Kind + ": " + this.Contents;
			}
		}

		/// <summary>
		/// Initializes a new <see cref="T:Bareplan.Core.Document"/>.
		/// </summary>
		public Document()
		{
			this.Steps = new Steps( this );
			this.dates = new List<DateTime>();
			this.tasks = new List<Task>();
			this.InitialDate = DateTime.Now;
			this.hasNext = false;
			this.FileName = "";
			this.NeedsSaving = true;
		}

		/// <summary>
		/// Adds a new date/pair as the last row.
		/// </summary>
		public void AddLast()
		{
			DateTime date = this.LastDate;

			if ( this.CountDates > 0 ) {
				date = date.AddDays( this.Steps.NextStep );
			}

			this.dates.Add( date );
			this.tasks.Add( new Task( this.CountDates + 1 ) );
			this.NeedsSaving = true;
		}

		/// <summary>
		/// Modify the specified the date/task at the given row number.
		/// </summary>
		/// <param name="rowNumber">The row number to modify.</param>
		/// <param name="date">A <see cref="DateTime"/>.</param>
		/// <param name="task">A <see cref="Task"/>.</param>
		public void Modify(int rowNumber, DateTime date, Task task)
		{
			this.dates[ rowNumber ] = date;
			this.tasks[ rowNumber ] = task;
			this.NeedsSaving = true;
		}

		/// <summary>
		/// Inserts a new date/task pair before the given position.
		/// </summary>
		/// <param name="i">The index of the position to insert in.</param>
		public void InsertRow(int i)
		{
			this.tasks.Insert( i, new Task( i + 1 ) );
			this.dates.Insert( i, this.dates[ i ] );
			this.NeedsSaving = true;
		}

		/// <summary>
		/// Inserts a <see cref="Task"/> with the default values.
		/// </summary>
		/// <param name="i">The index of the position to insert in.</param>
		/// <seealso cref="M:InsertTask"/>		
		public void InsertTask(int i)
		{
			this.InsertTask( i, new Task( i + 1 ) );
		}

		/// <summary>
		/// Inserts the task with the given value.
		/// Adds a new date at the end of dates,
		/// so both lists are kept of equal length.
		/// </summary>
		/// <param name="i">The index.</param>
		/// <param name="task">The <see cref="Task"/>.</param>
		public void InsertTask(int i, Task task)
		{
			this.tasks.Insert( i, task );
			this.dates.Add( this.LastDate.AddDays( this.Steps.NextStep ) );
			this.NeedsSaving = true;
		}

		/// <summary>
		/// Inserts a date, honoring the steps.
		/// A new task (with default contents) is added at the end of tasks,
		/// so both lists are kept of equal length.
		/// </summary>
		/// <param name="rowNumber">The position to insert in.</param>
		/// <seealso cref="Task"/> 
		public void InsertDate(int rowNumber)
		{
			int count = this.tasks.Count;

			// Insert
			this.dates.Insert( rowNumber, this.dates[ rowNumber ] );
			this.tasks.Add( new Task( count + 1 ) );
			
			this.NeedsSaving = true;
		}

		/// <summary>
		/// Removes the pair date/task at the specified position.
		/// </summary>
		/// <param name="i">The position to remove.</param>
		public void Remove(int i)
		{
			if ( this.CountDates > i ) {
				this.dates.RemoveAt( i );
				this.tasks.RemoveAt( i );
			}

			this.NeedsSaving = true;
		}

		/// <summary>
		/// Removes the task at a given position.
		/// A new task is added at the end of the tasks,
		/// so both lists are kept of equal length.
		/// </summary>
		/// <param name="i">The index.</param>
		public void RemoveTask(int i)
		{
			if ( this.CountDates > i ) {
				this.tasks.RemoveAt( i );
				this.tasks.Add( new Task( this.CountDates ) );
			}

			this.NeedsSaving = true;
		}

		/// <summary>
		/// Removes the date at a given position.
		/// A new date is added at the end of the dates,
		/// so both lists are kept of equal length.
		/// </summary>
		/// <param name="i">The index.</param>
		public void RemoveDate(int i)
		{
			if ( this.CountDates > i ) {
				this.dates.RemoveAt( i );
				this.dates.Add( this.LastDate.AddDays( this.Steps.NextStep ) );
			}

			this.NeedsSaving = true;
		}

		/// <summary>
		/// Goes to the first date/task entry.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.KeyValuePair"/> with date&amp;task.
		/// </returns>
		public Pair GotoFirst()
		{
			this.enumDates = this.dates.GetEnumerator();
			this.enumTasks = this.tasks.GetEnumerator();
			this.hasNext = true;

			return this.Next();
		}

		/// <summary>
		/// Returns the current date/task entry.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.KeyValuePair"/> with date&amp;task.
		/// </returns>
		public Pair Next()
		{
			this.hasNext = ( this.enumDates.MoveNext() && this.enumTasks.MoveNext() );
			
			return new KeyValuePair<DateTime, Task>( enumDates.Current, enumTasks.Current );
		}

		/// <summary>
		/// Gets the last date in the list of dates
		/// </summary>
		public DateTime LastDate {
			get {
				DateTime toret = this.InitialDate;
				int numDates = this.CountDates;

				if ( numDates > 0 ) {
					toret = this.dates[ numDates - 1 ];
				}

				return toret;
			}
		}

		/// <summary>
		/// Tells whether the end of the document has been reached.
		/// </summary>
		/// <returns><c>true</c>, if end was reached, <c>false</c> otherwise.</returns>
		/// <seealso cref="M:Document.Next"/>
		/// <seealso cref="M:Document.GotoFirst"/>
		public bool IsEnd()
		{
			return !this.hasNext;
		}

		/// <summary>
		/// Gets the number of stored dates.
		/// </summary>
		/// <value>The count of dates.</value>
		public int CountDates {
			get { return this.dates.Count; }
		}

		/// <summary>
		/// Gets the number of stored tasks.
		/// </summary>
		/// <value>The count of tasks.</value>
		public int CountTasks {
			get { return this.tasks.Count; }
		}
			
		/// <summary>
		/// Gets the date at the given position.
		/// </summary>
		/// <returns>A <see cref="System.DateTime"/>.</returns>
		/// <param name="i">The index.</param>
		public DateTime GetDate(int i)
		{
			return this.dates[ i ];
		}

		/// <summary>
		/// Gets the task at the given position.
		/// </summary>
		/// <returns>The task.</returns>
		/// <param name="i">The index.</param>
		public Task GetTask(int i)
		{
			return this.tasks[ i ];
		}

		/// <summary>
		/// Recalculates dates after changing initial date or steps.
		/// </summary>
		public void Recalculate() {
			DateTime currentDate = this.InitialDate;
			this.Steps.GotoFirstStep();
			int currentStep = this.Steps.NextStep;

			for (int i = 0; i < this.dates.Count; ++i) {
				this.dates[ i ] = currentDate;

				currentDate = currentDate.AddDays( currentStep );
				currentStep = this.Steps.NextStep;
			}

			this.NeedsSaving = true;
		}

		/// <summary>
		/// Gets all dates.
		/// </summary>
		/// <value>The dates, as a <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection"/>.</value>
		public ReadOnlyCollection<DateTime> Dates {
			get { return new ReadOnlyCollection<DateTime>( this.dates.ToArray() ); }
		}

		/// <summary>
		/// Gets all tasks.
		/// </summary>
		/// <value>The tasks, as a <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection"/>.</value>
		public ReadOnlyCollection<Task> Tasks {
			get { return new ReadOnlyCollection<Task>( this.tasks.ToArray() ); }
		}

		/// <summary>
		/// Gets or sets the name of the file for this planning document.
		/// </summary>
		/// <value>The name of the file.</value>
		public string FileName {
			get; set;
		}

		/// <summary>
		/// Gets a value indicating whether
		/// this <see cref="T:Bareplan.Core.Document"/> has a file name.
		/// </summary>
		/// <value><c>true</c> if has name; otherwise, <c>false</c>.</value>
		public bool HasName {
			get { return ( this.FileName.Length > 0 ); }
		}

		/// <summary>
		/// Gets or sets the initial date.
		/// </summary>
		/// <value>The initial date, as a <see cref="T:System.DateTime"/>.</value>
		public DateTime InitialDate {
			get {
				return this.initialDate;
			}
			set {
				if ( this.initialDate != value ) {
					this.initialDate = value;
					this.Recalculate();
					this.NeedsSaving = true;
				}
			}
		}

		/// <summary>
		/// Gets the steps for creating new date/task pairs.
		/// A step is an amout in days.
		/// </summary>
		/// <value>The <see cref="Steps"/>.</value>
		public Steps Steps {
			get; private set;
		}

		/// <summary>
		/// Looks for tasks in the given date.
		/// </summary>
		/// <returns>A vector with the positions of the tasks.</returns>
		/// <param name="date">A DateTime object, containing the date to look for.</param>
		public int[] LookForTasksIn(DateTime date)
		{
			var toret = new List<int>();
			int pos = 0;

			System.Console.WriteLine("-- Looking for: " + date);
			while( pos < this.dates.Count ) {
				System.Console.WriteLine("Considering " + this.dates[pos]);
				// There are no more possible matching dates (they are sorted).
				if ( this.dates[ pos ] > date ) {
					break;
				}

				if ( this.dates[ pos ] == date ) {
					toret.Add( pos );
				}
					
				++pos;
			}

			return toret.ToArray();
		}
		
		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Bareplan.Core.Document"/> needs saving.
		/// </summary>
		/// <value><c>true</c> if needs saving; otherwise, <c>false</c>.</value>
		public bool NeedsSaving {
			get; internal set;
		}

		List<DateTime> dates;
		List<Task> tasks;
		List<DateTime>.Enumerator enumDates;
		List<Task>.Enumerator enumTasks;
		bool hasNext;
		DateTime initialDate;
	}
}
