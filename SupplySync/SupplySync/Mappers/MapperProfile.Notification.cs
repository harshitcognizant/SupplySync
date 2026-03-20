// /SupplySync/Mappers/Mapper.Notification.cs
using AutoMapper;
using SupplySync.DTOs.Notification;
using SupplySync.Models;
using SupplySync.Constants.Enums;

namespace SupplySync.Mappers
{
	public partial class MapperProfile
	{
		private void ConfigureNotificationMappings()
		{
			CreateMap<CreateNotificationRequestDto, Notification>()
				.ForMember(d => d.IsDeleted, opt => opt.MapFrom(_ => false))
				.ForMember(d => d.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(d => d.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(d => d.Status, opt => opt.MapFrom(src => src.Status ?? NotificationStatus.Unread));

			CreateMap<Notification, NotificationResponseDto>()
				.ForMember(d => d.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : null));
		}
	}
}