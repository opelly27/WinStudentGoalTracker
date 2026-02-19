DROP PROCEDURE IF EXISTS sp_Student_Delete;
DELIMITER $$

CREATE PROCEDURE sp_Student_Delete(IN p_id_student INT)
BEGIN
    DELETE FROM student
    WHERE id_student = p_id_student;

    SELECT ROW_COUNT() AS rows_affected;
END$$

DELIMITER ;
