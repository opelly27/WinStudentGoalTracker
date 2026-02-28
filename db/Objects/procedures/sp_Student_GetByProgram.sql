DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Student_GetByProgram`(
    IN p_id_program CHAR(36)
)
BEGIN
    SELECT
        id_student,
        id_program,
        identifier,
        program_year,
        enrollment_date,
        expected_grad,
        created_at
    FROM student
    WHERE id_program = p_id_program
    ORDER BY id_student;
END;;
DELIMITER ;
