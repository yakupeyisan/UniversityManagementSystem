namespace UniversityMS.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Username { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    IEnumerable<string> Roles { get; }
    IEnumerable<string> Permissions { get; }
    bool IsInRole(string role);

    /// <summary>
    /// Şu an giriş yapan kullanıcının ID'sini döner
    /// </summary>
    Guid GetCurrentUserId();

    /// <summary>
    /// Şu an giriş yapan kullanıcının email'ini döner
    /// </summary>
    string GetCurrentUserEmail();

    /// <summary>
    /// Şu an giriş yapan kullanıcının rollerini döner
    /// </summary>
    IList<string> GetUserRoles();

    /// <summary>
    /// Kullanıcının belirli bir yetki sahibi olup olmadığını kontrol eder
    /// </summary>
    bool HasPermission(string permission);
}