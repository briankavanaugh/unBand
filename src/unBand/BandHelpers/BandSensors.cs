using System.Threading.Tasks;
using Microsoft.Band.Admin;
using unBand.BandHelpers.Sensors;

namespace unBand.BandHelpers
{
    internal class BandSensors
    {
        private readonly CargoClient _client;

        public BandSensors(CargoClient client)
        {
            _client = client;
        }

        public BandPedometer Pedometer { get; set; }

        public async Task InitAsync()
        {
            Pedometer = new BandPedometer(_client);
            await Pedometer.InitAsync();
        }
    }
}