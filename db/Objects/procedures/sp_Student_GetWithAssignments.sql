DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Student_GetWithAssignments`(
    IN p_id_program CHAR(36),
    IN p_id_user CHAR(36)
)
BEGIN
    SELECT
        vc.studentId,
        vc.identifier,
        vc.nextIepDate,
        vc.lastEntryDate,
        vc.goalCount,
        vc.progressEventCount,
        vc.benchmarkCount
    FROM v_student_card vc
    INNER JOIN student s ON s.id_student = vc.studentId
    WHERE s.id_program = p_id_program
    ORDER BY vc.studentId;
    SELECT
        us.id_user_student,
        us.id_user,
        us.id_student,
        us.is_primary
    FROM user_student us
    INNER JOIN student s ON s.id_student = us.id_student
    WHERE us.id_user = p_id_user
      AND s.id_program = p_id_program;
END;;
DELIMITER ;
