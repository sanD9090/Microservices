using System.Text.Json;
using System.Windows.Input;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
    public class EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper) : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly IMapper _mapper = mapper;

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;


            }
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

            try
            {
                var plat = _mapper.Map<Platform>(platformPublishedDto);
                if(!repo.ExternalPlatformExists(plat.ExternalId))
                {
                    repo.CreatePlatform(plat);
                    repo.SaveChanges();
                    Console.WriteLine("Platform Added Successfully.");

                }
                else
                {
                    Console.WriteLine("Platform already exists.");
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Couldn't add platform to DB {ex.Message}");
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining Event");
            var eventType  = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch(eventType.Event) {
                case "Platform_Published":
                    Console.WriteLine("--> Platform Published Event Detected.");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Couldn't determine the event type.");
                    return EventType.Undetermined;

            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}
