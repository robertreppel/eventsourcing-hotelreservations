using System;

namespace Ada.Hotel.Reservations.Commands
{
    public class ReserveRooms
    {
        public readonly Guid ReservationId;
        public readonly int NoOfUnits;
        public readonly DateTime CheckInDate;
        public readonly DateTime CheckoutDate;
        public readonly Guid RoomTypeId;
        public readonly string GuestId;

        public ReserveRooms(Guid reservationId, DateTime checkInDate, DateTime checkoutDate, Guid roomTypeId, string guestId, int noOfUnits)
        {
            ReservationId = reservationId;
            NoOfUnits = noOfUnits;
            GuestId = guestId;
            RoomTypeId = roomTypeId;
            CheckoutDate = checkoutDate;
            CheckInDate = checkInDate;
        }
    }
}
