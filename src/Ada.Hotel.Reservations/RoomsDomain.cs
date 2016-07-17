using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Ada.Hotel.Reservations.Commands;
using Cqrs;

namespace Ada.Hotel.Reservations
{
    public class RoomsDomain
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;
        private readonly IMessageBus _messageBus;

        public RoomsDomain(IAggregateRootRepository aggregateRootRepository, IMessageBus messageBus)
        {
            _aggregateRootRepository = aggregateRootRepository;
            _messageBus = messageBus;
        }

        public void Consume(CreateRooms createRooms)
        {
            ConsumeInternal(createRooms.HotelId, x => x.Create(createRooms.HotelId, createRooms.RoomTypeId, createRooms.RoomType, createRooms.NoOfUnits));
        }

        private void ConsumeInternal(Guid aggregateId, Func<Rooms, IEnumerable<object>> action)
        {
            using (new Mutex(true, aggregateId.ToString()))
            {
                var rooms = _aggregateRootRepository.Get<Rooms>(aggregateId);
                var events = action(rooms).ToList();
                _aggregateRootRepository.Save(rooms, events);
                _messageBus.Publish(events);
            }
        }

        internal void Consume(ReserveRoom x)
        {
            throw new NotImplementedException();
        }
    }
}