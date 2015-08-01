using System;
using Newtonsoft.Json.Linq;

namespace unBand.Cloud
{
	internal class GolfEventHoleSequenceItem : EventBaseSequenceItem
	{
		public ExerciseEventSequenceType SequenceType { get; internal set; }
		public Guid LocationBlob { get; set; }
		public int HoleNumber { get; set; }
		public int DistanceToPinInCm { get; set; }
		public int HolePar { get; set; }
		public int HoleDifficultyIndex { get; set; }
		public int UserScore { get; set; }
		public string HoleShotOverlayImageUrl { get; set; }
		public int StepCount { get; set; }
		public int DistanceWalkedInCm { get; set; }

		public GolfEventHoleSequenceItem( JObject json )
			: base( json )
		{
			dynamic rawSequence = json;
			SequenceType = rawSequence.SequenceType;
			LocationBlob = rawSequence.LocationBlob;
			HoleNumber = rawSequence.HoleNumber;
			DistanceToPinInCm = rawSequence.DistanceToPinInCm;
			HolePar = rawSequence.HolePar;
			HoleDifficultyIndex = rawSequence.HoleDifficultyIndex;
			UserScore = rawSequence.UserScore;
			HoleShotOverlayImageUrl = rawSequence.HoleShotOverlayImageUrl;
			StepCount = rawSequence.StepCount;
			DistanceWalkedInCm = rawSequence.DistanceWalkedInCm;
		}
	}
}
