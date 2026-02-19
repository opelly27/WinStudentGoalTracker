DROP PROCEDURE IF EXISTS sp_Student_GetById;
DELIMITER $$

CREATE PROCEDURE sp_Student_GetById(IN p_id_student INT)
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
    WHERE id_student = p_id_student
    LIMIT 1;
END$$

DELIMITER ;
