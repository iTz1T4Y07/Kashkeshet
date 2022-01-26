using Kashkeshet.Common;
using Kashkeshet.ConsoleUI;
using Kashkeshet.LogicBll;
using Kashkeshet.NetworkBll;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kashkeshet.Client
{
    public class Bootstrapper
    {
        private ConsoleScreen _consoleScreen;

        public ServerCommunicator GetServerCommunicator()
        {
            Guid defaultGuid = Guid.Empty;

            ChatUpdater updater = new ChatUpdater(defaultGuid, new Dictionary<Guid, Chat>());
            OperationHandler operationHandler = new OperationHandler(updater);
            ServerCommunicator serverCommunicator = new ServerCommunicator(defaultGuid, operationHandler, new DataSerializer(), new DataDeserializer());

            ChatInformationExtractor chatInformationExtractor = new ChatInformationExtractor(updater);
            _consoleScreen = new ConsoleScreen(chatInformationExtractor, serverCommunicator, updater);

            return serverCommunicator;
        }

        public ConsoleScreen GetConsoleScreen() => _consoleScreen;
    }
}
