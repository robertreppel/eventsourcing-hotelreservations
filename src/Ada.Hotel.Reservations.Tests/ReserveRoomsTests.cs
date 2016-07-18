using System;
using System.Linq;
using Ada.Hotel.Reservations.Events;
using NUnit.Framework;

namespace Ada.Hotel.Reservations.Tests
{
    [TestFixture]
    public class ReserveRoomsTests
    {
        [Test]
        public void Must_reserve_at_least_one_room()
        {
            //Given
            var rooms = new RoomTypes();
            rooms.Hydrate(new RoomsCreated(Guid.NewGuid(), "RoomTypeId", 1, Guid.NewGuid()));
            //When
            object[] events = null;
            TestDelegate when = () => events = rooms.Reserve(Guid.NewGuid(), "guest1", Guid.NewGuid(), new DateTime(2016,03, 1), new DateTime(2016, 03, 2), 0).ToArray();
            //Then
            var ex = Assert.Throws<InvalidOperationException>(when);
            Assert.That(events, Is.Null);
            Assert.That(ex.Message, Is.EqualTo("Must reserve at least one room."));
        }

        [Test]
        public void Checkout_cant_be_before_checkin()
        {
            //Given
            var rooms = new RoomTypes();
            rooms.Hydrate(new RoomsCreated(Guid.NewGuid(), "Single Room", 1, Guid.NewGuid()));
            //When
            object[] events = null;
            TestDelegate when = () => events = rooms.Reserve(Guid.NewGuid(), "guest1", Guid.NewGuid(), new DateTime(2016, 03, 3), new DateTime(2016, 03, 2), 1).ToArray();
            //Then
            var ex = Assert.Throws<InvalidOperationException>(when);
            Assert.That(events, Is.Null);
            Assert.That(ex.Message, Is.EqualTo("Checkin date must be before checkout date."));
        }

        [Test]
        public void Checking_out_on_the_same_day_as_checking_in_is_not_OK()
        {
            //Given
            var rooms = new RoomTypes();
            rooms.Hydrate(new RoomsCreated(Guid.NewGuid(), "Double Room", 1, Guid.NewGuid()));
            //When
            object[] events = null;
            TestDelegate when = () => events = rooms.Reserve(Guid.NewGuid(), "guest1", Guid.NewGuid(), new DateTime(2016, 03, 2), new DateTime(2016, 03, 2), 1).ToArray();
            //Then
            var ex = Assert.Throws<InvalidOperationException>(when);
            Assert.That(events, Is.Null);
            Assert.That(ex.Message, Is.EqualTo("Checkin date must be before checkout date."));
        }

        [Test]
        public void Reserving_without_guestid_fails()
        {
            //Given
            var rooms = new RoomTypes();
            var roomTypeId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            rooms.Hydrate(new RoomsCreated(roomTypeId, "Double Room", 5, hotelId));
            //When
            object[] events = null;
            TestDelegate when = () => events = rooms.Reserve(Guid.NewGuid(), "", Guid.NewGuid(), new DateTime(2016, 04, 2), new DateTime(2016, 04, 7), 1).ToArray();
            //Then
            var ex = Assert.Throws<InvalidOperationException>(when);
            Assert.That(events, Is.Null);
            Assert.That(ex.Message, Is.EqualTo("GuestId cannot be blank."));
        }

        [Test]
        public void Reserving_a_room_type_for_which_there_are_no_other_reservations_is_OK()
        {
            //Given
            var rooms = new RoomTypes();
            var roomTypeId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            rooms.Hydrate(new RoomsCreated(roomTypeId, "Double Room", 5, hotelId));
            //When
            var checkInDate = new DateTime(2016, 04, 2);
            var checkOutDate = new DateTime(2016, 04, 7);
            var events = rooms.Reserve(Guid.NewGuid(), "guest 1", Guid.NewGuid(), checkInDate, checkOutDate, 1).ToArray();
            //Then
            Assert.AreEqual(1, events.Length);
            var roomsReserved = events[0] as RoomsReserved;
            Assert.That(roomsReserved, Is.TypeOf<RoomsReserved>());
            // ReSharper disable once PossibleNullReferenceException
            Assert.AreEqual("guest 1", roomsReserved.GuestId);
            Assert.AreEqual(roomTypeId, roomsReserved.RoomTypeId);
            Assert.AreEqual(checkInDate, roomsReserved.CheckInDate);
            Assert.AreEqual(checkOutDate, roomsReserved.CheckoutDate);
        }

