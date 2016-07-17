using System;

namespace Ada.Hotel.Api.Models
{
    public class RoomType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int NoOfUnits { get; set; }
    }
}