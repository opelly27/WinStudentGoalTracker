// =============================================================================
// StudentAssignment.cs
// =============================================================================
// This class represents a single row from the `student_assignments` table.
//
// It is THE MOST IMPORTANT piece of data in the entire authorization system.
// Every permission decision boils down to: "Does this user have an active
// assignment to this student, and if so, what type is it?"
//
// The corresponding database table looks like this:
//
//   CREATE TABLE student_assignments (
//       id INT PRIMARY KEY AUTO_INCREMENT,
//       user_id INT NOT NULL,
//       student_id INT NOT NULL,
//       assignment_type ENUM('PrimaryTeacher','Paraeducator','Supervisor','TemporaryCoverage') NOT NULL,
//       start_date DATE NOT NULL,
//       end_date DATE NULL,           -- NULL means "no end date" (ongoing)
//       is_active BOOLEAN NOT NULL DEFAULT TRUE,
//       FOREIGN KEY (user_id) REFERENCES users(id),
//       FOREIGN KEY (student_id) REFERENCES students(id)
//   );
// =============================================================================

namespace RolesAssignments.Models;

public class StudentAssignment
{
    public int Id { get; set; }

    // Which user is assigned
    public int UserId { get; set; }

    // Which student they're assigned to
    public int StudentId { get; set; }

    // What kind of assignment this is (see AssignmentType.cs for details)
    public AssignmentType AssignmentType { get; set; }

    // When this assignment started (e.g., the beginning of the school year)
    public DateTime StartDate { get; set; }

    // When this assignment ends. NULL means it's ongoing with no planned end date.
    public DateTime? EndDate { get; set; }

    // A quick on/off switch. If a student transfers classes, you can set this
    // to false instead of deleting the row — preserving the history.
    public bool IsActive { get; set; }
}
