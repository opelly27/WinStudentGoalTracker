DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Benchmark_Delete`(IN p_id_benchmark CHAR(36))
BEGIN
    -- Remove progress-event/benchmark associations
    DELETE FROM progress_event_benchmark
    WHERE id_benchmark = p_id_benchmark;
    -- Remove the benchmark itself
    DELETE FROM benchmark
    WHERE id_benchmark = p_id_benchmark;
    SELECT ROW_COUNT() AS rows_affected;
END;;
DELIMITER ;
