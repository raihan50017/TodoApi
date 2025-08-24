using AutoMapper;
using TodoApi.Dtos;
using TodoApi.Entities;

namespace TodoApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TodoItem, TodoReadDto>();
            CreateMap<TodoCreateDto, TodoItem>();
            CreateMap<TodoUpdateDto, TodoItem>();
        }
    }
}
