using System;
using System.Linq;
using Ada.Hotel.Reservations;
using Nancy;

namespace Ada.Hotel.Api
{
    public sealed class RoomVacanciesApi : NancyModule
    {
        public RoomVacanciesApi()
        {
            Get("/rooms/vacancies",  _ => GetAvailableRoomsFor());
        }

        private Response GetAvailableRoomsFor()
        {
            DateTime checkin = DateTime.Parse(Request.Query["checkin"]);
            DateTime checkout = DateTime.Parse(Request.Query["checkout"]);
            var result = Repository.Reservations.FindVacanciesFor(checkin, checkout).ToList();
            var response = Response.AsJson(result);
            response.ContentType = "application/json";
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
    }
}