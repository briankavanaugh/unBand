using System.Threading.Tasks;
using Microsoft.Band.Admin;
using unBand.BandHelpers.Sensors;

namespace unBand.BandHelpers
{
    internal class BandSensors
    {
        private readonly ICargoClient _client;

        public BandSensors(ICargoClient client)
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