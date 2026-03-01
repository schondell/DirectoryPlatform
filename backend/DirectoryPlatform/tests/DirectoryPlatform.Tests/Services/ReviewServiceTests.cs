using AutoMapper;
using DirectoryPlatform.Application.Mapping;
using DirectoryPlatform.Application.Services;
using DirectoryPlatform.Contracts.DTOs.Review;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Enums;
using DirectoryPlatform.Core.Interfaces;
using FluentAssertions;
using Moq;

namespace DirectoryPlatform.Tests.Services;

public class ReviewServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IReviewRepository> _reviewRepoMock;
    private readonly IMapper _mapper;
    private readonly ReviewService _reviewService;

    public ReviewServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _reviewRepoMock = new Mock<IReviewRepository>();
        _unitOfWorkMock.Setup(u => u.Reviews).Returns(_reviewRepoMock.Object);

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _reviewService = new ReviewService(_unitOfWorkMock.Object, _mapper);
    }

    [Fact]
    public async Task GetByListingIdAsync_ReturnsReviews()
    {
        var listingId = Guid.NewGuid();
        var reviews = new List<Review>
        {
            new() { Id = Guid.NewGuid(), ListingId = listingId, UserId = Guid.NewGuid(), Rating = 4, Comment = "Good", Status = ReviewStatus.Approved }
        };
        _reviewRepoMock.Setup(r => r.GetByListingIdAsync(listingId)).ReturnsAsync(reviews);

        var result = await _reviewService.GetByListingIdAsync(listingId);

        result.Should().HaveCount(1);
        result.First().Rating.Should().Be(4);
    }

    [Fact]
    public async Task CreateAsync_ValidData_ReturnsCreatedReview()
    {
        var userId = Guid.NewGuid();
        var dto = new CreateReviewDto
        {
            ListingId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Excellent!"
        };

        _reviewRepoMock.Setup(r => r.AddAsync(It.IsAny<Review>()))
            .ReturnsAsync((Review r) => r);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _reviewService.CreateAsync(dto, userId);

        result.Should().NotBeNull();
        result.Rating.Should().Be(5);
        result.Comment.Should().Be("Excellent!");
        result.Status.Should().Be("Pending");
    }

    [Fact]
    public async Task UpdateStatusAsync_ValidStatus_UpdatesReview()
    {
        var review = new Review
        {
            Id = Guid.NewGuid(), ListingId = Guid.NewGuid(), UserId = Guid.NewGuid(),
            Rating = 4, Status = ReviewStatus.Pending
        };
        _reviewRepoMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _reviewService.UpdateStatusAsync(review.Id, "Approved");

        result.Status.Should().Be("Approved");
        review.Status.Should().Be(ReviewStatus.Approved);
    }

    [Fact]
    public async Task UpdateStatusAsync_NonExisting_ThrowsKeyNotFound()
    {
        _reviewRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Review?)null);

        var act = () => _reviewService.UpdateStatusAsync(Guid.NewGuid(), "Approved");

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_ExistingReview_Deletes()
    {
        var review = new Review { Id = Guid.NewGuid(), ListingId = Guid.NewGuid(), UserId = Guid.NewGuid(), Rating = 3 };
        _reviewRepoMock.Setup(r => r.GetByIdAsync(review.Id)).ReturnsAsync(review);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _reviewService.DeleteAsync(review.Id);

        _reviewRepoMock.Verify(r => r.DeleteAsync(review), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExisting_ThrowsKeyNotFound()
    {
        _reviewRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Review?)null);

        var act = () => _reviewService.DeleteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetAverageRatingAsync_ReturnsAverage()
    {
        var listingId = Guid.NewGuid();
        _reviewRepoMock.Setup(r => r.GetAverageRatingAsync(listingId)).ReturnsAsync(4.5);

        var result = await _reviewService.GetAverageRatingAsync(listingId);

        result.Should().Be(4.5);
    }

    [Fact]
    public async Task GetAverageRatingAsync_NoReviews_ReturnsZero()
    {
        var listingId = Guid.NewGuid();
        _reviewRepoMock.Setup(r => r.GetAverageRatingAsync(listingId)).ReturnsAsync(0.0);

        var result = await _reviewService.GetAverageRatingAsync(listingId);

        result.Should().Be(0.0);
    }
}
