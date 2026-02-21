DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Goal_GetAll`()
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
    ORDER BY id_goal;
END;;
DELIMITER ;
