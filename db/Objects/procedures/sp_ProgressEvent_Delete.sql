DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_ProgressEvent_Delete`(IN p_id_progress_event CHAR(36))
BEGIN
    DELETE FROM progress_event
    WHERE id_progress_event = p_id_progress_event;
    SELECT ROW_COUNT() AS rows_affected;
END;;
DELIMITER ;
