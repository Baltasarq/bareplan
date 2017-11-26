// Bareplan (c) 2015-17 MIT License <baltasarq@gmail.com>

namespace Bareplan.Core {
	using System.Text;
	using System.Collections.ObjectModel;
	using System.Collections.Generic;

	/// <summary>
	/// Represents a secquence of steps. Each step is a number of days.
	/// For example, a valid secquence could be: 0,2
	/// Which means that the first step is in the current day, and the next two days later.
	/// </summary>
	public class Steps {
		public Steps(Document doc)
		{
			this.Document = doc;
			this.steps = new List<int>();
			this.steps.Add( 0 );
			this.GotoFirstStep();
		}

		public int GotoFirstStep() {
			this.nextStep = 0;
			return this.steps[ 0 ];
		}

		public int GetStep(int i) {
			return this.steps[ i ];
		}

		/// <summary>
		/// Gets the next step. Steps are integers which represent days.
		/// </summary>
		/// <value>The next step, as an int.</value>
		public int NextStep {
			get {
				int toret = this.steps[ this.nextStep ];
				this.nextStep++;
				this.nextStep = this.nextStep % this.steps.Count;
				return toret;
			}
		}

		/// <summary>
		/// Sets all the steps,
		/// </summary>
		/// <param name="strSteps">
		/// The steps, as a space, comma, dash or semicolon separated values.
		/// </param>
		public void SetSteps(string strSteps)
		{
			this.steps.Clear();

			// Reduce string to canonical format
			strSteps = strSteps.Trim();
			strSteps = strSteps.Replace( ',', ' ' );
			strSteps = strSteps.Replace( '-', ' ' );
			strSteps = strSteps.Replace( ';', ' ' );

			// Create vector of steps
			foreach (var e in strSteps.Split()) {
				string value = e.Trim();

				if ( value.Length > 0 ) {
					int result;

					if ( int.TryParse( value, out result ) ) {
						this.steps.Add( result );
					}
				}
			}

			this.GotoFirstStep();
			this.Document.Recalculate();
			return;
		}

		public ReadOnlyCollection<int> StepsAsArray {
			get { return new ReadOnlyCollection<int>( this.steps.ToArray() ); }
			set {
				this.steps.Clear();
				this.steps.AddRange( value );
				this.GotoFirstStep();
			}
		}

		public override string ToString()
		{
			int numSteps = this.steps.Count;
			StringBuilder toret = new StringBuilder();

			for (int i = 0; i < numSteps; ++i) {
				toret.Append( this.steps[ i ].ToString() );

				if ( i < ( numSteps -1 ) ) {
					toret.Append( ',' );
				}
			}

			return toret.ToString().TrimEnd();
		}

		public Document Document {
			get; private set;
		}

		private List<int> steps;
		private int nextStep;
	}
}

