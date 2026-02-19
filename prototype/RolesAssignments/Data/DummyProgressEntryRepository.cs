// =============================================================================
// DummyProgressEntryRepository.cs
// =============================================================================
// Hardcoded progress entry data. This is the entity that demonstrates the
// ownership model — entries have a CreatedByUserId, and the authorization
// handler uses it to determine who can edit/delete them.
//
// The seed data includes entries created by different users so you can test:
//   - Ms. Rivera (User 1) editing any entry for her students → ALLOWED
//   - Mr. Daniels (User 2) editing his own entry → ALLOWED
//   - Mr. Daniels (User 2) editing Ms. Rivera's entry → DENIED
// =============================================================================

using RolesAssignments.Models;

namespace RolesAssignments.Data;

public class DummyProgressEntryRepository : IProgressEntryRepository
{
    // Simulates the progress_entries table
    private static readonly List<ProgressEntry> _entries = new()
    {
        // Entry created by Ms. Rivera (User 1) for Student 101
        new ProgressEntry
        {
            Id = 1, StudentId = 101,
            Notes = "Student read 12 words per minute today. Showing improvement.",
            CreatedByUserId = 1,  // Ms. Rivera made this entry
            CreatedAt = new DateTime(2025, 10, 1),
            IsDeleted = false
        },

        // Entry created by Mr. Daniels (User 2) for Student 101
        new ProgressEntry
        {
            Id = 2, StudentId = 101,
            Notes = "Worked on addition facts during small group. Got 7/10 correct.",
            CreatedByUserId = 2,  // Mr. Daniels made this entry
            CreatedAt = new DateTime(2025, 10, 2),
            IsDeleted = false
        },

        // Another entry by Ms. Rivera for Student 102
        new ProgressEntry
        {
            Id = 3, StudentId = 102,
            Notes = "Student initiated conversation with peer during recess.",
            CreatedByUserId = 1,  // Ms. Rivera made this entry
            CreatedAt = new DateTime(2025, 10, 3),
            IsDeleted = false
        },
    };

    private static int _nextId = 4;

    public Task<IEnumerable<ProgressEntry>> GetByStudentId(int studentId)
    {
        var entries = _entries.Where(e => e.StudentId == studentId && !e.IsDeleted);

        // ==========================================
        // REAL DATABASE VERSION (commented out):
        // ==========================================
        // return await _db.QueryAsync<ProgressEntry>(
        //     @"SELECT id AS Id, student_id AS StudentId, notes AS Notes,
        //              created_by_user_id AS CreatedByUserId,
        //              created_at AS CreatedAt, is_deleted AS IsDeleted
        //       FROM progress_entries
        //       WHERE student_id = @StudentId AND is_deleted = FALSE
        //       ORDER BY created_at DESC",
        //     new { StudentId = studentId });

        return Task.FromResult(entries);
    }

    public Task<ProgressEntry?> GetById(int entryId)
    {
        var entry = _entries.FirstOrDefault(e => e.Id == entryId && !e.IsDeleted);
        return Task.FromResult(entry);
    }

    public Task<int> Create(int studentId, CreateEntryRequest request, int createdByUserId)
    {
        var entry = new ProgressEntry
        {
            Id = _nextId++,
            StudentId = studentId,
            Notes = request.Notes,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        _entries.Add(entry);

        // ==========================================
        // REAL DATABASE VERSION (commented out):
        // ==========================================
        // var entryId = await _db.ExecuteScalarAsync<int>(
        //     @"INSERT INTO progress_entries (student_id, notes, created_by_user_id, created_at, is_deleted)
        //       VALUES (@StudentId, @Notes, @CreatedByUserId, NOW(), FALSE);
        //       SELECT LAST_INSERT_ID();",
        //     new { StudentId = studentId, request.Notes, CreatedByUserId = createdByUserId });

        return Task.FromResult(entry.Id);
    }

    public Task Update(int entryId, UpdateEntryRequest request, int updatedByUserId)
    {
        var entry = _entries.FirstOrDefault(e => e.Id == entryId && !e.IsDeleted);
        if (entry is not null)
        {
            entry.Notes = request.Notes;
        }

        // ==========================================
        // REAL DATABASE VERSION (commented out):
        // ==========================================
        // await _db.ExecuteAsync(
        //     @"UPDATE progress_entries
        //       SET notes = @Notes, updated_by_user_id = @UpdatedByUserId, updated_at = NOW()
        //       WHERE id = @EntryId AND is_deleted = FALSE",
        //     new { EntryId = entryId, request.Notes, UpdatedByUserId = updatedByUserId });

        return Task.CompletedTask;
    }

    /// <summary>
    /// Soft-delete: marks the entry as deleted instead of removing it.
    /// The row stays in the database for auditing purposes.
    /// </summary>
    public Task SoftDelete(int entryId, int deletedByUserId)
    {
        var entry = _entries.FirstOrDefault(e => e.Id == entryId);
        if (entry is not null)
        {
            entry.IsDeleted = true;
        }

        // ==========================================
        // REAL DATABASE VERSION (commented out):
        // ==========================================
        // await _db.ExecuteAsync(
        //     @"UPDATE progress_entries
        //       SET is_deleted = TRUE,
        //           deleted_by_user_id = @DeletedByUserId,
        //           deleted_at = NOW()
        //       WHERE id = @EntryId",
        //     new { EntryId = entryId, DeletedByUserId = deletedByUserId });

        return Task.CompletedTask;
    }
}
