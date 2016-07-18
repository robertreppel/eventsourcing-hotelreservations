using System;
using System.Collections.Generic;
using System.Linq;
using Ada.Hotel.Reservations;
using Ada.Hotel.Reservations.Read.Models;
using Nancy;

namespace Ada.Hotel.Api
{
    public sealed class RoomVacanciesApi : NancyModule
    {
        public RoomVacanciesApi()
        {
            Get("/rooms/vacancies",  dates => GetAvailableRoomsFor(dates));
        }

        private Response GetAvailableRoomsFor(dynamic dates)
        {
            DateTime checkin = DateTime.Parse(Request.Query["checkin"]);
            DateTime checkout = DateTime.Parse(Request.Query["checkin"]);
            List<VacantRooms> result = Repository.Reservations.FindVacanciesFor(checkin, checkout).ToList();
            var response = Response.AsJson(result);
            response.ContentType = "application/json";
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
    }
}