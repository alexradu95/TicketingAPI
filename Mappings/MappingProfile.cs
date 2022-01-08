using AutoMapper;
using WatersTicketingAPI.DTO;
using WatersTicketingAPI.Models;

namespace WatersTicketingAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<User, UserEditDTO>();
            CreateMap<User, UserLoginDTO>();
            CreateMap<User, UserRegisterDTO>();
            CreateMap<Ticket, MyTicketDTO>();
            CreateMap<TicketEditDTO, Ticket>();
            CreateMap<UserDTO, User>();
            CreateMap<UserEditDTO, User>();
            CreateMap<UserLoginDTO, User>();
            CreateMap<UserRegisterDTO, User>();
            CreateMap<TicketDTO, Ticket>();
            CreateMap<Ticket, TicketDTO>();
        }
    }
}
