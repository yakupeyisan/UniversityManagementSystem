using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.IdentityAggregate;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Infrastructure.Identity;


public class AuthorizationService : IAuthorizationService
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Department> _departmentRepository;
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<Student> _studentRepository;
    private readonly IRepository<Staff> _staffRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AuthorizationService> _logger;

    public AuthorizationService(
        IRepository<User> userRepository,
        IRepository<Department> departmentRepository,
        IRepository<Course> courseRepository,
        IRepository<Student> studentRepository,
        IRepository<Staff> staffRepository,
        ICurrentUserService currentUserService,
        ILogger<AuthorizationService> logger)
    {
        _userRepository = userRepository;
        _departmentRepository = departmentRepository;
        _courseRepository = courseRepository;
        _studentRepository = studentRepository;
        _staffRepository = staffRepository;
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
            if (user.UserRoles.Any(ur => ur.Role.Name == "Admin" || ur.Role.Name == "SuperAdmin"))
                return true;

            return user.UserRoles.Any(ur => ur.Role.Permissions.Any(p => p.Name == permission));
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
                _logger.LogWarning("Unauthorized faculty access attempt by {UserId} for faculty {FacultyId}",
                    currentUserId, facultyId);
                throw new UnauthorizedAccessException("Bu fakülteye erişim yetkisi yok.");
            }
            return;
        }

        _logger.LogWarning("Unauthorized faculty access attempt by {UserId}", currentUserId);
        throw new UnauthorizedAccessException("Fakülte bilgilerine erişim yetkisi yok.");
    }

    public async void CheckDepartmentAccess(Guid currentUserId, Guid? departmentId, IList<string> userRoles)
    {
        try
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
                    throw new UnauthorizedAccessException("Bu bölüme erişim yetkisi yok.");
                }
                return;
            }

            // Dekan kendi fakültesinin bölümlerine erişim
            if (userRoles.Contains("Dean"))
            {
                if (!departmentId.HasValue)
                    throw new UnauthorizedAccessException("Bölüm ID gereklidir.");

                // Bölümün fakültesini kontrol et
                var department = await _departmentRepository.GetByIdAsync(departmentId.Value, CancellationToken.None);
                if (department == null)
                    throw new UnauthorizedAccessException("Bölüm bulunamadı.");

                if (!_currentUserService.FacultyId.HasValue || department.FacultyId != _currentUserService.FacultyId)
                {
                    _logger.LogWarning("Unauthorized department access attempt by Dean {UserId} for department {DepartmentId}",
                        currentUserId, departmentId);
                    throw new UnauthorizedAccessException("Bu bölüme erişim yetkisi yok.");
                }
                return;
            }

            _logger.LogWarning("Unauthorized department access attempt by {UserId}", currentUserId);
            throw new UnauthorizedAccessException("Bölüm bilgilerine erişim yetkisi yok.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking department access for user {UserId}", currentUserId);
            throw;
        }
    }

    public async void CheckStudentAccess(Guid currentUserId, Guid studentId, IList<string> userRoles)
    {
        try
        {
            // Admin herkese erişim
            if (userRoles.Contains("Admin") || userRoles.Contains("SuperAdmin"))
                return;

            // Öğrenci kendini görebilir
            if (currentUserId == studentId)
                return;

            // Danışman öğrencisini görebilir
            if (userRoles.Contains("Advisor"))
            {
                var student = await _studentRepository.GetByIdAsync(studentId, CancellationToken.None);
                if (student == null)
                    throw new UnauthorizedAccessException("Öğrenci bulunamadı.");

                if (student.AdvisorId != currentUserId)
                {
                    _logger.LogWarning("Unauthorized student access attempt by Advisor {UserId} for student {StudentId}",
                        currentUserId, studentId);
                    throw new UnauthorizedAccessException("Bu öğrencinin danışmanı değilsiniz.");
                }
                return;
            }

            // Öğretim Üyesi kendi dersinin öğrencisini görebilir
            if (userRoles.Contains("Instructor") || userRoles.Contains("Professor") ||
                userRoles.Contains("AssociateProfessor") || userRoles.Contains("TeachingAssistant"))
            {
                var student = await _studentRepository.GetByIdAsync(studentId, CancellationToken.None);
                if (student == null)
                    throw new UnauthorizedAccessException("Öğrenci bulunamadı.");

                // Öğrencinin derslerde bu öğretim üyesi var mı kontrol et
                // Bu kontrol enrollment ve course registration üzerinden yapılmalı
                _logger.LogWarning("Instructor access to student {StudentId} by {UserId}", studentId, currentUserId);
                return;
            }

            // Department Head departmanının öğrencilerini görebilir
            if (userRoles.Contains("DepartmentHead"))
            {
                var student = await _studentRepository.GetByIdAsync(studentId, CancellationToken.None);
                if (student == null)
                    throw new UnauthorizedAccessException("Öğrenci bulunamadı.");

                if (!_currentUserService.DepartmentId.HasValue || student.DepartmentId != _currentUserService.DepartmentId)
                {
                    _logger.LogWarning("Unauthorized student access by DepartmentHead {UserId} for student {StudentId}",
                        currentUserId, studentId);
                    throw new UnauthorizedAccessException("Bu öğrencinin bilgilerine erişim yetkisi yok.");
                }
                return;
            }

            _logger.LogWarning("Unauthorized student access attempt by {UserId} for student {StudentId}",
                currentUserId, studentId);
            throw new UnauthorizedAccessException("Bu öğrencinin bilgilerine erişim yetkisi yok.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking student access for user {UserId}", currentUserId);
            throw;
        }
    }

    public async void CheckCourseAccess(Guid currentUserId, Guid courseId, IList<string> userRoles)
    {
        try
        {
            // Admin herkese erişim
            if (userRoles.Contains("Admin") || userRoles.Contains("SuperAdmin"))
                return;

            var course = await _courseRepository.GetByIdAsync(courseId, CancellationToken.None);
            if (course == null)
                throw new UnauthorizedAccessException("Ders bulunamadı.");

            // Öğretim Üyesi (dersini veren kişi)
            if (userRoles.Contains("Instructor") || userRoles.Contains("Professor") ||
                userRoles.Contains("AssociateProfessor") || userRoles.Contains("TeachingAssistant"))
            {
                // Dersin öğretim üyesi olup olmadığını kontrol et
                var staff = await _staffRepository.GetByIdAsync(currentUserId, CancellationToken.None);
                if (staff == null)
                    throw new UnauthorizedAccessException("Yetkili bulunamadı.");

                // CourseSession'lar üzerinden kontrol yapılmalı - şimdilik log atıyoruz
                _logger.LogInformation("Instructor {UserId} accessing course {CourseId}", currentUserId, courseId);
                return;
            }

            // Department Head kendi departmanının kursları
            if (userRoles.Contains("DepartmentHead"))
            {
                if (course.DepartmentId != _currentUserService.DepartmentId)
                {
                    _logger.LogWarning("Unauthorized course access by DepartmentHead {UserId} for course {CourseId}",
                        currentUserId, courseId);
                    throw new UnauthorizedAccessException("Bu derse erişim yetkisi yok.");
                }
                return;
            }

            // Dean kendi fakültesinin bölümlerinin kursları
            if (userRoles.Contains("Dean"))
            {
                var department = await _departmentRepository.GetByIdAsync(course.DepartmentId, CancellationToken.None);
                if (department == null || department.FacultyId != _currentUserService.FacultyId)
                {
                    _logger.LogWarning("Unauthorized course access by Dean {UserId} for course {CourseId}",
                        currentUserId, courseId);
                    throw new UnauthorizedAccessException("Bu derse erişim yetkisi yok.");
                }
                return;
            }

            _logger.LogWarning("Unauthorized course access attempt by {UserId} for course {CourseId}",
                currentUserId, courseId);
            throw new UnauthorizedAccessException("Bu derse erişim yetkisi yok.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking course access for user {UserId}", currentUserId);
            throw;
        }
    }
}