        [Test]
        public void Reserving_more_rooms_than_exist_for_a_type_of_room_is_not_OK()
        {
            //Given
            var rooms = new RoomTypes();
            var roomTypeId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            rooms.Hydrate(new RoomsCreated(roomTypeId, "Double Room", 2, hotelId));
            //When
            var checkInDate = new DateTime(2016, 04, 2);
            var checkOutDate = new DateTime(2016, 04, 7);
            object[] events = null;
            TestDelegate when = () => events = rooms.Reserve(Guid.NewGuid(), "guest 1", Guid.NewGuid(), checkInDate, checkOutDate, 3).ToArray();
            //Then
            var ex = Assert.Throws<InvalidOperationException>(when);
            Assert.That(events, Is.Null);
            Assert.That(ex.Message, Is.EqualTo("Not enough rooms available."));
        }

        [Test]
        public void Making_a_reservation_which_starts_after_checkout_of_a_previous_reservation_is_OK()
        {
            //Given
            var rooms = new RoomTypes();
            var roomTypeId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var reservationId = Guid.NewGuid();
            rooms.Hydrate(new RoomsCreated(roomTypeId, "Double Room", 5, hotelId));
            var previousReservationCheckoutDate = new DateTime(2016,5,2);
            var previousReservationCheckinDate = new DateTime(2016, 5,1);
            rooms.Hydrate(new RoomsReserved(reservationId, previousReservationCheckinDate, previousReservationCheckoutDate, roomTypeId, "guest 1", 1));
            //When
            var checkInDate = previousReservationCheckoutDate;
            var checkOutDate = new DateTime(2016, 5, 3);
            var events = rooms.Reserve(reservationId, "guest 2", Guid.NewGuid(), checkInDate, checkOutDate, 1).ToArray();
            //Then
            Assert.AreEqual(1, events.Length);
            var roomsReserved = events[0] as RoomsReserved;
            Assert.That(roomsReserved, Is.TypeOf<RoomsReserved>());
            // ReSharper disable once PossibleNullReferenceException
            Assert.AreEqual(roomTypeId, roomsReserved.RoomTypeId);
            Assert.AreEqual(reservationId, roomsReserved.ReservationId);
            Assert.AreEqual("guest 2", roomsReserved.GuestId);
            Assert.AreEqual(roomTypeId, roomsReserved.RoomTypeId);
            Assert.AreEqual(checkInDate, roomsReserved.CheckInDate);
            Assert.AreEqual(checkOutDate, roomsReserved.CheckoutDate);
            Assert.AreEqual(1, roomsReserved.NoOfUnits);
        }

        [Test]
        public void OKMaking_a_reservation_which_ends_before_a_subsequent_reservation_is_OK()
        {
            //Given
            var rooms = new RoomTypes();
            var roomTypeId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var reservationId = Guid.NewGuid();
            rooms.Hydrate(new RoomsCreated(roomTypeId, "Double Room", 1, hotelId));
            var laterReservationCheckinDate = new DateTime(2016, 5, 3);
            var laterReservationCheckoutDate = new DateTime(2016, 5, 4);
            rooms.Hydrate(new RoomsReserved(reservationId, laterReservationCheckinDate, laterReservationCheckoutDate, roomTypeId, "guest 1", 1));
            //When
            var checkInDate = new DateTime(2016, 5, 2);
            var checkOutDate = laterReservationCheckinDate;
            var events = rooms.Reserve(reservationId, "guest 2", Guid.NewGuid(), checkInDate, checkOutDate, 1).ToArray();
            //Then
            Assert.AreEqual(1, events.Length);
            var roomsReserved = events[0] as RoomsReserved;
            Assert.That(roomsReserved, Is.TypeOf<RoomsReserved>());
            // ReSharper disable once PossibleNullReferenceException
            Assert.AreEqual("guest 2", roomsReserved.GuestId);
            Assert.AreEqual(roomTypeId, roomsReserved.RoomTypeId);
            Assert.AreEqual(checkInDate, roomsReserved.CheckInDate);
            Assert.AreEqual(checkOutDate, roomsReserved.CheckoutDate);
            Assert.AreEqual(1, roomsReserved.NoOfUnits);
        }

        [Test]
        public void New_reservation_with_same_checkin_and_checkout_dates_as_existing_one_is_not_OK()
        {
            //Given
            var rooms = new RoomTypes();
            var roomTypeId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var noOfUnits = 1;
            var reservationId = Guid.NewGuid();

            rooms.Hydrate(new RoomsCreated(roomTypeId, "Double Room", noOfUnits, hotelId));
            rooms.Hydrate(new RoomsReserved(reservationId, new DateTime(2016, 04, 02), new DateTime(2016, 04, 03), roomTypeId, "guest 1", noOfUnits));
            //When
            var checkInDate = new DateTime(2016, 04, 2);
            var checkOutDate = new DateTime(2016, 04, 3);
            object[] events = null;
            TestDelegate when = () => events = rooms.Reserve(reservationId, "guest 2", roomTypeId, checkInDate, checkOutDate, noOfUnits).ToArray();
            //Then
            var ex = Assert.Throws<InvalidOperationException>(when);
            Assert.That(events, Is.Null);
            Assert.That(ex.Message, Is.EqualTo("No vacancy."));
        }

