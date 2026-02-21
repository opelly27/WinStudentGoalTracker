DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Goal_Update`(
    IN p_id_goal CHAR(36),
    IN p_id_goal_parent CHAR(36),
    IN p_id_student CHAR(36),
    IN p_id_user_created CHAR(36),
    IN p_title VARCHAR(255),
    IN p_description TEXT,
    IN p_category VARCHAR(100)
)
BEGIN
    UPDATE goal
    SET
        id_goal_parent = COALESCE(p_id_goal_parent, id_goal_parent),
        id_student = COALESCE(p_id_student, id_student),
        id_user_created = COALESCE(p_id_user_created, id_user_created),
        title = COALESCE(p_title, title),
        description = COALESCE(p_description, description),
        category = COALESCE(p_category, category),
        updated_at = UTC_TIMESTAMP()
    WHERE id_goal = p_id_goal;
    SELECT ROW_COUNT() AS rows_affected;
END;;
DELIMITER ;
