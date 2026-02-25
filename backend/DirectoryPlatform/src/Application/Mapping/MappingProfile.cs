using AutoMapper;
using DirectoryPlatform.Contracts.DTOs.Category;
using DirectoryPlatform.Contracts.DTOs.Listing;
using DirectoryPlatform.Contracts.DTOs.Message;
using DirectoryPlatform.Contracts.DTOs.Notification;
using DirectoryPlatform.Contracts.DTOs.Region;
using DirectoryPlatform.Contracts.DTOs.Review;
using DirectoryPlatform.Contracts.DTOs.Subscription;
using DirectoryPlatform.Contracts.DTOs.User;
using DirectoryPlatform.Core.Entities;

namespace DirectoryPlatform.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Listing
        CreateMap<Listing, ListingDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.User != null ? s.User.Username : null));
        CreateMap<ListingDetail, ListingDetailDto>().ReverseMap();
        CreateMap<ListingAttribute, ListingAttributeDto>()
            .ForMember(d => d.AttributeName, o => o.MapFrom(s => s.AttributeDefinition.Name))
            .ForMember(d => d.AttributeSlug, o => o.MapFrom(s => s.AttributeDefinition.Slug))
            .ForMember(d => d.Unit, o => o.MapFrom(s => s.AttributeDefinition.Unit));
        CreateMap<ListingMedia, ListingMediaDto>()
            .ForMember(d => d.MediaType, o => o.MapFrom(s => s.MediaType.ToString()));

        // Category
        CreateMap<Category, CategoryDto>();
        CreateMap<Category, CategoryWithChildrenDto>();
        CreateMap<CreateCategoryDto, Category>();

        // Region
        CreateMap<Region, RegionDto>();
        CreateMap<Region, RegionWithChildrenDto>();
        CreateMap<CreateRegionDto, Region>();

        // User
        CreateMap<User, UserDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));

        // Review
        CreateMap<Review, ReviewDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.User != null ? s.User.Username : null));

        // Message
        CreateMap<Message, MessageDto>()
            .ForMember(d => d.SenderName, o => o.MapFrom(s => s.Sender != null ? s.Sender.Username : null))
            .ForMember(d => d.RecipientName, o => o.MapFrom(s => s.Recipient != null ? s.Recipient.Username : null));

        // Notification
        CreateMap<Notification, NotificationDto>()
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()));

        // Subscription
        CreateMap<Subscription, SubscriptionDto>()
            .ForMember(d => d.TierName, o => o.MapFrom(s => s.SubscriptionTier != null ? s.SubscriptionTier.Name : null));
        CreateMap<SubscriptionTier, SubscriptionTierDto>();
        CreateMap<SubscriptionFeature, SubscriptionFeatureDto>();
    }
}
