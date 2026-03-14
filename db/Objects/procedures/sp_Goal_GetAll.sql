DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Goal_GetAll`()
BEGIN
    SELECT
        goalId,
        goalParentId,
        studentId,
        description,
        category,
        baseline,
        progressEventCount
    FROM v_goal_card
    ORDER BY goalId;
END;;
DELIMITER ;
