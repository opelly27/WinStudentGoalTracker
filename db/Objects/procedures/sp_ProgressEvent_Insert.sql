DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_ProgressEvent_Insert`(
    IN p_id_progress_event CHAR(36),
    IN p_id_student CHAR(36),
    IN p_id_goal CHAR(36),
    IN p_id_user_created CHAR(36),
    IN p_content TEXT,
    IN p_is_sensitive TINYINT(1)
)
BEGIN
    INSERT INTO progress_event
    (
        id_progress_event,
        id_student,
        id_goal,
        id_user_created,
        content,
        is_sensitive,
        created_at,
        updated_at
    )
    VALUES
    (
        p_id_progress_event,
        p_id_student,
        p_id_goal,
        p_id_user_created,
        p_content,
        p_is_sensitive,
        UTC_TIMESTAMP(),
        UTC_TIMESTAMP()
    );
    SELECT
        id_progress_event,
        id_student,
        id_goal,
        id_user_created,
        content,
        is_sensitive,
        created_at,
        updated_at
    FROM progress_event
    WHERE id_progress_event = p_id_progress_event
    LIMIT 1;
END;;
DELIMITER ;
