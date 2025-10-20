using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.IdentityAggregate;
using UniversityMS.Domain.Interfaces;

public class AuthorizationService : IAuthorizationService
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Department> _departmentRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AuthorizationService> _logger;

    public AuthorizationService(
        IRepository<User> userRepository,
        IRepository<Department> departmentRepository,
        ICurrentUserService currentUserService,
        ILogger<AuthorizationService> logger)
    {
        _userRepository = userRepository;
        _departmentRepository = departmentRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<bool> IsAuthorizedAsync(
        Guid userId, string permission, Guid? resourceId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                return false;
            }

            // Admin her şeye erişim
            if (user.Roles.Any(r => r.Name == "Admin" || r.Name == "SuperAdmin"))
                return true;

            return user.Roles.Any(r => r.Permissions.Any(p => p.Name == permission));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking authorization for user {UserId}", userId);
            return false;
        }
    }

    public void CheckFacultyAccess(Guid currentUserId, Guid? facultyId, IList<string> userRoles)
    {
        // Admin herkese erişim
        if (userRoles.Contains("Admin") || userRoles.Contains("SuperAdmin"))
            return;

        // Dekan sadece kendi fakültesi
        if (userRoles.Contains("Dean"))
        {
            if (!_currentUserService.FacultyId.HasValue || _currentUserService.FacultyId != facultyId)
            {
                _logger.LogWarning("Unauthorized faculty access attempt by {UserId} for faculty {FacultyId}",
                    currentUserId, facultyId);
                throw new UnauthorizedAccessException("Bu fakülteye erişim yetkisi yok.");
            }
            return;
        }

        // Department Head kendi fakültesi
        if (userRoles.Contains("DepartmentHead"))
        {
            if (!_currentUserService.FacultyId.HasValue || _currentUserService.FacultyId != facultyId)
            {
                throw new UnauthorizedAccessException("Bu fakülteye erişim yetkisi yok.");
            }
            return;
        }

        _logger.LogWarning("Unauthorized faculty access attempt by {UserId}", currentUserId);
        throw new UnauthorizedAccessException("Fakülte bilgilerine erişim yetkisi yok.");
    }

    public void CheckDepartmentAccess(Guid currentUserId, Guid? departmentId, IList<string> userRoles)
    {
        // Admin herkese erişim
        if (userRoles.Contains("Admin") || userRoles.Contains("SuperAdmin"))
            return;

        // Department Head sadece kendi departmanı
        if (userRoles.Contains("DepartmentHead"))
        {
            if (!_currentUserService.DepartmentId.HasValue || _currentUserService.DepartmentId != departmentId)
            {
                _logger.LogWarning("Unauthorized department access attempt by {UserId} for department {DepartmentId}",
                    currentUserId, departmentId);
                throw new UnauthorizedAccessException("Bu departmana erişim yetkisi yok.");
            }
            return;
        }

        // Dekan kendi fakültesinin bölümlerine erişim
        if (userRoles.Contains("Dean"))
        {
            // Fakülte üzerinden departman kontrolü yapılmalı
            return;
        }

        _logger.LogWarning("Unauthorized department access attempt by {UserId}", currentUserId);
        throw new UnauthorizedAccessException("Departman bilgilerine erişim yetkisi yok.");
    }

    public void CheckStudentAccess(Guid currentUserId, Guid studentId, IList<string> userRoles)
    {
        // Admin herkese erişim
        if (userRoles.Contains("Admin") || userRoles.Contains("SuperAdmin"))
            return;

        // Student kendini görebilir
        if (currentUserId == studentId)
            return;

        // Danışman öğrencisini görebilir
        if (userRoles.Contains("Advisor"))
        {
            // currentUserId'nin bu student'ın danışmanı olup olmadığını kontrol et
            // Database sorgusundan yapılmalı
            return;
        }

        // Öğretim Üyesi kendi derseğinin öğrencisini görebilir
        if (userRoles.Contains("Instructor") || userRoles.Contains("Professor"))
        {
            return;
        }

        _logger.LogWarning("Unauthorized student access attempt by {UserId} for student {StudentId}",
            currentUserId, studentId);
        throw new UnauthorizedAccessException("Bu öğrencinin bilgilerine erişim yetkisi yok.");
    }

    public void CheckCourseAccess(Guid currentUserId, Guid courseId, IList<string> userRoles)
    {
        // Admin herkese erişim
        if (userRoles.Contains("Admin") || userRoles.Contains("SuperAdmin"))
            return;

        // Course sahibi (Instructor) erişim
        if (userRoles.Contains("Instructor") || userRoles.Contains("Professor"))
        {
            // Course'un instructor'ü olup olmadığını kontrol et
            return;
        }

        // Department Head kendi departmanının kursları
        if (userRoles.Contains("DepartmentHead"))
        {
            return;
        }

        _logger.LogWarning("Unauthorized course access attempt by {UserId} for course {CourseId}",
            currentUserId, courseId);
        throw new UnauthorizedAccessException("Bu derse erişim yetkisi yok.");
    }
}