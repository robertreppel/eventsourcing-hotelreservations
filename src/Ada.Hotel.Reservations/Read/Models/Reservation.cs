using System;

namespace Ada.Hotel.Reservations.Read.Models
{
    public class Reservation
    {
        public Guid RoomTypeId { get; set; }
        public string GuestId { get; set; }
        public int NoOfUnits { get; set; }
        public DateTime CheckInDate { get; set;  }
        public DateTime CheckoutDate { get; set; }
        public Guid Id { get; set; }
    }
}