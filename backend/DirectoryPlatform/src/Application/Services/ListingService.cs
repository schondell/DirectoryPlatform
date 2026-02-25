using AutoMapper;
using DirectoryPlatform.Contracts.DTOs.Common;
using DirectoryPlatform.Contracts.DTOs.Listing;
using DirectoryPlatform.Contracts.Services;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Enums;
using DirectoryPlatform.Core.Interfaces;

namespace DirectoryPlatform.Application.Services;

public class ListingService : IListingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ListingService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ListingDto?> GetByIdAsync(Guid id)
    {
        var listing = await _unitOfWork.Listings.GetWithDetailsAsync(id);
        return listing == null ? null : _mapper.Map<ListingDto>(listing);
    }

    public async Task<PagedResultDto<ListingDto>> GetFilteredListingsAsync(ListingFilterRequestDto filter)
    {
        var filterParams = new ListingFilterParams
        {
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            SearchTerm = filter.SearchTerm,
            CategoryId = filter.CategoryId,
            RegionId = filter.RegionId,
            SortBy = filter.SortBy,
            Ascending = filter.Ascending,
            AttributeFilters = filter.Attributes
        };

        var (items, totalCount) = await _unitOfWork.Listings.GetFilteredAsync(filterParams);

        return new PagedResultDto<ListingDto>
        {
            Items = _mapper.Map<IEnumerable<ListingDto>>(items),
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<IEnumerable<ListingDto>> GetByUserIdAsync(Guid userId)
    {
        var listings = await _unitOfWork.Listings.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<ListingDto>>(listings);
    }

    public async Task<IEnumerable<ListingDto>> GetFeaturedAsync(int count = 10)
    {
        var listings = await _unitOfWork.Listings.GetFeaturedAsync(count);
        return _mapper.Map<IEnumerable<ListingDto>>(listings);
    }

    public async Task<IEnumerable<ListingDto>> GetRecentAsync(int count = 10)
    {
        var listings = await _unitOfWork.Listings.GetRecentAsync(count);
        return _mapper.Map<IEnumerable<ListingDto>>(listings);
    }

    public async Task<ListingDto> CreateAsync(CreateListingDto dto, Guid userId)
    {
        var listing = new Listing
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            ShortDescription = dto.ShortDescription,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            RegionId = dto.RegionId,
            Town = dto.Town,
            UserId = userId,
            Status = ListingStatus.Active,
            IsFeatured = dto.IsFeatured,
            IsPremium = dto.IsPremium,
            Weight = dto.Weight
        };

        if (dto.Detail != null)
        {
            listing.Detail = new ListingDetail
            {
                Id = Guid.NewGuid(),
                ListingId = listing.Id,
                Address = dto.Detail.Address,
                Latitude = dto.Detail.Latitude,
                Longitude = dto.Detail.Longitude,
                Phone = dto.Detail.Phone,
                Email = dto.Detail.Email,
                Website = dto.Detail.Website,
                AvailabilityHours = dto.Detail.AvailabilityHours,
                PriceInfo = dto.Detail.PriceInfo,
                PaymentMethods = dto.Detail.PaymentMethods
            };
        }

        foreach (var attr in dto.Attributes)
        {
            listing.Attributes.Add(new ListingAttribute
            {
                Id = Guid.NewGuid(),
                ListingId = listing.Id,
                AttributeDefinitionId = attr.AttributeDefinitionId,
                Value = attr.Value,
                DisplayOrder = listing.Attributes.Count
            });
        }

        await _unitOfWork.Listings.AddAsync(listing);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ListingDto>(await _unitOfWork.Listings.GetWithDetailsAsync(listing.Id));
    }

    public async Task<ListingDto> UpdateAsync(Guid id, UpdateListingDto dto, Guid userId)
    {
        var listing = await _unitOfWork.Listings.GetWithDetailsAsync(id)
            ?? throw new KeyNotFoundException("Listing not found");
        if (listing.UserId != userId)
            throw new UnauthorizedAccessException("Not authorized to update this listing");

        listing.Title = dto.Title;
        listing.ShortDescription = dto.ShortDescription;
        listing.Description = dto.Description;
        listing.CategoryId = dto.CategoryId;
        listing.RegionId = dto.RegionId;
        listing.Town = dto.Town;

        if (dto.Detail != null)
        {
            if (listing.Detail == null)
            {
                listing.Detail = new ListingDetail { Id = Guid.NewGuid(), ListingId = listing.Id };
            }
            listing.Detail.Address = dto.Detail.Address;
            listing.Detail.Latitude = dto.Detail.Latitude;
            listing.Detail.Longitude = dto.Detail.Longitude;
            listing.Detail.Phone = dto.Detail.Phone;
            listing.Detail.Email = dto.Detail.Email;
            listing.Detail.Website = dto.Detail.Website;
            listing.Detail.AvailabilityHours = dto.Detail.AvailabilityHours;
            listing.Detail.PriceInfo = dto.Detail.PriceInfo;
            listing.Detail.PaymentMethods = dto.Detail.PaymentMethods;
        }

        // Replace attributes
        foreach (var existing in listing.Attributes.ToList())
            await _unitOfWork.ListingAttributes.DeleteAsync(existing);

        foreach (var attr in dto.Attributes)
        {
            listing.Attributes.Add(new ListingAttribute
            {
                Id = Guid.NewGuid(),
                ListingId = listing.Id,
                AttributeDefinitionId = attr.AttributeDefinitionId,
                Value = attr.Value,
                DisplayOrder = listing.Attributes.Count
            });
        }

        await _unitOfWork.Listings.UpdateAsync(listing);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ListingDto>(await _unitOfWork.Listings.GetWithDetailsAsync(listing.Id));
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var listing = await _unitOfWork.Listings.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Listing not found");
        if (listing.UserId != userId)
            throw new UnauthorizedAccessException("Not authorized to delete this listing");
        await _unitOfWork.Listings.DeleteAsync(listing);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<ListingDto> UpdateStatusAsync(Guid id, string status, Guid adminUserId, string? comment = null)
    {
        var listing = await _unitOfWork.Listings.GetWithDetailsAsync(id)
            ?? throw new KeyNotFoundException("Listing not found");
        var oldStatus = listing.Status;
        listing.Status = Enum.Parse<ListingStatus>(status);
        listing.ApprovalHistory.Add(new ListingApprovalHistory
        {
            Id = Guid.NewGuid(),
            ListingId = listing.Id,
            ReviewedByUserId = adminUserId,
            OldStatus = oldStatus,
            NewStatus = listing.Status,
            Comment = comment
        });
        await _unitOfWork.Listings.UpdateAsync(listing);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ListingDto>(listing);
    }

    public async Task IncrementViewCountAsync(Guid id)
    {
        await _unitOfWork.Listings.IncrementViewCountAsync(id);
    }
}
