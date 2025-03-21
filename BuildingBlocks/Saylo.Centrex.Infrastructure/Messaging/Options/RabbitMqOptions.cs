﻿namespace Saylo.Centrex.Infrastructure.Messaging.Options
{
    public class RabbitMqOptions
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ExchangeName { get; set; }
        public Dictionary<string, string> RoutingKeys { get; set; }
        public Dictionary<string, string> QueueNames { get; set; }

        public string ConnectionString => $"amqp://{UserName}:{Password}@{HostName}/%2f";
    }
}
