using System;

namespace Ada.Hotel.Api.Models
{
    public class Reservation
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckoutDate { get; set; }
        public Guid RoomTypeId { get; set; }
        public string GuestId { get; set; }
        public int NoOfUnits { get; set; }
        public Guid Id { get; set; }
    }
}
