DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Student_GetWithAssignments`(
    IN p_id_program CHAR(36),
    IN p_id_user CHAR(36)
)
BEGIN
    -- Result set 1: All students in the program (card shape)
    SELECT
        vc.studentId,
        vc.identifier,
        vc.nextIepDate,
        vc.lastEntryDate,
        vc.goalCount,
        vc.progressEventCount
    FROM v_student_card vc
    INNER JOIN student s ON s.id_student = vc.studentId
    WHERE s.id_program = p_id_program
    ORDER BY vc.studentId;
    -- Result set 2: user_student assignments for the requesting user in this program
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
