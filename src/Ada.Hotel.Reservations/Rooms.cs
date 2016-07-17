using System;
using System.Collections.Generic;
using System.Linq;
using Ada.Hotel.Reservations.Events;
using Cqrs.Domain;

namespace Ada.Hotel.Reservations
{
    public class Rooms : AggregateRoot
    {
        private bool _created;
        private int _totalNumberOfRoomsOfThisType;
        private Guid _roomTypeId;
        private readonly List<RoomsReserved> _roomsReserved;

        public Rooms()
        {
            _roomsReserved = new List<RoomsReserved>();
            RegisterHandler<RoomsCreated>(OnRoomsCreated);
            RegisterHandler<RoomsReserved>(OnRoomsReserved);
        }

        public IEnumerable<object> Create(Guid hotelId, Guid roomTypeId, string typeName, int noOfUnits)
        {
            if (_created) throw new InvalidOperationException("Room type already created.");
            if (string.IsNullOrEmpty(typeName)) throw new InvalidOperationException("Room type must not be blank.");
            if(noOfUnits < 0) throw new InvalidOperationException("Must have zero or more rooms in inventory.");

            yield return Apply(new RoomsCreated(roomTypeId, typeName, noOfUnits, hotelId));
        }

        public IEnumerable<object> Reserve(string guestId, DateTime newReservationCheckInDate, DateTime newReservationCheckoutDate, int noOfUnits)
        {
            if (noOfUnits < 1) throw new InvalidOperationException("Must reserve at least one room.");
            if (newReservationCheckoutDate <= newReservationCheckInDate) throw new InvalidOperationException("Checkin date must be before checkout date.");
            if (string.IsNullOrEmpty(guestId)) throw new InvalidOperationException("GuestId cannot be blank.");
            if (noOfUnits > _totalNumberOfRoomsOfThisType) throw new InvalidOperationException("Not enough rooms available.");

            var conflictingReservations = _roomsReserved.FindAll(existingReservation =>
                    (existingReservation.CheckInDate >= newReservationCheckInDate && existingReservation.CheckoutDate <= newReservationCheckoutDate) ||
                    (existingReservation.CheckInDate >= newReservationCheckInDate && existingReservation.CheckInDate < newReservationCheckoutDate) ||
                    (existingReservation.CheckInDate <= newReservationCheckInDate && newReservationCheckInDate < existingReservation.CheckoutDate)
                ).ToList();

            var noOfRoomsReserved = CountNoOfRoomsReservedIn(conflictingReservations);

            if (noOfRoomsReserved >= _totalNumberOfRoomsOfThisType) throw new InvalidOperationException("No vacancy.");

            if(noOfRoomsReserved >= _totalNumberOfRoomsOfThisType) throw new InvalidOperationException("No vacancy.");

            yield return Apply(new RoomsReserved(newReservationCheckInDate, newReservationCheckoutDate, _roomTypeId, guestId, 1));
        }

        private static int CountNoOfRoomsReservedIn(List<RoomsReserved> conflictingReservations)
        {
            var noOfConflictingReservations = 0;
            foreach (var conflictingReservation in conflictingReservations)
            {
                noOfConflictingReservations = noOfConflictingReservations + conflictingReservation.NoOfUnits;
            }
            return noOfConflictingReservations;
        }

        private void OnRoomsCreated(RoomsCreated roomsCreated)
        {
            _created = true;
            _roomTypeId = roomsCreated.RoomTypeId;
            _totalNumberOfRoomsOfThisType = roomsCreated.NoOfUnits;
            AggregateId = roomsCreated.RoomTypeId;
        }

        private void OnRoomsReserved(RoomsReserved roomsReserved)
        {
            _roomsReserved.Add(roomsReserved);
        }
    }
}