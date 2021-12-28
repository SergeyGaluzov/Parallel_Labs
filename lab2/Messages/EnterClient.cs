using lab2.Models;

namespace lab2.Messages
{
    class EnterClient
    {
        public Client Client { get; }

        public EnterClient(Client client)
        {
            Client = client;
        }

    }
}