        [Test]
        public void New_reservation_with_checkin_and_checkout_dates_within_existing_reservation_is_not_OK()
        {
            //Given
            var rooms = new RoomTypes();
            var roomTypeId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var noOfUnits = 1;
            var reservationId = Guid.NewGuid();
            rooms.Hydrate(new RoomsCreated(roomTypeId, "Double Room", noOfUnits, hotelId));
            rooms.Hydrate(new RoomsReserved(reservationId, new DateTime(2016, 03, 30), new DateTime(2016, 04, 03), roomTypeId, "guest 1", noOfUnits));
            //When
            var checkInDate = new DateTime(2016, 04, 2);
            var checkOutDate = new DateTime(2016, 04, 03);
            object[] events = null;
            TestDelegate when = () => events = rooms.Reserve(reservationId, "guest 2", roomTypeId, checkInDate, checkOutDate, noOfUnits).ToArray();
            //Then
            var ex = Assert.Throws<InvalidOperationException>(when);
            Assert.That(events, Is.Null);
            Assert.That(ex.Message, Is.EqualTo("No vacancy."));
        }

        [Test]
        public void New_reservation_with_checkout_during_existing_reservation_is_not_OK()
        {
            //Given
            var rooms = new RoomTypes();
            var roomTypeId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var noOfUnits = 1;
            var reservationId = Guid.NewGuid();
            rooms.Hydrate(new RoomsCreated(roomTypeId, "Double Room", noOfUnits, hotelId));
            rooms.Hydrate(new RoomsReserved(reservationId, new DateTime(2016, 04, 20), new DateTime(2016, 04, 22), roomTypeId, "guest 1", noOfUnits));
            //When
            var checkInDate = new DateTime(2016, 04, 01);
            var checkOutDate = new DateTime(2016, 04, 21);
            object[] events = null;
            TestDelegate when = () => events = rooms.Reserve(reservationId, "guest 2", roomTypeId, checkInDate, checkOutDate, noOfUnits).ToArray();
            //Then
            var ex = Assert.Throws<InvalidOperationException>(when);
            Assert.That(events, Is.Null);
            Assert.That(ex.Message, Is.EqualTo("No vacancy."));
        }

        [Test]
        public void New_reservation_where_checkin_is_during_a_previous_reservation_is_not_OK()
        {
            //Given
            var rooms = new RoomTypes();
            var roomTypeId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var noOfUnits = 1;
            var reservationId = Guid.NewGuid();
            rooms.Hydrate(new RoomsCreated(roomTypeId, "Double Room", noOfUnits, hotelId));
            rooms.Hydrate(new RoomsReserved(reservationId, new DateTime(2016, 04, 20), new DateTime(2016, 04, 22), roomTypeId, "guest 1", noOfUnits));
            //When
            var checkInDate = new DateTime(2016, 04, 21);
            var checkOutDate = new DateTime(2016, 04, 30);
            object[] events = null;
            TestDelegate when = () => events = rooms.Reserve(reservationId, "guest 2", roomTypeId, checkInDate, checkOutDate, noOfUnits).ToArray();
            //Then
            var ex = Assert.Throws<InvalidOperationException>(when);
            Assert.That(events, Is.Null);
            Assert.That(ex.Message, Is.EqualTo("No vacancy."));
        }

        [Test]
        public void Checking_in_same_day_as_previous_reservation_checkout_is_OK()
        {
            //Given
            var rooms = new RoomTypes();
            var roomTypeId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var reservationId = Guid.NewGuid();
            var noOfUnits = 1;
            rooms.Hydrate(new RoomsCreated(roomTypeId, "Double Room", noOfUnits, hotelId));
            rooms.Hydrate(new RoomsReserved(reservationId, new DateTime(2016, 04, 20), new DateTime(2016, 04, 22), roomTypeId, "guest 1", noOfUnits));
            //When
            var checkInDate = new DateTime(2016, 04, 22);
            var checkOutDate = new DateTime(2016, 04, 23);
            var events = rooms.Reserve(reservationId, "guest 2", Guid.NewGuid(), checkInDate, checkOutDate, noOfUnits).ToArray();
            var roomsReserved = events[0];
            //Then
            Assert.That(roomsReserved, Is.TypeOf<RoomsReserved>());
        }

