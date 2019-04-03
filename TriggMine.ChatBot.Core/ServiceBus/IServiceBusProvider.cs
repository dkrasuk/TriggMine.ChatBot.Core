using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriggMine.ChatBot.Core.ServiceBus
{
    public interface IServiceBusProvider
    {
        Task SendMessageToServiceBus(string message);
        void RegisterOnMessageHandlerAndReceiveMessages();
    }
}
