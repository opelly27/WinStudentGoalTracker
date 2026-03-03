DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Goal_GetAll`()
BEGIN
    SELECT
        goalId,
        goalParentId,
        studentId,
        title,
        description,
        category,
        progressEventCount
    FROM v_goal_card
    ORDER BY goalId;
END;;
DELIMITER ;
