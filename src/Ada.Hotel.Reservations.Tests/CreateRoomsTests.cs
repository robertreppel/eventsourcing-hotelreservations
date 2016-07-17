using System;
using System.Linq;
using Ada.Hotel.Reservations.Events;
using NUnit.Framework;

namespace Ada.Hotel.Reservations.Tests
{
    [TestFixture]
    public class CreateRoomsTests
    {
        [Test]
        public void WhenCreate_ThenRoomsCreated()
        {
            //Given
            var rooms = new Rooms();
            //When
            var roomCategoryId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            const string roomTypeName = "Double";
            var events = rooms.Create(hotelId, roomCategoryId, roomTypeName, 1).ToArray();
            //Then
            Assert.That(events, Has.Length.EqualTo(1));
            Assert.That(events[0], Is.TypeOf<RoomsCreated>());
            var roomsCreated = (RoomsCreated)events[0];
            Assert.That(roomsCreated.RoomTypeId, Is.EqualTo(roomCategoryId));
            Assert.That(roomsCreated.HotelId, Is.EqualTo(hotelId));
            Assert.That(roomsCreated.RoomType, Is.EqualTo(roomTypeName));
            Assert.That(roomsCreated.NoOfUnits, Is.EqualTo(1));
        }

        [Test]
        public void GivenRoomCreated_WhenAlreadyExists_ThenInvalidOperationException()
        {
            //Given
            var rooms = new Rooms();
            rooms.Hydrate(new RoomsCreated(Guid.NewGuid(), "Id", 1, Guid.NewGuid()));
            //When
            object[] events = null;
            TestDelegate when = () => events = rooms.Create(Guid.NewGuid(), Guid.NewGuid(), "Id", 1).ToArray();
            //Then
            var ex = Assert.Throws<InvalidOperationException>(when);
            Assert.That(events, Is.Null);
            Assert.That(ex.Message, Is.EqualTo("Room type already created."));
        }

        [Test]
        public void GivenRoomCreated_WhenRoomTypeBlank_ThenInvalidOperationException()
        {
            //Given
            var rooms = new Rooms();
            //When
            object[] events = null;
            TestDelegate when = () => events = rooms.Create(Guid.NewGuid(), Guid.NewGuid(), "", 1).ToArray();
            //Then
            var ex = Assert.Throws<InvalidOperationException>(when);
            Assert.That(events, Is.Null);
            Assert.That(ex.Message, Is.EqualTo("Room type must not be blank."));
        }
        [Test]
        public void GivenRoomCreated_WhenLessThanZeroUnits_ThenInvalidOperationException()
        {
            //Given
            var rooms = new Rooms();
            //When
            object[] events = null;
            TestDelegate when = () => events = rooms.Create(Guid.NewGuid(), Guid.NewGuid(), "Single", -1).ToArray();
            //Then
            var ex = Assert.Throws<InvalidOperationException>(when);
            Assert.That(events, Is.Null);
            Assert.That(ex.Message, Is.EqualTo("Must have zero or more rooms in inventory."));
        }

    }
}