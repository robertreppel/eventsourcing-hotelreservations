using System;

namespace Ada.Hotel.Reservations.Commands
{
    public class CreateRooms
    {
        public readonly Guid HotelId;
        public readonly Guid RoomTypeId;
        public readonly string RoomType;
        public readonly int NoOfUnits;

        public CreateRooms(Guid hotelId, Guid roomTypeId, string roomType, int noOfUnits)
        {
            HotelId = hotelId;
            RoomTypeId = roomTypeId;
            RoomType = roomType;
            NoOfUnits = noOfUnits;
        }
    }
}