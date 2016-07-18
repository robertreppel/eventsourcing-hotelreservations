using System;
using Ada.Hotel.Api.Models;
using Ada.Hotel.Reservations.Commands;
using Cqrs.Services;
using Nancy;
using Nancy.ModelBinding;

namespace Ada.Hotel.Api
{
    public sealed class RoomReservationsApi : NancyModule
    {
        public RoomReservationsApi()
        {
            Post("/rooms/reservations",  _ => ReserveRoomFor());
        }

        private Response ReserveRoomFor()
        {
            var reservation = this.Bind<Reservation>();
            reservation.Id = Guid.NewGuid();

            DirectBus.Instance.Send(new ReserveRooms(reservation.Id, reservation.CheckInDate, reservation.CheckoutDate, reservation.RoomTypeId, reservation.GuestId, reservation.NoOfUnits));

            var result = "";
            var response = Response.AsJson(result);
            response.ContentType = "application/json";
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
    }
}