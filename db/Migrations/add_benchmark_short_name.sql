-- =====================================================================
-- Migration: Add short_name column to benchmark table and update
-- all benchmark stored procedures to support it.
-- Run in TablePlus against MySQL.
-- =====================================================================

-- 1. Add the column
ALTER TABLE `benchmark`
ADD COLUMN `short_name` VARCHAR(50) DEFAULT NULL AFTER `benchmark`;


-- 2. Recreate sp_Benchmark_GetByStudentId
DROP PROCEDURE IF EXISTS `sp_Benchmark_GetByStudentId`;
CREATE PROCEDURE `sp_Benchmark_GetByStudentId`(IN p_id_student CHAR(36))
BEGIN
    SELECT
        s.`identifier`          AS `studentIdentifier`,
        b.`id_benchmark`        AS `benchmarkId`,
        b.`id_goal`             AS `goalId`,
        g.`category`            AS `goalCategory`,
        b.`benchmark`           AS `benchmark`,
        b.`short_name`          AS `shortName`,
        u.`name`                AS `createdByName`,
        b.`created_at`          AS `createdAt`,
        b.`updated_at`          AS `updatedAt`
    FROM `benchmark` b
    INNER JOIN `goal` g ON g.`id_goal` = b.`id_goal`
    INNER JOIN `student` s ON s.`id_student` = g.`id_student`
    LEFT JOIN `user` u ON u.`id_user` = b.`id_user_created`
    WHERE g.`id_student` = p_id_student
    ORDER BY b.`created_at` DESC;
END;


-- 3. Recreate sp_Benchmark_Insert
DROP PROCEDURE IF EXISTS `sp_Benchmark_Insert`;
CREATE PROCEDURE `sp_Benchmark_Insert`(
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
END;


-- 4. Recreate sp_Benchmark_Update
DROP PROCEDURE IF EXISTS `sp_Benchmark_Update`;
CREATE PROCEDURE `sp_Benchmark_Update`(
    IN p_id_benchmark CHAR(36),
    IN p_benchmark TEXT,
    IN p_short_name VARCHAR(50)
)
BEGIN
    UPDATE benchmark
    SET
        benchmark = p_benchmark,
        short_name = p_short_name,
        updated_at = UTC_TIMESTAMP()
    WHERE id_benchmark = p_id_benchmark;
    SELECT ROW_COUNT() AS rowsAffected;
END;
