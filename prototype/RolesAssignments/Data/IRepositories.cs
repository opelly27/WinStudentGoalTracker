// =============================================================================
// IStudentRepository.cs / IGoalRepository.cs / IProgressEntryRepository.cs
// =============================================================================
// All three repository interfaces in one file for this prototype.
// Each defines the operations available for that entity.
// =============================================================================

using RolesAssignments.Models;

namespace RolesAssignments.Data;

// ---------------------------------------------------------------------------
// STUDENT REPOSITORY
// ---------------------------------------------------------------------------
public interface IStudentRepository
{
    /// <summary>
    /// Returns a list of students the given user is allowed to see.
    /// This is the "scoped list" query — it filters at the database level
    /// so unauthorized student records never leave the data layer.
    /// </summary>
    Task<IEnumerable<StudentSummaryDto>> GetAccessibleStudents(int userId);

    /// <summary>
    /// Returns a single student by ID, or null if not found.
    /// NOTE: This does NOT check authorization — the controller is responsible
    /// for calling AuthorizeAsync before returning the data.
    /// </summary>
    Task<Student?> GetById(int studentId);
}

// ---------------------------------------------------------------------------
// GOAL REPOSITORY  
// ---------------------------------------------------------------------------
public interface IGoalRepository
{
    Task<IEnumerable<Goal>> GetByStudentId(int studentId);
    Task<Goal?> GetById(int goalId);
    Task<int> Create(int studentId, CreateGoalRequest request, int createdByUserId);
    Task Update(int goalId, UpdateGoalRequest request, int updatedByUserId);
}

// ---------------------------------------------------------------------------
// PROGRESS ENTRY REPOSITORY
// ---------------------------------------------------------------------------
public interface IProgressEntryRepository
{
    Task<IEnumerable<ProgressEntry>> GetByStudentId(int studentId);
    Task<ProgressEntry?> GetById(int entryId);
    Task<int> Create(int studentId, CreateEntryRequest request, int createdByUserId);
    Task Update(int entryId, UpdateEntryRequest request, int updatedByUserId);
    Task SoftDelete(int entryId, int deletedByUserId);
}
