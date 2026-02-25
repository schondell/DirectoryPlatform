using DirectoryPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.API.Controllers.Admin;

[Authorize(Roles = "Admin,SuperAdmin")]
[Route("api/admin/[controller]")]
public class AuditController : BaseController
{
    private readonly ApplicationDbContext _context;

    public AuditController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var total = await _context.AuditLogs.CountAsync();
        var logs = await _context.AuditLogs
            .Include(a => a.User)
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new
            {
                a.Id, a.Action, a.EntityType, a.EntityId,
                a.IpAddress, a.CreatedAt,
                UserName = a.User != null ? a.User.Username : null
            })
            .ToListAsync();

        return Ok(new { items = logs, totalCount = total, pageNumber, pageSize });
    }
}
