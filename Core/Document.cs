using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Bareplan.Core {
	/// <summary>
	/// Represents documents with dates and tasks.
	/// The document is ordered by date, which has a corresponding task.
	/// </summary>
	public class Document {

		public const string TaskTag = "thema";

		public Document()
		{
			this.Steps = new Steps( this );
			this.dates = new List<DateTime>();
			this.tasks = new List<string>();
			this.InitialDate = DateTime.Now;
			this.hasNext = false;
			this.FileName = "";
		}

		public void AddLast()
		{
			DateTime date = this.LastDate;

			if ( this.CountDates > 0 ) {
				date = date.AddDays( this.Steps.NextStep );
			}

			this.dates.Add( date );
			this.tasks.Add( TaskTag + this.CountDates.ToString() );
		}

		public void Modify(int rowNumber, DateTime date, string task)
		{
			this.dates[ rowNumber ] = date;
			this.tasks[ rowNumber ] = task;
		}

		public void InsertTask(int i)
		{
			InsertTask( i, TaskTag + ( i + 1 ).ToString() );
		}

		public void InsertTask(int i, string task)
		{
			this.tasks.Insert( i, task );
			this.dates.Add( this.LastDate.AddDays( this.Steps.NextStep ) );
		}

		public void Remove(int i)
		{
			if ( this.CountDates > i ) {
				this.dates.RemoveAt( i );
				this.tasks.RemoveAt( i );
			}

			return;
		}

		public void RemoveTask(int i)
		{
			if ( this.CountDates > i ) {
				this.tasks.RemoveAt( i );
				this.tasks.Add( TaskTag + this.CountDates.ToString() );
			}

			return;
		}

		public void RemoveDate(int i)
		{
			if ( this.CountDates > i ) {
				this.dates.RemoveAt( i );
				this.dates.Add( this.LastDate.AddDays( this.Steps.NextStep ) );
			}

			return;
		}

		public KeyValuePair<DateTime, string> GotoFirst()
		{
			this.enumDates = this.dates.GetEnumerator();
			this.enumTasks = this.tasks.GetEnumerator();
			this.hasNext = true;

			return this.Next();
		}

		public KeyValuePair<DateTime, string> Next()
		{
			this.hasNext = ( this.enumDates.MoveNext() && this.enumTasks.MoveNext() );
			
			return new KeyValuePair<DateTime, string>( enumDates.Current, enumTasks.Current );
		}

		/// <summary>
		/// Gets the last date in the list of dates
		/// </summary>
		public DateTime LastDate {
			get {
				DateTime toret = this.InitialDate;
				int numDates = this.CountDates;

				if ( numDates > 0 ) {
					toret = this.dates [numDates - 1];
				}

				return toret;
			}
		}

		public bool IsEnd {
			get { return !this.hasNext; }
		}

		public int CountDates {
			get { return this.dates.Count; }
		}

		public int CountTasks {
			get { return this.tasks.Count; }
		}
			
		public DateTime GetDate(int i)
		{
			return this.dates[ i ];
		}

		public string GetTask(int i)
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

			return;
		}

		public ReadOnlyCollection<DateTime> Dates {
			get { return new ReadOnlyCollection<DateTime>( this.dates.ToArray() ); }
		}

		public ReadOnlyCollection<string> Tasks {
			get { return new ReadOnlyCollection<string>( this.tasks.ToArray() ); }
		}

		public string FileName {
			get; set;
		}

		public bool HasName {
			get { return ( this.FileName.Length > 0 ); }
		}

		public DateTime InitialDate {
			get {
				return this.initialDate;
			}
			set {
				if ( this.initialDate != value ) {
					this.initialDate = value;
					this.Recalculate();
				}
			}
		}

		public Steps Steps {
			get; private set;
		}

		private List<DateTime> dates;
		private List<string> tasks;
		private List<DateTime>.Enumerator enumDates;
		private List<string>.Enumerator enumTasks;
		private bool hasNext;
		private DateTime initialDate;
	}
}

