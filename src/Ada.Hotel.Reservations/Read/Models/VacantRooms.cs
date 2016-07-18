using System;

namespace Ada.Hotel.Reservations.Read.Models
{
    public class VacantRooms
    {
        public Guid RoomTypeId { get; set; }
        public string Description { get; set; }
        public int NoOfUnitsAvailable { get; set; }
        public int TotalNoOfUnits { get; set; }
    }
}