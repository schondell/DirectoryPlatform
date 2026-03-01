using System.Security.Claims;
using DirectoryPlatform.API.Controllers;
using DirectoryPlatform.Contracts.DTOs.Common;
using DirectoryPlatform.Contracts.DTOs.Listing;
using DirectoryPlatform.Contracts.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DirectoryPlatform.Tests.Controllers;

public class ListingsControllerTests
{
    private readonly Mock<IListingService> _listingServiceMock;
    private readonly ListingsController _controller;
    private readonly Guid _userId = Guid.NewGuid();

    public ListingsControllerTests()
    {
        _listingServiceMock = new Mock<IListingService>();
        _controller = new ListingsController(_listingServiceMock.Object);
        SetupUser(_userId);
    }

    private void SetupUser(Guid userId)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        httpContext.Request.QueryString = new QueryString("");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithPagedResult()
    {
        var pagedResult = new PagedResultDto<ListingDto>
        {
            Items = new List<ListingDto> { new() { Id = Guid.NewGuid(), Title = "Test" } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 20
        };
        _listingServiceMock.Setup(s => s.GetFilteredListingsAsync(It.IsAny<ListingFilterRequestDto>()))
            .ReturnsAsync(pagedResult);

        var result = await _controller.GetAll();

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(pagedResult);
    }

    [Fact]
    public async Task GetFeatured_ReturnsOkWithListings()
    {
        var listings = new List<ListingDto> { new() { Id = Guid.NewGuid(), Title = "Featured" } };
        _listingServiceMock.Setup(s => s.GetFeaturedAsync(10)).ReturnsAsync(listings);

        var result = await _controller.GetFeatured(10);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value as IEnumerable<ListingDto>;
        value.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetRecent_ReturnsOkWithListings()
    {
        var listings = new List<ListingDto> { new() { Id = Guid.NewGuid(), Title = "Recent" } };
        _listingServiceMock.Setup(s => s.GetRecentAsync(5)).ReturnsAsync(listings);

        var result = await _controller.GetRecent(5);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value as IEnumerable<ListingDto>;
        value.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetById_ExistingListing_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var listing = new ListingDto { Id = id, Title = "Test" };
        _listingServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(listing);

        var result = await _controller.GetById(id);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeOfType<ListingDto>().Subject;
        value.Id.Should().Be(id);
        _listingServiceMock.Verify(s => s.IncrementViewCountAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        _listingServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((ListingDto?)null);

        var result = await _controller.GetById(Guid.NewGuid());

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetMyListings_ReturnsOk()
    {
        var listings = new List<ListingDto> { new() { Id = Guid.NewGuid(), Title = "My Listing" } };
        _listingServiceMock.Setup(s => s.GetByUserIdAsync(_userId)).ReturnsAsync(listings);

        var result = await _controller.GetMyListings();

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value as IEnumerable<ListingDto>;
        value.Should().HaveCount(1);
    }

    [Fact]
    public async Task Create_ValidData_ReturnsCreatedAtAction()
    {
        var dto = new CreateListingDto { Title = "New", CategoryId = Guid.NewGuid() };
        var created = new ListingDto { Id = Guid.NewGuid(), Title = "New" };
        _listingServiceMock.Setup(s => s.CreateAsync(dto, _userId)).ReturnsAsync(created);

        var result = await _controller.Create(dto);

        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be("GetById");
        var value = createdResult.Value.Should().BeOfType<ListingDto>().Subject;
        value.Title.Should().Be("New");
    }

    [Fact]
    public async Task Update_ByOwner_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var dto = new UpdateListingDto { Title = "Updated", CategoryId = Guid.NewGuid() };
        var updated = new ListingDto { Id = id, Title = "Updated" };
        _listingServiceMock.Setup(s => s.UpdateAsync(id, dto, _userId)).ReturnsAsync(updated);

        var result = await _controller.Update(id, dto);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeOfType<ListingDto>().Subject;
        value.Title.Should().Be("Updated");
    }

    [Fact]
    public async Task Update_NotOwner_ReturnsForbid()
    {
        var id = Guid.NewGuid();
        var dto = new UpdateListingDto { Title = "Updated" };
        _listingServiceMock.Setup(s => s.UpdateAsync(id, dto, _userId))
            .ThrowsAsync(new UnauthorizedAccessException());

        var result = await _controller.Update(id, dto);

        result.Result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task Update_NonExisting_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        var dto = new UpdateListingDto { Title = "Updated" };
        _listingServiceMock.Setup(s => s.UpdateAsync(id, dto, _userId))
            .ThrowsAsync(new KeyNotFoundException());

        var result = await _controller.Update(id, dto);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ByOwner_ReturnsNoContent()
    {
        var id = Guid.NewGuid();

        var result = await _controller.Delete(id);

        result.Should().BeOfType<NoContentResult>();
        _listingServiceMock.Verify(s => s.DeleteAsync(id, _userId), Times.Once);
    }

    [Fact]
    public async Task Delete_NotOwner_ReturnsForbid()
    {
        var id = Guid.NewGuid();
        _listingServiceMock.Setup(s => s.DeleteAsync(id, _userId))
            .ThrowsAsync(new UnauthorizedAccessException());

        var result = await _controller.Delete(id);

        result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task Delete_NonExisting_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        _listingServiceMock.Setup(s => s.DeleteAsync(id, _userId))
            .ThrowsAsync(new KeyNotFoundException());

        var result = await _controller.Delete(id);

        result.Should().BeOfType<NotFoundResult>();
    }
}
