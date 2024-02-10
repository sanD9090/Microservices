using PlatformService.Dtos;

namespace PlatformService.AsyncDataServices
{
    public interface IMessagBusClient
    {
            void PublishNewPlatform(PlatformPublishedDto platformPublishedDto);
    }
}
