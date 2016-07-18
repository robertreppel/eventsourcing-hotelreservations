using Ada.Hotel.Reservations.Events;
using Ada.Hotel.Reservations.Read.Models;
using Cqrs;

namespace Ada.Hotel.Reservations.Read.Services
{
    public class VacantRoomsDenormalizer : IEventHandler
    {
        private readonly ReservationsRepository _reservationsRepository;

        public VacantRoomsDenormalizer(ReservationsRepository reservationsRepository)
        {
            _reservationsRepository = reservationsRepository;
        }

        public void Handle(object @event)
        {
            if (@event is RoomsCreated) OnRoomsCreated((RoomsCreated)@event);
            if (@event is RoomsReserved) OnRoomsReserved((RoomsReserved)@event);
        }

        private void OnRoomsReserved(RoomsReserved @event)
        {
            var reservation = new Reservation
            {
                Id = @event.ReservationId,
                RoomTypeId = @event.RoomTypeId,
                GuestId = @event.GuestId,
                NoOfUnits = @event.NoOfUnits,
                CheckInDate = @event.CheckInDate,
                CheckoutDate = @event.CheckoutDate
            };
            _reservationsRepository.Save(reservation);
        }

        private void OnRoomsCreated(RoomsCreated roomsCreated)
        {
            var roomType = new RoomType
            {
                Id = roomsCreated.RoomTypeId,
                Name = roomsCreated.RoomType,
                TotalNoOfUnits = roomsCreated.NoOfUnits
            };
            _reservationsRepository.Save(roomType);
        }

    }
}