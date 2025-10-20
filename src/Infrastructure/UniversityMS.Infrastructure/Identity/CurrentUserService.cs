using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using UniversityMS.Application.Common.Interfaces;

namespace UniversityMS.Infrastructure.Identity;
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CurrentUserService> _logger;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<CurrentUserService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Mevcut kullanıcının ID'sini döndürür
    /// </summary>
    public Guid UserId
    {
        get
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userIdClaim == null)
                {
                    _logger.LogWarning("UserId claim not found for current user");
                    return Guid.Empty;
                }

                return Guid.Parse(userIdClaim);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing UserId from claims");
                return Guid.Empty;
            }
        }
    }

    /// <summary>
    /// Mevcut kullanıcının kullanıcı adını döndürür
    /// </summary>
    public string Username
    {
        get
        {
            var username = _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("Username claim not found for current user");
                return "Unknown";
            }

            return username;
        }
    }

    /// <summary>
    /// Mevcut kullanıcının email'ini döndürür
    /// </summary>
    public string Email
    {
        get
        {
            var email = _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Email claim not found for current user");
                return "Unknown";
            }

            return email;
        }
    }

    /// <summary>
    /// Mevcut kullanıcının rollerini döndürür
    /// </summary>
    public IList<string> Roles
    {
        get
        {
            var roles = _httpContextAccessor.HttpContext?.User?
                .FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList() ?? new List<string>();

            return roles.AsReadOnly() as IList<string> ?? roles;
        }
    }

    /// <summary>
    /// Mevcut kullanıcının izinlerini döndürür
    /// </summary>
    public IList<string> Permissions
    {
        get
        {
            var permissions = _httpContextAccessor.HttpContext?.User?
                .FindAll("Permission")
                .Select(c => c.Value)
                .ToList() ?? new List<string>();

            return permissions.AsReadOnly() as IList<string> ?? permissions;
        }
    }

    /// <summary>
    /// Mevcut kullanıcının ait olduğu departmanın ID'sini döndürür
    /// </summary>
    public Guid? DepartmentId
    {
        get
        {
            try
            {
                var departmentIdClaim = _httpContextAccessor.HttpContext?.User?
                    .FindFirst("DepartmentId")?.Value;

                if (string.IsNullOrEmpty(departmentIdClaim))
                    return null;

                return Guid.Parse(departmentIdClaim);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing DepartmentId from claims");
                return null;
            }
        }
    }

    /// <summary>
    /// Mevcut kullanıcının ait olduğu fakültenin ID'sini döndürür
    /// </summary>
    public Guid? FacultyId
    {
        get
        {
            try
            {
                var facultyIdClaim = _httpContextAccessor.HttpContext?.User?
                    .FindFirst("FacultyId")?.Value;

                if (string.IsNullOrEmpty(facultyIdClaim))
                    return null;

                return Guid.Parse(facultyIdClaim);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing FacultyId from claims");
                return null;
            }
        }
    }

    /// <summary>
    /// Kullanıcının Admin rolünde olup olmadığını kontrol eder
    /// </summary>
    public bool IsAdmin => IsInRole("Admin") || IsInRole("SuperAdmin");

    /// <summary>
    /// Kullanıcının kimlik doğrulaması yapılıp yapılmadığını belirtir
    /// </summary>
    public bool IsAuthenticated
    {
        get
        {
            var isAuthenticated = _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
            return isAuthenticated && UserId != Guid.Empty;
        }
    }

    /// <summary>
    /// Kullanıcının belirtilen rolde olup olmadığını kontrol eder
    /// </summary>
    public bool IsInRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return false;

        return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }

    /// <summary>
    /// Kullanıcının belirtilen izne sahip olup olmadığını kontrol eder
    /// </summary>
    public bool HasPermission(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            return false;

        return Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Kullanıcının belirtilen rollerden herhangi birine sahip olup olmadığını kontrol eder
    /// </summary>
    public bool HasAnyRole(params string[] roles)
    {
        if (roles == null || roles.Length == 0)
            return false;

        return roles.Any(role => IsInRole(role));
    }

    /// <summary>
    /// Kullanıcının belirtilen tüm rollere sahip olup olmadığını kontrol eder
    /// </summary>
    public bool HasAllRoles(params string[] roles)
    {
        if (roles == null || roles.Length == 0)
            return true;

        return roles.All(role => IsInRole(role));
    }


    // ============================================
    // EXTENSION METHODS
    // ============================================

    /// <summary>
    /// Kullanıcının Student veya Instructor olduğunu kontrol eder
    /// </summary>
    public bool IsAcademicPerson => HasAnyRole("Student", "Instructor", "Professor", "AssociateProfessor", "PhDStudent", "TeachingAssistant");

    /// <summary>
    /// Kullanıcının IdariPersonel olduğunu kontrol eder
    /// </summary>
    public bool IsAdministrativeStaff => HasAnyRole("Director", "Secretary", "Manager", "Staff");

    /// <summary>
    /// Kullanıcının Dekan veya Dekan Yardımcısı olduğunu kontrol eder
    /// </summary>
    public bool IsFacultyLeadership => HasAnyRole("Dean", "DeanAssistant");

    /// <summary>
    /// Kullanıcının Bölüm Başkanı olduğunu kontrol eder
    /// </summary>
    public bool IsDepartmentHead => IsInRole("DepartmentHead");
}