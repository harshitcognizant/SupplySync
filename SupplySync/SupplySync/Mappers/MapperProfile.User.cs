using SupplySync.DTOs.User;
using SupplySync.Models;

namespace SupplySync.Mappers
{
	public partial class MapperProfile
	{
		private void ConfigureUserMappings()
		{
			CreateMap<CreateUserRequestDto, User>()
				.ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
				.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));


			CreateMap<User, UserResponseDto>()
				.ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
					src.UserRoles.Where(ur => !ur.IsDeleted && ur.Role!=null && !ur.Role.IsDeleted)
					.Select(ur => new UserRoleSummaryDto
					{
						RoleID = ur.RoleID,
						RoleType = ur.Role.RoleType.ToString()
					})));

		}
	}
}