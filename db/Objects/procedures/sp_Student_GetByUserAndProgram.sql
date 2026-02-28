DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Student_GetByUserAndProgram`(
    IN p_id_user CHAR(36),
    IN p_id_program CHAR(36)
)
BEGIN
    SELECT
        s.id_student,
        s.id_program,
        s.id_user,
        s.identifier,
        s.program_year,
        s.enrollment_date,
        s.expected_grad,
        s.created_at
    FROM student s
    WHERE s.id_user = p_id_user
      AND s.id_program = p_id_program
    ORDER BY s.id_student;
END;;
DELIMITER ;
