using Ada.Hotel.Reservations.Commands;
using Ada.Hotel.Reservations.Read.Services;
using Cqrs;
using Cqrs.Services;

namespace Ada.Hotel.Reservations
{
    public class Bootstrap
    {
        public static void WireUp(IDispatcher dispatcher, IMessageBus messageBus)
        {
            WireUpRead(dispatcher);
            WireUpTransactional(dispatcher, messageBus);
        }

        private static void WireUpTransactional(IDispatcher dispatcher, IMessageBus messageBus)
        {
            var eventStore = new InMemoryEventStore();
            var aggregateRootRepository = new InMemoryAggregateRootRepository(eventStore);

            var roomsDomain = new RoomsDomain(aggregateRootRepository, messageBus);
            dispatcher.RegisterHandler<CreateRooms>(x => roomsDomain.Consume(x));
            dispatcher.RegisterHandler<ReserveRooms>(x => roomsDomain.Consume(x));
        }
        
        private static void WireUpRead(IDispatcher dispatcher)
        {
            Repository.RoomType = new RoomTypeRepository();
            var roomsDenormalizer = new RoomTypesDenormalizer(Repository.RoomType);
            dispatcher.RegisterHandler(roomsDenormalizer);

            Repository.Reservations = new ReservationsRepository();
            var availableRoomsDenormalizer = new VacantRoomsDenormalizer(Repository.Reservations);
            dispatcher.RegisterHandler(availableRoomsDenormalizer);
        }
    }
}