        [Test]
        public void New_reservation_with_checkin_before_and_checkout_after_an_existing_one_is_not_OK()
        {
            //Given
            var rooms = new RoomTypes();
            var roomTypeId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var noOfUnits = 1;
            var reservationId = Guid.NewGuid();
            rooms.Hydrate(new RoomsCreated(roomTypeId, "Double Room", noOfUnits, hotelId));
            rooms.Hydrate(new RoomsReserved(reservationId, new DateTime(2016, 04, 20), new DateTime(2016, 04, 22), roomTypeId, "guest 1", noOfUnits));
            //When
            var checkInDate = new DateTime(2016, 04, 19);
            var checkOutDate = new DateTime(2016, 04, 23);
            object[] events = null;
            TestDelegate when = () =>
            {
                events = rooms.Reserve(reservationId, "guest 2", roomTypeId, checkInDate, checkOutDate, noOfUnits).ToArray();
            };
            //Then
            var ex = Assert.Throws<InvalidOperationException>(when);
            Assert.That(events, Is.Null);
            Assert.That(ex.Message, Is.EqualTo("No vacancy."));
        }

        [Test]
        public void Multiple_conflicting_reservations_are_handled_correctly()
        {
            //Given
            var rooms = new RoomTypes();
            var roomTypeId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var reservationId = Guid.NewGuid();
            var noOfUnits = 1;
            rooms.Hydrate(new RoomsCreated(roomTypeId, "Double Room", noOfUnits, hotelId));
            rooms.Hydrate(new RoomsReserved(reservationId, new DateTime(2016, 04, 20), new DateTime(2016, 04, 22), roomTypeId, "guest 1", noOfUnits));
            rooms.Hydrate(new RoomsReserved(reservationId, new DateTime(2016, 04, 20), new DateTime(2016, 04, 22), roomTypeId, "guest 3", noOfUnits));
            //When
            var checkInDate = new DateTime(2016, 04, 19);
            var checkOutDate = new DateTime(2016, 04, 23);
            object[] events = null;
            TestDelegate when = () =>
            {
                events = rooms.Reserve(reservationId, "guest 2", roomTypeId, checkInDate, checkOutDate, noOfUnits).ToArray();
            };
            //Then
            var ex = Assert.Throws<InvalidOperationException>(when);
            Assert.That(events, Is.Null);
            Assert.That(ex.Message, Is.EqualTo("No vacancy."));
        }

        [Test]
        public void New_reservations_when_all_existing_rooms_are_full_is_not_OK()
        {
            //Given
            var rooms = new RoomTypes();
            var roomTypeId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var reservationId = Guid.NewGuid();
            var totalNumberOfDoubleRoomsInThisHotel = 2;
            rooms.Hydrate(new RoomsCreated(roomTypeId, "Double Room", totalNumberOfDoubleRoomsInThisHotel, hotelId));
            rooms.Hydrate(new RoomsReserved(reservationId, new DateTime(2016, 04, 20), new DateTime(2016, 04, 22), roomTypeId, "guest 1", totalNumberOfDoubleRoomsInThisHotel));
            //When
            var checkInDate = new DateTime(2016, 04, 19);
            var checkOutDate = new DateTime(2016, 04, 23);
            object[] events = null;
            TestDelegate when = () => events = rooms.Reserve(reservationId, "guest 2", roomTypeId, checkInDate, checkOutDate, totalNumberOfDoubleRoomsInThisHotel).ToArray();
            //Then
            var ex = Assert.Throws<InvalidOperationException>(when);
            Assert.That(events, Is.Null);
            Assert.That(ex.Message, Is.EqualTo("No vacancy."));
        }

        [Test]
        public void New_reservations_when_there_are_less_existing_reservations_than_there_are_rooms_of_this_type_are_OK()
        {
            //Given
            var rooms = new RoomTypes();
            var roomTypeId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var reservationId = Guid.NewGuid();
            var totalNumberOfDoubleRoomsInThisHotel = 3;
            rooms.Hydrate(new RoomsCreated(roomTypeId, "Double Room", totalNumberOfDoubleRoomsInThisHotel, hotelId));
            rooms.Hydrate(new RoomsReserved(reservationId, new DateTime(2016, 04, 20), new DateTime(2016, 04, 22), roomTypeId, "guest 1", 1));
            rooms.Hydrate(new RoomsReserved(reservationId, new DateTime(2016, 04, 20), new DateTime(2016, 04, 22), roomTypeId, "guest 3", 1));
            //When
            var checkInDate = new DateTime(2016, 04, 19);
            var checkOutDate = new DateTime(2016, 04, 23);
            var events = rooms.Reserve(reservationId, "guest 2", Guid.NewGuid(), checkInDate, checkOutDate, totalNumberOfDoubleRoomsInThisHotel).ToArray();
            var roomsReserved = events[0];
            //Then
            Assert.That(roomsReserved, Is.TypeOf<RoomsReserved>());
        }
    }
}