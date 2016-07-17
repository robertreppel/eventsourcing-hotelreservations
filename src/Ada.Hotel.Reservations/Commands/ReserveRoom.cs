using System;

namespace Ada.Hotel.Reservations.Commands
{
    class ReserveRoom
    {
        public readonly DateTime CheckInDate;
        public readonly DateTime CheckoutDate;
        public readonly string RoomType;
        public readonly string GuestId;

        public ReserveRoom(DateTime checkInDate, DateTime checkoutDate, string roomType, string guestId)
        {
            GuestId = guestId;
            RoomType = roomType;
            CheckoutDate = checkoutDate;
            CheckInDate = checkInDate;
        }
    }
}
