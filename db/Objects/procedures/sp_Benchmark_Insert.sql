DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Benchmark_Insert`(
    IN p_id_benchmark CHAR(36),
    IN p_id_goal CHAR(36),
    IN p_id_user_created CHAR(36),
    IN p_benchmark TEXT,
    IN p_short_name VARCHAR(50)
)
BEGIN
    INSERT INTO benchmark
    (
        id_benchmark,
        id_goal,
        id_user_created,
        benchmark,
        short_name,
        created_at,
        updated_at
    )
    VALUES
    (
        p_id_benchmark,
        p_id_goal,
        p_id_user_created,
        p_benchmark,
        p_short_name,
        UTC_TIMESTAMP(),
        NULL
    );
    SELECT
        id_benchmark,
        id_goal,
        id_user_created,
        benchmark,
        short_name,
        created_at,
        updated_at
    FROM benchmark
    WHERE id_benchmark = p_id_benchmark
    LIMIT 1;
END;;
DELIMITER ;
