DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_ProgressEvent_GetById`(IN p_id_progress_event CHAR(36))
BEGIN
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
