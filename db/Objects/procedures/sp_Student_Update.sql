DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Student_Update`(
    IN p_id_student CHAR(36),
    IN p_identifier VARCHAR(50),
    IN p_program_year INT,
    IN p_enrollment_date DATE,
    IN p_next_iep_date DATE
)
BEGIN
    UPDATE student
    SET
        identifier = COALESCE(p_identifier, identifier),
        program_year = COALESCE(p_program_year, program_year),
        enrollment_date = COALESCE(p_enrollment_date, enrollment_date),
        next_iep_date = COALESCE(p_next_iep_date, next_iep_date)
    WHERE id_student = p_id_student;
    SELECT ROW_COUNT() AS rows_affected;
END;;
DELIMITER ;
