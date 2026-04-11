DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_ProgressEvent_Delete`(IN p_id_progress_event CHAR(36))
BEGIN
    -- Remove benchmark associations
    DELETE FROM progress_event_benchmark
    WHERE id_progress_event = p_id_progress_event;
    -- Remove the progress event itself
    DELETE FROM progress_event
    WHERE id_progress_event = p_id_progress_event;
    SELECT ROW_COUNT() AS rows_affected;
END;;
DELIMITER ;
