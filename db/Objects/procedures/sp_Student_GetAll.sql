DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Student_GetAll`()
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
    ORDER BY id_student;
utf8mb4_0900_ai_ci;;
DELIMITER ;
