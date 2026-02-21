DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_ProgressEvent_Update`(
    IN p_id_progress_event CHAR(36),
    IN p_id_student CHAR(36),
    IN p_id_goal CHAR(36),
    IN p_id_user_created CHAR(36),
    IN p_content TEXT,
    IN p_is_sensitive TINYINT(1)
)
BEGIN
    UPDATE progress_event
    SET
        id_student = COALESCE(p_id_student, id_student),
        id_goal = COALESCE(p_id_goal, id_goal),
        id_user_created = COALESCE(p_id_user_created, id_user_created),
        content = COALESCE(p_content, content),
        is_sensitive = COALESCE(p_is_sensitive, is_sensitive),
        updated_at = UTC_TIMESTAMP()
    WHERE id_progress_event = p_id_progress_event;
    SELECT ROW_COUNT() AS rows_affected;
END;;
DELIMITER ;
