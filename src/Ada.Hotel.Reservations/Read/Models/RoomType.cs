using System;

namespace Ada.Hotel.Reservations.Read.Models
{
    public class RoomType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int TotalNoOfUnits { get; set; }
    }
}