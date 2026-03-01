using AutoMapper;
using DirectoryPlatform.Application.Mapping;
using DirectoryPlatform.Application.Services;
using DirectoryPlatform.Contracts.DTOs.Listing;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Enums;
using DirectoryPlatform.Core.Interfaces;
using FluentAssertions;
using Moq;

namespace DirectoryPlatform.Tests.Services;

public class ListingServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IListingRepository> _listingRepoMock;
    private readonly Mock<IListingAttributeRepository> _listingAttrRepoMock;
    private readonly IMapper _mapper;
    private readonly ListingService _listingService;

    public ListingServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _listingRepoMock = new Mock<IListingRepository>();
        _listingAttrRepoMock = new Mock<IListingAttributeRepository>();

        _unitOfWorkMock.Setup(u => u.Listings).Returns(_listingRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.ListingAttributes).Returns(_listingAttrRepoMock.Object);

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _listingService = new ListingService(_unitOfWorkMock.Object, _mapper);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingListing_ReturnsDto()
    {
        var id = Guid.NewGuid();
        var listing = CreateTestListing(id);
        _listingRepoMock.Setup(r => r.GetWithDetailsAsync(id)).ReturnsAsync(listing);

        var result = await _listingService.GetByIdAsync(id);

        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Listing");
        result.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingListing_ReturnsNull()
    {
        _listingRepoMock.Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Listing?)null);

        var result = await _listingService.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ValidData_ReturnsCreatedListing()
    {
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var dto = new CreateListingDto
        {
            Title = "New Listing",
            ShortDescription = "Short desc",
            CategoryId = categoryId,
            Weight = 10
        };

        _listingRepoMock.Setup(r => r.AddAsync(It.IsAny<Listing>()))
            .ReturnsAsync((Listing l) => l);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var createdListing = new Listing
        {
            Id = Guid.NewGuid(),
            Title = "New Listing",
            ShortDescription = "Short desc",
            CategoryId = categoryId,
            UserId = userId,
            Status = ListingStatus.Active,
            Category = new Category { Name = "Test", Slug = "test" },
            User = new User { Username = "testuser" },
            Attributes = new List<ListingAttribute>(),
            Media = new List<ListingMedia>(),
            Languages = new List<ListingLanguage>(),
            ApprovalHistory = new List<ListingApprovalHistory>()
        };
        _listingRepoMock.Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(createdListing);

        var result = await _listingService.CreateAsync(dto, userId);

        result.Should().NotBeNull();
        result.Title.Should().Be("New Listing");
        _listingRepoMock.Verify(r => r.AddAsync(It.IsAny<Listing>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDetail_SetsListingDetail()
    {
        var userId = Guid.NewGuid();
        var dto = new CreateListingDto
        {
            Title = "With Detail",
            CategoryId = Guid.NewGuid(),
            Detail = new ListingDetailDto
            {
                Address = "123 Main St",
                Phone = "+41 12 345 67 89"
            }
        };

        Listing? captured = null;
        _listingRepoMock.Setup(r => r.AddAsync(It.IsAny<Listing>()))
            .Callback<Listing>(l => captured = l)
            .ReturnsAsync((Listing l) => l);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var returnListing = CreateTestListing(Guid.NewGuid());
        _listingRepoMock.Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(returnListing);

        await _listingService.CreateAsync(dto, userId);

        captured.Should().NotBeNull();
        captured!.Detail.Should().NotBeNull();
        captured.Detail!.Address.Should().Be("123 Main St");
        captured.Detail.Phone.Should().Be("+41 12 345 67 89");
    }

    [Fact]
    public async Task CreateAsync_WithAttributes_SetsListingAttributes()
    {
        var userId = Guid.NewGuid();
        var attrDefId = Guid.NewGuid();
        var dto = new CreateListingDto
        {
            Title = "With Attrs",
            CategoryId = Guid.NewGuid(),
            Attributes = new List<ListingAttributeValueDto>
            {
                new() { AttributeDefinitionId = attrDefId, Value = "Blue" }
            }
        };

        Listing? captured = null;
        _listingRepoMock.Setup(r => r.AddAsync(It.IsAny<Listing>()))
            .Callback<Listing>(l => captured = l)
            .ReturnsAsync((Listing l) => l);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var returnListing = CreateTestListing(Guid.NewGuid());
        _listingRepoMock.Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(returnListing);

        await _listingService.CreateAsync(dto, userId);

        captured.Should().NotBeNull();
        captured!.Attributes.Should().HaveCount(1);
        captured.Attributes.First().Value.Should().Be("Blue");
    }

    [Fact]
    public async Task UpdateAsync_ExistingListing_ByOwner_ReturnsUpdated()
    {
        var userId = Guid.NewGuid();
        var listingId = Guid.NewGuid();
        var existing = CreateTestListing(listingId);
        existing.UserId = userId;

        var dto = new UpdateListingDto
        {
            Title = "Updated Title",
            CategoryId = existing.CategoryId,
            Attributes = new List<ListingAttributeValueDto>()
        };

        var updated = CreateTestListing(listingId);
        updated.Title = "Updated Title";
        updated.UserId = userId;

        _listingRepoMock.SetupSequence(r => r.GetWithDetailsAsync(listingId))
            .ReturnsAsync(existing)
            .ReturnsAsync(updated);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _listingService.UpdateAsync(listingId, dto, userId);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_NonExistingListing_ThrowsKeyNotFoundException()
    {
        _listingRepoMock.Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Listing?)null);

        var act = () => _listingService.UpdateAsync(Guid.NewGuid(), new UpdateListingDto(), Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_NotOwner_ThrowsUnauthorizedAccessException()
    {
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var listing = CreateTestListing(Guid.NewGuid());
        listing.UserId = ownerId;

        _listingRepoMock.Setup(r => r.GetWithDetailsAsync(listing.Id)).ReturnsAsync(listing);

        var act = () => _listingService.UpdateAsync(listing.Id, new UpdateListingDto(), otherUserId);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task DeleteAsync_ByOwner_Succeeds()
    {
        var userId = Guid.NewGuid();
        var listing = new Listing { Id = Guid.NewGuid(), UserId = userId, Title = "Test" };

        _listingRepoMock.Setup(r => r.GetByIdAsync(listing.Id)).ReturnsAsync(listing);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _listingService.DeleteAsync(listing.Id, userId);

        _listingRepoMock.Verify(r => r.DeleteAsync(listing), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NotOwner_ThrowsUnauthorized()
    {
        var listing = new Listing { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Title = "Test" };
        _listingRepoMock.Setup(r => r.GetByIdAsync(listing.Id)).ReturnsAsync(listing);

        var act = () => _listingService.DeleteAsync(listing.Id, Guid.NewGuid());

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task DeleteAsync_NonExisting_ThrowsKeyNotFound()
    {
        _listingRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Listing?)null);

        var act = () => _listingService.DeleteAsync(Guid.NewGuid(), Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetFeaturedAsync_ReturnsListings()
    {
        var listings = new List<Listing>
        {
            CreateTestListing(Guid.NewGuid()),
            CreateTestListing(Guid.NewGuid())
        };
        _listingRepoMock.Setup(r => r.GetFeaturedAsync(10)).ReturnsAsync(listings);

        var result = await _listingService.GetFeaturedAsync(10);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetRecentAsync_ReturnsListings()
    {
        var listings = new List<Listing>
        {
            CreateTestListing(Guid.NewGuid())
        };
        _listingRepoMock.Setup(r => r.GetRecentAsync(5)).ReturnsAsync(listings);

        var result = await _listingService.GetRecentAsync(5);

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetFilteredListingsAsync_ReturnsPaged()
    {
        var items = new List<Listing> { CreateTestListing(Guid.NewGuid()) };
        _listingRepoMock.Setup(r => r.GetFilteredAsync(It.IsAny<ListingFilterParams>()))
            .ReturnsAsync((items, 1));

        var filter = new ListingFilterRequestDto { PageNumber = 1, PageSize = 20 };
        var result = await _listingService.GetFilteredListingsAsync(filter);

        result.Should().NotBeNull();
        result.TotalCount.Should().Be(1);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(20);
    }

    [Fact]
    public async Task UpdateStatusAsync_ValidStatus_UpdatesListing()
    {
        var listing = CreateTestListing(Guid.NewGuid());
        listing.Status = ListingStatus.PendingApproval;

        _listingRepoMock.Setup(r => r.GetWithDetailsAsync(listing.Id)).ReturnsAsync(listing);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _listingService.UpdateStatusAsync(listing.Id, "Active", Guid.NewGuid(), "Approved");

        listing.Status.Should().Be(ListingStatus.Active);
        listing.ApprovalHistory.Should().HaveCount(1);
    }

    [Fact]
    public async Task IncrementViewCountAsync_CallsRepository()
    {
        var id = Guid.NewGuid();
        await _listingService.IncrementViewCountAsync(id);

        _listingRepoMock.Verify(r => r.IncrementViewCountAsync(id), Times.Once);
    }

    private static Listing CreateTestListing(Guid id) => new()
    {
        Id = id,
        Title = "Test Listing",
        ShortDescription = "Description",
        CategoryId = Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        Status = ListingStatus.Active,
        Category = new Category { Name = "Test Cat", Slug = "test-cat" },
        User = new User { Username = "testuser" },
        Attributes = new List<ListingAttribute>(),
        Media = new List<ListingMedia>(),
        Languages = new List<ListingLanguage>(),
        ApprovalHistory = new List<ListingApprovalHistory>()
    };
}
