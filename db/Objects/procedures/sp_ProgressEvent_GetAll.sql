DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_ProgressEvent_GetAll`()
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
    ORDER BY id_progress_event;
END;;
DELIMITER ;
