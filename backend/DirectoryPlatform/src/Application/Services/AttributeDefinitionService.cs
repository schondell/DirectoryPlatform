using System.Text.Json;
using AutoMapper;
using DirectoryPlatform.Contracts.DTOs.Listing;
using DirectoryPlatform.Contracts.Services;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Enums;
using DirectoryPlatform.Core.Interfaces;

namespace DirectoryPlatform.Application.Services;

public class AttributeDefinitionService : IAttributeDefinitionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AttributeDefinitionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AttributeDefinitionDto>> GetByCategoryIdAsync(Guid categoryId, bool filterableOnly = false)
    {
        var attrs = await _unitOfWork.AttributeDefinitions.GetByCategoryIdAsync(categoryId, filterableOnly);
        return attrs.Select(a => new AttributeDefinitionDto
        {
            Id = a.Id,
            Name = a.Name,
            Slug = a.Slug,
            Type = a.Type.ToString(),
            CategoryId = a.CategoryId,
            Options = string.IsNullOrEmpty(a.Options) ? null : JsonSerializer.Deserialize<string[]>(a.Options),
            IsFilterable = a.IsFilterable,
            IsRequired = a.IsRequired,
            DisplayOrder = a.DisplayOrder,
            Description = a.Description,
            Unit = a.Unit,
            MinValue = a.MinValue,
            MaxValue = a.MaxValue
        });
    }

    public async Task<AttributeDefinitionDto?> GetByIdAsync(Guid id)
    {
        var attr = await _unitOfWork.AttributeDefinitions.GetByIdAsync(id);
        if (attr == null) return null;
        return new AttributeDefinitionDto
        {
            Id = attr.Id, Name = attr.Name, Slug = attr.Slug, Type = attr.Type.ToString(),
            CategoryId = attr.CategoryId,
            Options = string.IsNullOrEmpty(attr.Options) ? null : JsonSerializer.Deserialize<string[]>(attr.Options),
            IsFilterable = attr.IsFilterable, IsRequired = attr.IsRequired,
            DisplayOrder = attr.DisplayOrder, Description = attr.Description,
            Unit = attr.Unit, MinValue = attr.MinValue, MaxValue = attr.MaxValue
        };
    }

    public async Task<AttributeDefinitionDto> CreateAsync(CreateAttributeDefinitionDto dto)
    {
        var attr = new AttributeDefinition
        {
            Id = Guid.NewGuid(), Name = dto.Name, Slug = dto.Slug,
            Type = Enum.Parse<AttributeType>(dto.Type), CategoryId = dto.CategoryId,
            Options = dto.Options != null ? JsonSerializer.Serialize(dto.Options) : null,
            IsFilterable = dto.IsFilterable, IsRequired = dto.IsRequired,
            DisplayOrder = dto.DisplayOrder, Description = dto.Description,
            Unit = dto.Unit, MinValue = dto.MinValue, MaxValue = dto.MaxValue
        };
        await _unitOfWork.AttributeDefinitions.AddAsync(attr);
        await _unitOfWork.SaveChangesAsync();
        return (await GetByIdAsync(attr.Id))!;
    }

    public async Task<AttributeDefinitionDto> UpdateAsync(Guid id, UpdateAttributeDefinitionDto dto)
    {
        var attr = await _unitOfWork.AttributeDefinitions.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Attribute definition not found");
        attr.Name = dto.Name; attr.Slug = dto.Slug;
        attr.Type = Enum.Parse<AttributeType>(dto.Type); attr.CategoryId = dto.CategoryId;
        attr.Options = dto.Options != null ? JsonSerializer.Serialize(dto.Options) : null;
        attr.IsFilterable = dto.IsFilterable; attr.IsRequired = dto.IsRequired;
        attr.DisplayOrder = dto.DisplayOrder; attr.Description = dto.Description;
        attr.Unit = dto.Unit; attr.MinValue = dto.MinValue; attr.MaxValue = dto.MaxValue;
        await _unitOfWork.AttributeDefinitions.UpdateAsync(attr);
        await _unitOfWork.SaveChangesAsync();
        return (await GetByIdAsync(attr.Id))!;
    }

    public async Task DeleteAsync(Guid id)
    {
        var attr = await _unitOfWork.AttributeDefinitions.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Attribute definition not found");
        await _unitOfWork.AttributeDefinitions.DeleteAsync(attr);
        await _unitOfWork.SaveChangesAsync();
    }
}
