using System.Collections.Concurrent;
using Ada.Hotel.Reservations.Read.Models;
using Cqrs.Services;

namespace Ada.Hotel.Reservations.Read.Services
{
    public class RoomTypeRepository : InMemoryReadModelRepository<RoomType>
    {
        private static readonly ConcurrentDictionary<object, RoomType> RoomTypes = new ConcurrentDictionary<object, RoomType>();

        public RoomTypeRepository(ConcurrentDictionary<object, RoomType> store) : base(store, x => x.Id)
        {
        }

        public RoomTypeRepository() : this(RoomTypes)
        {
        }
    }
}