using ApiUsuariosRC.Entities.Dtos;
using ApiUsuariosRC.Entities.Models;
using AutoMapper;

namespace ApiUsuariosRC.Services.Mappings;

public class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
        CreateMap<Usuario, UsuarioDto>()
            .ForMember(dest => dest.Genero, opt => opt.MapFrom(src => src.Genero == null ? string.Empty : src.Genero.Nombre));

        CreateMap<UsuarioConsulta, UsuarioDto>();
        CreateMap<UsuarioConsulta, UsuarioConHistorialDto>();

        CreateMap<UsuarioCambioLog, UsuarioCambioLogDto>();

        CreateMap<Genero, GeneroDto>()
            .ForMember(dest => dest.Genero, opt => opt.MapFrom(src => src.Nombre));

        CreateMap<CrearUsuarioRequestDto, Usuario>()
            .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
            .ForMember(dest => dest.NumeroEmpleado, opt => opt.Ignore())
            .ForMember(dest => dest.Genero, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore());
    }
}
