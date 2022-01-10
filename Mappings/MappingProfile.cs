using AutoMapper;
using WatersTicketingAPI.DTO;
using WatersTicketingAPI.DTO.Ticket;
using WatersTicketingAPI.DTO.User;
using WatersTicketingAPI.Models;

namespace WatersTicketingAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<User, UserEditDto>();
            CreateMap<User, UserLoginDto>();
            CreateMap<User, UserRegisterDto>();
            CreateMap<Ticket, MyTicketDto>();
            CreateMap<TicketEditDto, Ticket>();
            CreateMap<UserDto, User>();
            CreateMap<UserEditDto, User>();
            CreateMap<UserLoginDto, User>();
            CreateMap<UserRegisterDto, User>();
            CreateMap<TicketDto, Ticket>();
            CreateMap<Ticket, TicketDto>();
        }
    }
}
