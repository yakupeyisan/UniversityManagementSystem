namespace UniversityMS.Application.Common.Interfaces;

public interface IAuthorizationService
{
    /// <summary>
    /// Kullanıcının belirtilen izne sahip olup olmadığını asenkron olarak kontrol eder
    /// </summary>
    Task<bool> IsAuthorizedAsync(Guid userId, string permission, Guid? resourceId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fakülte erişim yetkisini kontrol eder
    /// </summary>
    void CheckFacultyAccess(Guid currentUserId, Guid? facultyId, IList<string> userRoles);

    /// <summary>
    /// Departman erişim yetkisini kontrol eder
    /// </summary>
    void CheckDepartmentAccess(Guid currentUserId, Guid? departmentId, IList<string> userRoles);

    /// <summary>
    /// Öğrenci bilgilerine erişim yetkisini kontrol eder
    /// </summary>
    void CheckStudentAccess(Guid currentUserId, Guid studentId, IList<string> userRoles);

    /// <summary>
    /// Kurs erişim yetkisini kontrol eder
    /// </summary>
    void CheckCourseAccess(Guid currentUserId, Guid courseId, IList<string> userRoles);
}