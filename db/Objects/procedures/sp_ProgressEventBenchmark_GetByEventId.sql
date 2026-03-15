DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_ProgressEventBenchmark_GetByEventId`(
    IN p_id_progress_event CHAR(36)
)
BEGIN
    SELECT
        peb.id_benchmark AS benchmarkId
    FROM progress_event_benchmark peb
    WHERE peb.id_progress_event = p_id_progress_event;
END;;
DELIMITER ;
