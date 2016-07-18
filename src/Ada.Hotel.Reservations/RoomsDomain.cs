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

        private void ConsumeInternal(Guid aggregateId, Func<RoomTypes, IEnumerable<object>> action)
        {
            using (new Mutex(true, aggregateId.ToString()))
            {
                var rooms = _aggregateRootRepository.Get<RoomTypes>(aggregateId);
                var events = action(rooms).ToList();
                _aggregateRootRepository.Save(rooms, events);
                _messageBus.Publish(events);
            }
        }

        internal void Consume(ReserveRooms reserveRooms)
        {
            using (new Mutex(true, reserveRooms.ReservationId.ToString()))
            {
                var rooms = _aggregateRootRepository.Get<RoomTypes>(reserveRooms.RoomTypeId);
                var events = rooms.Reserve(reserveRooms.ReservationId, reserveRooms.GuestId, reserveRooms.RoomTypeId, reserveRooms.CheckInDate, reserveRooms.CheckoutDate,
                    reserveRooms.NoOfUnits).ToList();
                _aggregateRootRepository.Save(rooms, events);
                _messageBus.Publish(events);
            }
        }
    }
}