using AutoMapper;
using OrphanManagement.Application.DTOs.Orphans;
using OrphanManagement.Application.DTOs.Events;
using OrphanManagement.Application.DTOs.Users;
using OrphanManagement.Domain.Entities;

namespace OrphanManagement.Application.Mappings;

/// <summary>
/// AutoMapper profile for mapping between entities and DTOs
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
        
        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Set separately after hashing
        
        CreateMap<UpdateUserRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        
        // Orphan mappings
        CreateMap<Orphan, OrphanDto>()
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
            .ForMember(dest => dest.HealthStatus, opt => opt.MapFrom(src => src.HealthStatus.ToString()))
            .ForMember(dest => dest.EducationStatus, opt => opt.MapFrom(src => src.EducationStatus.ToString()))
            .ForMember(dest => dest.SponsorshipStatus, opt => opt.MapFrom(src => src.SponsorshipStatus.ToString()))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age));
        
        CreateMap<CreateOrphanRequest, Orphan>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.PhotoUrl, opt => opt.Ignore())
            .ForMember(dest => dest.Sponsorships, opt => opt.Ignore())
            .ForMember(dest => dest.OrphanEvents, opt => opt.Ignore());
        
        CreateMap<UpdateOrphanRequest, Orphan>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.PhotoUrl, opt => opt.Ignore())
            .ForMember(dest => dest.Sponsorships, opt => opt.Ignore())
            .ForMember(dest => dest.OrphanEvents, opt => opt.Ignore());
        
        // Event mappings
        CreateMap<Event, EventDto>()
            .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => src.EventType.ToString()))
            .ForMember(dest => dest.ParticipantCount, opt => opt.MapFrom(src => src.ParticipantCount))
            .ForMember(dest => dest.IsUpcoming, opt => opt.MapFrom(src => src.IsUpcoming))
            .ForMember(dest => dest.IsOngoing, opt => opt.MapFrom(src => src.IsOngoing))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted));
        
        CreateMap<CreateEventRequest, Event>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.OrphanEvents, opt => opt.Ignore());
        
        CreateMap<UpdateEventRequest, Event>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.OrphanEvents, opt => opt.Ignore());
    }
}
