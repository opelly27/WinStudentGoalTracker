DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Student_Insert`(
    IN p_id_student CHAR(36),
    IN p_id_program CHAR(36),
    IN p_id_user CHAR(36),
    IN p_identifier VARCHAR(50),
    IN p_program_year INT,
    IN p_enrollment_date DATE,
    IN p_expected_grad DATE
)
BEGIN
    INSERT INTO student
    (
        id_student,
        id_program,
        identifier,
        program_year,
        enrollment_date,
        expected_grad,
        created_at
    )
    VALUES
    (
        p_id_student,
        p_id_program,
        p_identifier,
        p_program_year,
        p_enrollment_date,
        p_expected_grad,
        UTC_TIMESTAMP()
    );
    INSERT INTO user_student (id_user_student, id_user, id_student, is_primary)
    VALUES (UUID(), p_id_user, p_id_student, 1);
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
END;;
DELIMITER ;
