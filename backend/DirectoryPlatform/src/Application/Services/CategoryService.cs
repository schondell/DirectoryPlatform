using AutoMapper;
using DirectoryPlatform.Contracts.DTOs.Category;
using DirectoryPlatform.Contracts.Services;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;

namespace DirectoryPlatform.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<IEnumerable<CategoryWithChildrenDto>> GetTreeAsync()
    {
        var all = await _unitOfWork.Categories.GetAllWithChildrenAsync();
        var roots = all.Where(c => c.ParentId == null).OrderBy(c => c.DisplayOrder);
        return _mapper.Map<IEnumerable<CategoryWithChildrenDto>>(roots);
    }

    public async Task<CategoryWithChildrenDto?> GetByIdAsync(Guid id)
    {
        var category = await _unitOfWork.Categories.GetWithChildrenAsync(id);
        return category == null ? null : _mapper.Map<CategoryWithChildrenDto>(category);
    }

    public async Task<CategoryDto?> GetBySlugAsync(string slug)
    {
        var category = await _unitOfWork.Categories.GetBySlugAsync(slug);
        return category == null ? null : _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var category = _mapper.Map<Category>(dto);
        category.Id = Guid.NewGuid();
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> UpdateAsync(Guid id, CreateCategoryDto dto)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Category not found");
        _mapper.Map(dto, category);
        await _unitOfWork.Categories.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Category not found");
        await _unitOfWork.Categories.DeleteAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }
}
