﻿
using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;


namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient(IConfiguration configuration, IMapper mapper) : IPlatformDataClient
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IMapper _mapper = mapper;

        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            Console.WriteLine($"--> Calling GRPC Service {_configuration["GrpcPlatform"]}");
            var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"]!);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not call GRPC Server {ex.Message}");
                return null;
            }
        }
    }
}
