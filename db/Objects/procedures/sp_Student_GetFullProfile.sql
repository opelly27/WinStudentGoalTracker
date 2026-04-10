DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Student_GetFullProfile`(IN p_id_student CHAR(36))
BEGIN
    -- Result set 1: Student card
    SELECT
        studentId,
        identifier,
        nextIepDate,
        firstEntryDate,
        lastEntryDate,
        goalCount,
        progressEventCount,
        benchmarkCount
    FROM v_student_card
    WHERE studentId = p_id_student
    LIMIT 1;
    -- Result set 2: Goals
    SELECT
        s.`identifier`              AS `studentIdentifier`,
        vc.`goalId`,
        vc.`goalParentId`,
        vc.`description`,
        vc.`category`,
        vc.`baseline`,
        vc.`targetCompletionDate`,
        vc.`closeDate`,
        vc.`achieved`,
        vc.`closeNotes`,
        vc.`progressEventCount`,
        vc.`benchmarkCount`
    FROM `v_goal_card` vc
    INNER JOIN `student` s ON s.`id_student` = vc.`studentId`
    WHERE vc.`studentId` = p_id_student
    ORDER BY vc.`goalId`;
    -- Result set 3: Benchmarks
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
    -- Result set 4: Progress events (all goals for this student)
    SELECT
        vc.`progressEventId`,
        vc.`goalId`,
        vc.`content`,
        vc.`createdAt`,
        vc.`createdByName`
    FROM `v_progress_event_card` vc
    WHERE vc.`studentId` = p_id_student
    ORDER BY vc.`createdAt` DESC;
    -- Result set 5: Benchmark/progress-event associations
    SELECT
        peb.`id_progress_event` AS `progressEventId`,
        peb.`id_benchmark`      AS `benchmarkId`
    FROM `progress_event_benchmark` peb
    INNER JOIN `progress_event` pe ON pe.`id_progress_event` = peb.`id_progress_event`
    INNER JOIN `goal` g ON g.`id_goal` = pe.`id_goal`
    WHERE g.`id_student` = p_id_student;
END;;
DELIMITER ;
