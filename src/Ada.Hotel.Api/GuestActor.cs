using System;
using Ada.Hotel.Api.Models;
using Akka.Persistence;

namespace Ada.Hotel.Api
{
    internal class GuestActor : PersistentActor
    {
        private string _firstname;
        private string _lastname;
        private string _phone;
        private string _persistenceId;

        protected override bool ReceiveRecover(object message)
        {
            var guest = (RegisterGuestCommand)message;
            _firstname = guest.Firstname;
            _lastname = guest.Lastname;
            _phone = guest.Phone;
            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            var guest = (RegisterGuestCommand)message;
            _firstname = guest.Firstname;
            _lastname = guest.Lastname;
            _phone = guest.Phone;
            Console.WriteLine($"Registered {_firstname} {_lastname}, {_phone}");

            _persistenceId = Guid.NewGuid().ToString();
            return true;
        }

        public override string PersistenceId {
            get { return _persistenceId;  }
        }
    }
}