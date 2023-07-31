using AutoMapper;
using Tienda.Modelo;
using Tienda.Modelo.DTO;

namespace Tienda.Mapping
{
    public class MappingConfig : Profile
    {

        public MappingConfig()
        {
            //Hacerlo uno por uno
            //Funte   //Destino
            CreateMap<Usuario, UsuarioDTO>();
            //Funte   //Destino
            CreateMap<UsuarioDTO, Usuario>();

            //Es lo mismo que esta arriba
            CreateMap<Usuario, CrearUsuarioDTO>().ReverseMap();
            CreateMap<Usuario, ActualizarUsuarioDTO>().ReverseMap();
        }

    }
}
