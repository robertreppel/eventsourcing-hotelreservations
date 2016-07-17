using System;

namespace Ada.Hotel.Reservations.Events
{
    public class RoomsCreated
    {
        public readonly int NoOfUnits;
        public readonly Guid RoomTypeId;
        public readonly string RoomType;
        public readonly Guid HotelId;

        public RoomsCreated(Guid roomTypeId, string roomType, int noOfUnits, Guid hotelId)
        {
            NoOfUnits = noOfUnits;
            RoomTypeId = roomTypeId;
            RoomType = roomType;
            HotelId = hotelId;
        }
    }
}