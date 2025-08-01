﻿using RystBrewery.Software.AlarmSystem;

namespace RystBrewery.Software.Services
{
    internal class RystIPAWashingService : WashingService
    {
        public RystIPAWashingService(AlarmService alarmService) : base(alarmService)
        {
        }
    }

    internal class RystEpleciderWashingService : WashingService
    {
        public RystEpleciderWashingService(AlarmService alarmService) : base(alarmService)
        {
        }
    }

    internal class RystSommerølWashingService : WashingService
    {
        public RystSommerølWashingService(AlarmService alarmService) : base(alarmService)
        {
        }
    }
}