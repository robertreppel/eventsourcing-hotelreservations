using System;
using System.Collections.Generic;
using System.Linq;
using Ada.Hotel.Reservations.Read.Models;

namespace Ada.Hotel.Reservations.Read.Services
{
    public class ReservationsRepository
    {
        public IEnumerable<VacantRooms> FindVacanciesFor(DateTime checkinDate, DateTime checkoutDate)
        {
            var conflictingReservations = _reservations.FindAll(existingReservation =>
                (existingReservation.CheckInDate >= checkinDate && existingReservation.CheckoutDate <= checkoutDate) ||
                (existingReservation.CheckInDate >= checkinDate && existingReservation.CheckInDate < checkoutDate) ||
                (existingReservation.CheckInDate <= checkinDate && checkinDate < existingReservation.CheckoutDate)
                ).ToList();

            var result = new List<VacantRooms>();
            foreach (var rooms in _roomTypes)
            {
                var roomTypeInfo = _roomTypes.FindLast(roomType => roomType.Id == rooms.Id);
                result.Add(new VacantRooms
                {
                    RoomTypeId = rooms.Id,
                    Description = rooms.Name,
                    NoOfUnitsAvailable = roomTypeInfo.TotalNoOfUnits - conflictingReservations.Count,
                    TotalNoOfUnits = roomTypeInfo.TotalNoOfUnits
                });
            }
            return result;
        }

        private static List<RoomType> _roomTypes;
        private static List<Reservation> _reservations;

        public void Save(RoomType roomType)
        {
            _roomTypes.Add(roomType);
        }

        public void Save(Reservation reservation)
        {
            _reservations.Add(reservation);
        }

        public ReservationsRepository()
        {
            _roomTypes = new List<RoomType>();
            _reservations = new List<Reservation>();
        }
    }
}