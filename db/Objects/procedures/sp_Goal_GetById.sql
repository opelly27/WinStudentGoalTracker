DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Goal_GetById`(IN p_id_goal CHAR(36))
BEGIN
    SELECT
        id_goal,
        id_goal_parent,
        id_student,
        id_user_created,
        title,
        description,
        category,
        created_at,
        updated_at
    FROM goal
    WHERE id_goal = p_id_goal
    LIMIT 1;
END;;
DELIMITER ;
