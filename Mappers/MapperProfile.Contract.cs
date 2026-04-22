using AutoMapper;
using SupplySync.DTOs.Contract;
using SupplySync.Models;
using System.Linq;

namespace SupplySync.Mappers
{
    public partial class MapperProfile
    {
        private void ConfigureContractMappings()
        {
            CreateMap<CreateContractRequestDto, Contract>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.ContractTerms, opt => opt.Ignore()) // handled below
                ;

            CreateMap<CreateContractTermRequestDto, ContractTerm>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Contract, ContractResponseDto>()
                .ForMember(dst => dst.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<ContractTerm, ContractTermResponseDto>();

            CreateMap<Contract, ContractWithTermsResponseDto>()
                .ForMember(dst => dst.Terms, opt => opt.MapFrom(src => src.ContractTerms));
        }
    }
}