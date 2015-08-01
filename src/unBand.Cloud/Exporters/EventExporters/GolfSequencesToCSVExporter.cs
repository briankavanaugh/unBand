using System;
using System.Linq;
using System.Threading.Tasks;
using unBand.Cloud.Exporters.EventExporters.Helpers;

namespace unBand.Cloud.Exporters.EventExporters
{
	internal class GolfSequencesToCSVExporter : IEventExporter
	{
		#region Singleton

		private static IEventExporter _theOne;

		public static IEventExporter Instance
		{
			get { return _theOne ?? (_theOne = new GolfSequencesToCSVExporter()); }
		}

		private GolfSequencesToCSVExporter()
		{
		}

		#endregion

		public string DefaultExtension
		{
			get { return ".csv"; }
		}

		public string DefaultExportSuffix
		{
			get { return "sequence"; }
		}

		public async Task ExportToFile(BandEventBase eventBase, string filePath)
		{
			var golfEvent = eventBase as GolfEvent;
			if (golfEvent == null)
			{
				throw new ArgumentException("eventBase must be of type GolfEvent to use the RunToCSVExporter");
			}

			await Task.Run(() =>
			{
				var dataDump = golfEvent.Sequences
					.Select(sequence => sequence as GolfEventHoleSequenceItem)
					.Select(GolfEventHoleSequenceDumper.Dump).ToList();

				// TODO: pass through convertDateTimeToLocal
				CSVExporter.ExportToFile(dataDump, filePath);
			});
		}
	}
}
