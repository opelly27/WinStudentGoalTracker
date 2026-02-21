DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Goal_Insert`(
    IN p_id_goal CHAR(36),
    IN p_id_goal_parent CHAR(36),
    IN p_id_student CHAR(36),
    IN p_id_user_created CHAR(36),
    IN p_title VARCHAR(255),
    IN p_description TEXT,
    IN p_category VARCHAR(100)
)
BEGIN
    INSERT INTO goal
    (
        id_goal,
        id_goal_parent,
        id_student,
        id_user_created,
        title,
        description,
        category,
        created_at,
        updated_at
    )
    VALUES
    (
        p_id_goal,
        p_id_goal_parent,
        p_id_student,
        p_id_user_created,
        p_title,
        p_description,
        p_category,
        UTC_TIMESTAMP(),
        UTC_TIMESTAMP()
    );
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
