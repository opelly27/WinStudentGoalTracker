DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Student_GetById`(IN p_id_student CHAR(36))
BEGIN
    SELECT
        studentId,
        identifier,
        nextIepDate,
        lastEntryDate,
        goalCount,
        progressEventCount
    FROM v_student_card
    WHERE studentId = p_id_student
    LIMIT 1;
END;;
DELIMITER ;
