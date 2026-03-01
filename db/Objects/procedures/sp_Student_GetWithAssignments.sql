DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Student_GetWithAssignments`(
    IN p_id_program CHAR(36),
    IN p_id_user CHAR(36)
)
BEGIN
    -- Result set 1: All students in the program
    SELECT
        s.id_student,
        s.id_program,
        s.identifier,
        s.program_year,
        s.enrollment_date,
        s.expected_grad,
        s.created_at
    FROM student s
    WHERE s.id_program = p_id_program
    ORDER BY s.id_student;
    -- Result set 2: user_student assignments for the requesting user in this program
    SELECT
        us.id_user_student,
        us.id_user,
        us.id_student,
        us.is_primary
    FROM user_student us
    INNER JOIN student s ON s.id_student = us.id_student
    WHERE us.id_user = p_id_user
      AND s.id_program = p_id_program;
END;;
DELIMITER ;
