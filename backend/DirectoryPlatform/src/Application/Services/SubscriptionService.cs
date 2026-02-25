using AutoMapper;
using DirectoryPlatform.Contracts.DTOs.Subscription;
using DirectoryPlatform.Contracts.Services;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;

namespace DirectoryPlatform.Application.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SubscriptionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SubscriptionDto?> GetActiveByUserIdAsync(Guid userId)
    {
        var sub = await _unitOfWork.Subscriptions.GetActiveByUserIdAsync(userId);
        return sub == null ? null : _mapper.Map<SubscriptionDto>(sub);
    }

    public async Task<IEnumerable<SubscriptionTierDto>> GetTiersAsync()
    {
        var tiers = await _unitOfWork.Subscriptions.GetActiveTiersAsync();
        return _mapper.Map<IEnumerable<SubscriptionTierDto>>(tiers);
    }

    public async Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto dto, Guid userId)
    {
        var tier = await _unitOfWork.Subscriptions.GetByIdAsync(dto.SubscriptionTierId); // Would need SubscriptionTier repo
        var sub = new Subscription
        {
            Id = Guid.NewGuid(), UserId = userId,
            SubscriptionTierId = dto.SubscriptionTierId,
            StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(1),
            IsActive = true, AutoRenew = dto.AutoRenew
        };
        await _unitOfWork.Subscriptions.AddAsync(sub);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<SubscriptionDto>(sub);
    }

    public async Task CancelAsync(Guid id, Guid userId)
    {
        var sub = await _unitOfWork.Subscriptions.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Subscription not found");
        if (sub.UserId != userId)
            throw new UnauthorizedAccessException("Not authorized");
        sub.IsActive = false;
        sub.AutoRenew = false;
        await _unitOfWork.Subscriptions.UpdateAsync(sub);
        await _unitOfWork.SaveChangesAsync();
    }
}
