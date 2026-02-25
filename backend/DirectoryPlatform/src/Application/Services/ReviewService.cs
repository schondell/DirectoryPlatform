using AutoMapper;
using DirectoryPlatform.Contracts.DTOs.Review;
using DirectoryPlatform.Contracts.Services;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Enums;
using DirectoryPlatform.Core.Interfaces;

namespace DirectoryPlatform.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReviewDto>> GetByListingIdAsync(Guid listingId)
    {
        var reviews = await _unitOfWork.Reviews.GetByListingIdAsync(listingId);
        return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }

    public async Task<ReviewDto> CreateAsync(CreateReviewDto dto, Guid userId)
    {
        var review = new Review
        {
            Id = Guid.NewGuid(), ListingId = dto.ListingId,
            UserId = userId, Rating = dto.Rating, Comment = dto.Comment,
            Status = ReviewStatus.Pending
        };
        await _unitOfWork.Reviews.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ReviewDto>(review);
    }

    public async Task<ReviewDto> UpdateStatusAsync(Guid id, string status)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Review not found");
        review.Status = Enum.Parse<ReviewStatus>(status);
        await _unitOfWork.Reviews.UpdateAsync(review);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ReviewDto>(review);
    }

    public async Task DeleteAsync(Guid id)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Review not found");
        await _unitOfWork.Reviews.DeleteAsync(review);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<double> GetAverageRatingAsync(Guid listingId)
        => await _unitOfWork.Reviews.GetAverageRatingAsync(listingId);
}
