DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Goal_Update`(
    IN p_id_goal CHAR(36),
    IN p_id_goal_parent CHAR(36),
    IN p_id_student CHAR(36),
    IN p_id_user_created CHAR(36),
    IN p_description TEXT,
    IN p_category VARCHAR(255),
    IN p_baseline TEXT,
    IN p_target_completion_date DATE,
    IN p_close_date DATE,
    IN p_achieved TINYINT(1),
    IN p_close_notes TEXT
)
BEGIN
    UPDATE goal
    SET
        id_goal_parent = COALESCE(p_id_goal_parent, id_goal_parent),
        id_student = COALESCE(p_id_student, id_student),
        id_user_created = COALESCE(p_id_user_created, id_user_created),
        description = COALESCE(p_description, description),
        category = COALESCE(p_category, category),
        baseline = COALESCE(p_baseline, baseline),
        target_completion_date = COALESCE(p_target_completion_date, target_completion_date),
        close_date = COALESCE(p_close_date, close_date),
        achieved = COALESCE(p_achieved, achieved),
        close_notes = COALESCE(p_close_notes, close_notes),
        updated_at = UTC_TIMESTAMP()
    WHERE id_goal = p_id_goal;
    SELECT ROW_COUNT() AS rows_affected;
END;;
DELIMITER ;
