using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Data;
using RaceDay.Api.Dtos;
using RaceDay.Api.Security;
using RaceDay.Api.Services;

namespace RaceDay.Api.Controllers;

/// <summary>The signed-in user's own profile. Both roles.</summary>
[Route("api/profile")]
[SessionAuthorize]
public class ProfileController : RaceDayControllerBase
{
    private readonly RaceDayDbContext _db;
    private readonly IBannerStorage _storage;
    public ProfileController(RaceDayDbContext db, IBannerStorage storage) { _db = db; _storage = storage; }

    /// <summary>Read the signed-in user's profile.</summary>
    /// <response code="200">The profile.</response>
    /// <response code="401">Not signed in.</response>
    [HttpGet]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get()
    {
        if (CurrentUserId is not int userId) return UnauthorizedProblem();
        var user = await _db.Users.FindAsync(userId);
        return user is null ? UnauthorizedProblem() : Ok(user.ToResponse());
    }

    /// <summary>Update name, phone and profile image URL. Email and role cannot be changed here.</summary>
    /// <response code="200">Updated profile.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="401">Not signed in.</response>
    [HttpPut]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update([FromBody] UpdateProfileRequest request)
    {
        if (CurrentUserId is not int userId) return UnauthorizedProblem();
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return UnauthorizedProblem();

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
        user.ProfileImageUrl = string.IsNullOrWhiteSpace(request.ProfileImageUrl) ? null : request.ProfileImageUrl.Trim();
        await _db.SaveChangesAsync();

        return Ok(user.ToResponse());
    }

    /// <summary>Upload a profile picture through the API to the configured storage provider.</summary>
    [HttpPost("image")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (CurrentUserId is not int userId) return UnauthorizedProblem();
        if (file is null || file.Length == 0) return BadRequestProblem("Choose an image to upload.");
        if (file.Length > 5 * 1024 * 1024) return BadRequestProblem("Image must be 5 MB or smaller.");
        var extensions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        { ["image/jpeg"] = ".jpg", ["image/png"] = ".png", ["image/webp"] = ".webp" };
        if (!extensions.TryGetValue(file.ContentType, out var extension))
            return BadRequestProblem("Only JPEG, PNG and WebP images are accepted.");
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return UnauthorizedProblem();
        await using var stream = file.OpenReadStream();
        user.ProfileImageUrl = await _storage.SaveBannerAsync(-userId, stream, file.ContentType, extension, cancellationToken);
        await _db.SaveChangesAsync();
        return Ok(user.ToResponse());
    }
}
