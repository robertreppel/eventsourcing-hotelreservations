using System;
using System.Linq;
using Ada.Hotel.Reservations.Events;
using Ada.Hotel.Reservations.Read.Services;
using NUnit.Framework;

namespace Ada.Hotel.Reservations.Tests.Read
{
    [TestFixture]
    public class AvailableRoomsQueryTests
    {
        private VacantRoomsDenormalizer _sut;
        private ReservationsRepository _reservationsRepository;

        [SetUp]
        public void SetUp()
        {
            _reservationsRepository = new ReservationsRepository();
            _sut = new VacantRoomsDenormalizer(_reservationsRepository);
        }

        [Test]
        public void When_no_rooms_exist_then_no_rooms_are_available()
        {
            //Given
            //Then
            var availableRooms = _reservationsRepository.FindVacanciesFor(new DateTime(2016, 7, 1), new DateTime(2016, 7, 2)).ToArray();
            Assert.That(availableRooms, Has.Length.EqualTo(0));
        }


        [Test]
        public void When_roomtype_created_and_there_are_no_reservations_then_roomtype_is_available()
        {
            //Given
            var roomTypeId = Guid.NewGuid();
            var roomsCreated = new RoomsCreated(roomTypeId, "Extra Large Suite", 3, Guid.NewGuid());
            _sut.Handle(roomsCreated);
            //Then
            var availableRooms = _reservationsRepository.FindVacanciesFor(new DateTime(2016, 7, 1), new DateTime(2016, 7, 2)).ToArray();
            Assert.That(availableRooms, Has.Length.EqualTo(1));
            var availableRoomType = availableRooms[0];
            Assert.That(availableRoomType.RoomTypeId, Is.EqualTo(roomTypeId));
            Assert.That(availableRoomType.Description, Is.EqualTo("Extra Large Suite"));
            Assert.That(availableRoomType.TotalNoOfUnits, Is.EqualTo(3));
            Assert.That(availableRoomType.NoOfUnitsAvailable, Is.EqualTo(3));
        }

        [Test]
        public void Request_with_checkin_and_checkout_dates_within_existing_reservation_reduces_no_of_available_rooms()
        {
            //Given
            var roomTypeId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var noOfUnits = 2;
            _sut.Handle(new RoomsCreated(roomTypeId, "Double Room", noOfUnits, hotelId));
            var reservationId = Guid.NewGuid();
            _sut.Handle(new RoomsReserved(reservationId, new DateTime(2016, 03, 30), new DateTime(2016, 04, 03), roomTypeId, "guest 1", 1));
            //When
            var checkInDate = new DateTime(2016, 04, 2);
            var checkOutDate = new DateTime(2016, 04, 03);
            var availableRooms = _reservationsRepository.FindVacanciesFor(checkInDate, checkOutDate).ToArray();
            Assert.That(availableRooms, Has.Length.EqualTo(1));
            var availableRoomType = availableRooms[0];
            Assert.That(availableRoomType.RoomTypeId, Is.EqualTo(roomTypeId));
            Assert.That(availableRoomType.Description, Is.EqualTo("Double Room"));
            Assert.That(availableRoomType.TotalNoOfUnits, Is.EqualTo(noOfUnits));
            Assert.That(availableRoomType.NoOfUnitsAvailable, Is.EqualTo(1));
        }
    }
}