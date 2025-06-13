using RystBrewery.Software.AlarmSystem;

namespace RystBrewery.Software.Services
{
    internal class RystIPABrewingService : BrewingService
    {
        public RystIPABrewingService(AlarmService alarmService) : base(alarmService)
        {
        }
    }

    internal class RystEpleciderBrewingService : BrewingService
    {
        public RystEpleciderBrewingService(AlarmService alarmService) : base(alarmService)
        {
        }
    }

    internal class RystSommerølBrewingService : BrewingService
    {
        public RystSommerølBrewingService(AlarmService alarmService) : base(alarmService)
        {
        }
    }
}