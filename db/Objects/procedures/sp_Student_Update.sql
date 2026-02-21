DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Student_Update`(
    IN p_id_student INT,
    IN p_id_program INT,
    IN p_identifier VARCHAR(50),
    IN p_program_year INT,
    IN p_enrollment_date DATE,
    IN p_expected_grad DATE
)
BEGIN
    UPDATE student
    SET
        id_program = COALESCE(p_id_program, id_program),
        identifier = COALESCE(p_identifier, identifier),
        program_year = COALESCE(p_program_year, program_year),
        enrollment_date = COALESCE(p_enrollment_date, enrollment_date),
        expected_grad = COALESCE(p_expected_grad, expected_grad)
    WHERE id_student = p_id_student;
    SELECT ROW_COUNT() AS rows_affected;
utf8mb4_0900_ai_ci;;
DELIMITER ;
