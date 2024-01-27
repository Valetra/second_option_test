using AutoMapper;

namespace Mapper;

public class AppMappingProfile : Profile
{
	public AppMappingProfile()
	{
		CreateMap<RequestObjects.Event, DAL.Models.Event>();
		CreateMap<Services.ValuesAtMinute, ResponseObjects.ValuesAtMinute>();
	}
}
