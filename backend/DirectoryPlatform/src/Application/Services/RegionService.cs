using AutoMapper;
using DirectoryPlatform.Contracts.DTOs.Region;
using DirectoryPlatform.Contracts.Services;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;

namespace DirectoryPlatform.Application.Services;

public class RegionService : IRegionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RegionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RegionDto>> GetAllAsync()
    {
        var regions = await _unitOfWork.Regions.GetAllAsync();
        return _mapper.Map<IEnumerable<RegionDto>>(regions);
    }

    public async Task<IEnumerable<RegionWithChildrenDto>> GetTreeAsync()
    {
        var all = await _unitOfWork.Regions.GetAllWithChildrenAsync();
        var roots = all.Where(r => r.ParentId == null).OrderBy(r => r.DisplayOrder);
        return _mapper.Map<IEnumerable<RegionWithChildrenDto>>(roots);
    }

    public async Task<RegionWithChildrenDto?> GetByIdAsync(Guid id)
    {
        var region = await _unitOfWork.Regions.GetWithChildrenAsync(id);
        return region == null ? null : _mapper.Map<RegionWithChildrenDto>(region);
    }

    public async Task<RegionDto?> GetBySlugAsync(string slug)
    {
        var region = await _unitOfWork.Regions.GetBySlugAsync(slug);
        return region == null ? null : _mapper.Map<RegionDto>(region);
    }

    public async Task<RegionDto> CreateAsync(CreateRegionDto dto)
    {
        var region = _mapper.Map<Region>(dto);
        region.Id = Guid.NewGuid();
        await _unitOfWork.Regions.AddAsync(region);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<RegionDto>(region);
    }

    public async Task<RegionDto> UpdateAsync(Guid id, CreateRegionDto dto)
    {
        var region = await _unitOfWork.Regions.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Region not found");
        _mapper.Map(dto, region);
        await _unitOfWork.Regions.UpdateAsync(region);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<RegionDto>(region);
    }

    public async Task DeleteAsync(Guid id)
    {
        var region = await _unitOfWork.Regions.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Region not found");
        await _unitOfWork.Regions.DeleteAsync(region);
        await _unitOfWork.SaveChangesAsync();
    }
}
