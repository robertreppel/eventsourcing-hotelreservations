using Ada.Hotel.Reservations.Events;
using Ada.Hotel.Reservations.Read.Models;
using Cqrs;

namespace Ada.Hotel.Reservations.Read.Services
{
    public class RoomTypesDenormalizer : IEventHandler
    {
        private readonly IReadModelRepository<RoomType> _roomTypesRepository;

        public RoomTypesDenormalizer(IReadModelRepository<RoomType> roomTypesRepository)
        {
            _roomTypesRepository = roomTypesRepository;
        }

        public void Handle(object @event)
        {
            if (@event is RoomsCreated) OnRoomsCreated((RoomsCreated)@event);
        }

        private void OnRoomsCreated(RoomsCreated roomsCreated)
        {
            var roomType = new RoomType
            {
                Id = roomsCreated.RoomTypeId,
                Name = roomsCreated.RoomType,
                TotalNoOfUnits = roomsCreated.NoOfUnits
            };
            _roomTypesRepository.Save(roomType);
        }

    }
}