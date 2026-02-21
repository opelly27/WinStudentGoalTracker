DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Student_Delete`(IN p_id_student CHAR(36))
BEGIN
    DELETE FROM student
    WHERE id_student = p_id_student;
    SELECT ROW_COUNT() AS rows_affected;
END;;
DELIMITER ;
