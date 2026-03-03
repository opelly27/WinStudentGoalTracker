CREATE OR REPLACE VIEW `v_student_card` AS
SELECT
    s.`id_student`                          AS `studentId`,
    s.`identifier`                          AS `identifier`,
    s.`expected_grad`                       AS `expectedGradDate`,
    MAX(pe.`created_at`)                    AS `lastEntryDate`,
    COUNT(DISTINCT g.`id_goal`)             AS `goalCount`,
    COUNT(DISTINCT pe.`id_progress_event`)  AS `progressEventCount`
FROM `student` s
LEFT JOIN `goal` g
    ON g.`id_student` = s.`id_student`
LEFT JOIN `progress_event` pe
    ON pe.`id_goal` = g.`id_goal`
GROUP BY
    s.`id_student`,
    s.`identifier`,
    s.`expected_grad`;
