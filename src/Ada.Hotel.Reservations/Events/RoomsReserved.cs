using System;

namespace Ada.Hotel.Reservations.Events
{
    public class RoomsReserved
    {
        public readonly DateTime CheckInDate;
        public readonly DateTime CheckoutDate;
        public readonly Guid RoomTypeId;
        public readonly string GuestId;
        public readonly int NoOfUnits;

        public RoomsReserved(DateTime checkInDate, DateTime checkoutDate, Guid roomTypeIdId, string guestId, int noOfUnits)
        {
            GuestId = guestId;
            RoomTypeId = roomTypeIdId;
            CheckoutDate = checkoutDate;
            CheckInDate = checkInDate;
            NoOfUnits = noOfUnits;
        }
    }
}
