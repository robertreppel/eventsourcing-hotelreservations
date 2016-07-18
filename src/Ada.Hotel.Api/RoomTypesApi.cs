using System;
using Ada.Hotel.Api.Models;
using Ada.Hotel.Reservations;
using Ada.Hotel.Reservations.Commands;
using Cqrs.Services;
using Nancy;
using Nancy.ModelBinding;

namespace Ada.Hotel.Api
{
    public sealed class RoomTypesApi : NancyModule
    {
        private readonly Guid _hotelId = Guid.Parse("c2d0a994-e7a6-471b-ad03-ee344e074a80");
        public RoomTypesApi()
        {
            Get("/rooms/types", _ => GetRoomTypes());
            Post("/rooms/types", _ => CreateNewRoomType());
        }

        private Response CreateNewRoomType()
        {
            var newRoomType = this.Bind<RoomType>();
            DirectBus.Instance.Send(new CreateRooms(_hotelId, newRoomType.Id, newRoomType.Name, newRoomType.NoOfUnits));

            var newResourceUri = $"{Request.Path}/{newRoomType.Id}";
            var response = Response.AsJson(newResourceUri);
            response.ContentType = "application/json";
            response.StatusCode = HttpStatusCode.Accepted;
            return response;
        }

        private Response GetRoomTypes()
        {
            var result = Repository.RoomType.GetAll();
            var response = Response.AsJson(result);
            response.ContentType = "application/json";
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
    }
}