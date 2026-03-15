DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Goal_GetById`(IN p_id_goal CHAR(36))
BEGIN
    SELECT
        goalId,
        goalParentId,
        studentId,
        description,
        category,
        baseline,
        targetCompletionDate,
        closeDate,
        achieved,
        closeNotes,
        progressEventCount
    FROM v_goal_card
    WHERE goalId = p_id_goal
    LIMIT 1;
END;;
DELIMITER ;
