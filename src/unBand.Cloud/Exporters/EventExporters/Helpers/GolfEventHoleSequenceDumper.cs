using System.Collections.Generic;

namespace unBand.Cloud.Exporters.EventExporters.Helpers {
	internal static class GolfEventHoleSequenceDumper {
		internal static Dictionary<string, object> Dump( GolfEventHoleSequenceItem item ) {
			return new Dictionary<string, object>( BaseSequenceDumper.Dump( item ) )
			{
				{"Sequence Type", item.SequenceType},
				{"Hole Number", item.HoleNumber},
				{"Distance to Pin (cm)", item.DistanceToPinInCm},
				{"Hole Par", item.HolePar},
				{"Hole Difficulty Index", item.HoleDifficultyIndex},
				{"User Score", item.UserScore},
				{"Hole Shot Overlay Image Url", item.HoleShotOverlayImageUrl},
				{"Step Count", item.StepCount},
				{"Distance Walked (cm)", item.DistanceWalkedInCm},
				{"Heart Rate Zone - Aerobic", item.HeartRate.Zones.Aerobic},
				{"Heart Rate Zone - Anaerobic", item.HeartRate.Zones.Anaerobic},
				{"Heart Rate Zone - Fitness Zone", item.HeartRate.Zones.FitnessZone},
				{"Heart Rate Zone - Healthy Heart", item.HeartRate.Zones.HealthyHeart},
				{"Heart Rate Zone - Over", item.HeartRate.Zones.Over},
				{"Heart Rate Zone - Red Line", item.HeartRate.Zones.RedLine},
				{"Heart Rate Zone - Under", item.HeartRate.Zones.Under},
			};
		}
	}
}
