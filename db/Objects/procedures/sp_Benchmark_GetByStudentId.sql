DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Benchmark_GetByStudentId`(IN p_id_student CHAR(36))
BEGIN
    SELECT
        s.`identifier`          AS `studentIdentifier`,
        b.`id_benchmark`        AS `benchmarkId`,
        b.`id_goal`             AS `goalId`,
        g.`category`            AS `goalCategory`,
        b.`benchmark`           AS `benchmark`,
        u.`name`                AS `createdByName`,
        b.`created_at`          AS `createdAt`,
        b.`updated_at`          AS `updatedAt`
    FROM `benchmark` b
    INNER JOIN `goal` g ON g.`id_goal` = b.`id_goal`
    INNER JOIN `student` s ON s.`id_student` = g.`id_student`
    LEFT JOIN `user` u ON u.`id_user` = b.`id_user_created`
    WHERE g.`id_student` = p_id_student
    ORDER BY b.`created_at` DESC;
END;;
DELIMITER ;
