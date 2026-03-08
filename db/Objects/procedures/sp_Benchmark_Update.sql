DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Benchmark_Update`(
    IN p_id_benchmark CHAR(36),
    IN p_benchmark TEXT
)
BEGIN
    UPDATE benchmark
    SET
        benchmark = p_benchmark,
        updated_at = UTC_TIMESTAMP()
    WHERE id_benchmark = p_id_benchmark;
    SELECT ROW_COUNT() AS rowsAffected;
END;;
DELIMITER ;
