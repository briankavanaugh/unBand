using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json.Linq;
using unBand.Cloud.Exporters.EventExporters;

namespace unBand.Cloud {
	public class GolfEventConverter : TypeConverter {
		public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value ) {
			var o = value as JObject;
			return o != null ? new GolfEvent( o ) : null;
		}
	}
	[TypeConverter( typeof( GolfEventConverter ) )]
	public class GolfEvent : BandEventBase {

		public override string FriendlyEventType { get { return "Golf"; } }
		//public override string PrimaryMetric { get { return ( TotalDistanceWalkedInCm / 100000.0 ).ToString( "N", CultureInfo.InvariantCulture ) + "km"; } }
		public override string PrimaryMetric {
			get { return String.Format("{0} for {1}", TotalScore, TotalHolesPlayed); }
		}
		public GolfEvent( JObject json )
			: base( json )
		{
			dynamic eventSummary = json;

			UsedAsEvidence = eventSummary.UsedAsEvidence;
			CourseID = eventSummary.CourseID;
			CourseDataVersion = eventSummary.CourseDataVersion;
			CourseName = eventSummary.CourseName;
			CoursePar = eventSummary.CoursePar;
			TotalHolesAtCourse = eventSummary.TotalHolesAtCourse;
			TotalHolesPlayed = eventSummary.TotalHolesPlayed;
			TotalScore = eventSummary.TotalScore;
			ParForHolesPlayed = eventSummary.ParForHolesPlayed;
			ParOrBetterCount = eventSummary.ParOrBetterCount;
			LongestDriveInCm = eventSummary.LongestDriveInCm;
			LongestStrokeInCm = eventSummary.LongestStrokeInCm;
			PaceOfPlayInSeconds = eventSummary.PaceOfPlayInSeconds;
			LowestScoreOverParForHole = eventSummary.LowestScoreOverParForHole;
			TeeNameSelected = eventSummary.TeeNameSelected;
			TotalStepCount = eventSummary.TotalStepCount;
			TotalDistanceWalkedInCm = eventSummary.TotalDistanceWalkedInCm;
			GPSState = eventSummary.GPSState;
			TmaGEventId = eventSummary.TmaGEventId;
		}

		public int TimeZoneOffsetMinutes { get; set; }
		public bool UsedAsEvidence { get; set; }
		public int CourseID { get; set; }
		public int CourseDataVersion { get; set; }
		public string CourseName { get; set; }
		public int CoursePar { get; set; }
		public int TotalHolesAtCourse { get; set; }
		public int TotalHolesPlayed { get; set; }
		public int TotalScore { get; set; }
		public int ParForHolesPlayed { get; set; }
		public int ParOrBetterCount { get; set; }
		public int LongestDriveInCm { get; set; }
		public int LongestStrokeInCm { get; set; }
		public int PaceOfPlayInSeconds { get; set; }
		public int LowestScoreOverParForHole { get; set; }
		public string TeeNameSelected { get; set; }
		public int TotalStepCount { get; set; }
		public int TotalDistanceWalkedInCm { get; set; }
		public int GPSState { get; set; }
		public string TmaGEventId { get; set; }

		public override Dictionary<string, object> DumpBasicEventData()
		{
			var rv = new Dictionary<string, object>(base.DumpBasicEventData())
			{
				{"Total Distance Walked (cm)", TotalDistanceWalkedInCm.ToString()},
				{"Course Name", CourseName},
				{"Course Par", CoursePar},
				{"Holes at Course", TotalHolesPlayed},
				{"Holes Played", TotalHolesPlayed},
				{"Score", TotalScore},
				{"Par for Holes Played", ParForHolesPlayed},
				{"Par or Better Count", ParOrBetterCount},
				{"Longest Drive (cm)", LongestDriveInCm.ToString()},
				{"Longest Stroke (cm)", LongestStrokeInCm.ToString()},
				{"Pace of Play (seconds)", PaceOfPlayInSeconds},
				{"Lowest Score Over Par for Hole", LowestScoreOverParForHole},
				{"Tees Played", TeeNameSelected},
				{"Total Steps", TotalStepCount}
			};


			return rv;
		}

		private static List<IEventExporter> _exporters;

		public override List<IEventExporter> Exporters {
			get {
				if( _exporters == null ) {
					_exporters = new List<IEventExporter> { GolfSequencesToCSVExporter.Instance };
					_exporters.AddRange( base.Exporters );
				}

				return _exporters;
			}
		}

		public override BandEventExpandType[ ] Expanders {
			get { return new[ ] { BandEventExpandType.Info, BandEventExpandType.Sequences }; }
		}

		public override void InitFullEventData( JObject json ) {
			base.InitFullEventData( json );

			dynamic fullEvent = json;

			foreach( var sequenceData in fullEvent.value[ 0 ].Sequences ) {
				Sequences.Add( new GolfEventHoleSequenceItem( sequenceData ) );
			}
		}
	}
}
