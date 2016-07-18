using System;
using System.Collections.Concurrent;
using System.Linq;
using Ada.Hotel.Reservations.Events;
using Ada.Hotel.Reservations.Read.Models;
using Ada.Hotel.Reservations.Read.Services;
using NUnit.Framework;

namespace Ada.Hotel.Reservations.Tests.Read
{
    [TestFixture]
    public class RoomTypesQueryTests
    {
        private RoomTypesDenormalizer _sut;
        private RoomTypeRepository _availableRoomsRepository;

        [SetUp]
        public void SetUp()
        {
            _availableRoomsRepository = new RoomTypeRepository(new ConcurrentDictionary<object, RoomType>());
            _sut = new RoomTypesDenormalizer(_availableRoomsRepository);
        }

        [Test]
        public void WhenRoomTypeCreated_ThenAvailableRoomsListContainsIt()
        {
            //Given
            //When
            var roomsCreated = new RoomsCreated(Guid.NewGuid(), "Extra Large Suite", 3, Guid.NewGuid());
            _sut.Handle(roomsCreated);
            //Then
            var availableRooms = _availableRoomsRepository.GetAll().ToArray();
            Assert.That(availableRooms, Has.Length.EqualTo(1));
            var createdRoomType = availableRooms[0];
            Assert.That(createdRoomType.Name, Is.EqualTo("Extra Large Suite"));
            Assert.That(createdRoomType.TotalNoOfUnits, Is.EqualTo(3));
        }
    }
}