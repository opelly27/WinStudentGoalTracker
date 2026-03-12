DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Student_GetAll`()
BEGIN
    SELECT
        studentId,
        identifier,
        nextIepDate,
        lastEntryDate,
        goalCount,
        progressEventCount
    FROM v_student_card
    ORDER BY studentId;
END;;
DELIMITER ;
