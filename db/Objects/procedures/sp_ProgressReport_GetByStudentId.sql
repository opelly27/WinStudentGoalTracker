DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_ProgressReport_GetByStudentId`(
    IN p_id_student CHAR(36),
    IN p_from_date DATE,
    IN p_to_date DATE,
    IN p_goal_ids TEXT
)
BEGIN
    -- Result set 1: Goals that have at least one progress event in the date range
    SELECT
        g.`id_goal`         AS `goalId`,
        g.`category`        AS `category`,
        g.`description`     AS `description`
    FROM `goal` g
    WHERE g.`id_student` = p_id_student
      AND (p_goal_ids IS NULL OR FIND_IN_SET(g.`id_goal`, p_goal_ids))
      AND EXISTS (
          SELECT 1 FROM `progress_event` pe
          WHERE pe.`id_goal` = g.`id_goal`
            AND DATE(pe.`created_at`) >= p_from_date
            AND DATE(pe.`created_at`) <= p_to_date
      )
    ORDER BY g.`category`;

    -- Result set 2: Progress events within the date range, with benchmark names
    SELECT
        pe.`id_goal`            AS `goalId`,
        pe.`id_progress_event`  AS `progressEventId`,
        pe.`content`            AS `content`,
        pe.`created_at`         AS `createdAt`,
        GROUP_CONCAT(
            COALESCE(b.`short_name`, b.`benchmark`)
            ORDER BY b.`short_name`, b.`benchmark`
            SEPARATOR ', '
        ) AS `benchmarkNames`
    FROM `progress_event` pe
    INNER JOIN `goal` g ON g.`id_goal` = pe.`id_goal`
    LEFT JOIN `progress_event_benchmark` peb ON peb.`id_progress_event` = pe.`id_progress_event`
    LEFT JOIN `benchmark` b ON b.`id_benchmark` = peb.`id_benchmark`
    WHERE g.`id_student` = p_id_student
      AND (p_goal_ids IS NULL OR FIND_IN_SET(g.`id_goal`, p_goal_ids))
      AND DATE(pe.`created_at`) >= p_from_date
      AND DATE(pe.`created_at`) <= p_to_date
    GROUP BY pe.`id_progress_event`, pe.`id_goal`, pe.`content`, pe.`created_at`
    ORDER BY pe.`created_at` ASC;
END;;
DELIMITER ;
