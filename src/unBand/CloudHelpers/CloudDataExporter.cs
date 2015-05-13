using System.Collections.Generic;

namespace unBand.CloudHelpers
{
    public abstract class CloudDataExporter
    {
        public abstract string DefaultExt { get; }
        public CloudDataExporterSettings Settings { get; set; }
        public abstract void ExportToFile(List<Dictionary<string, object>> data, string filename);
    }
}