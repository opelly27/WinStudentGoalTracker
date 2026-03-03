CREATE OR REPLACE VIEW `v_goal_card` AS
SELECT
    goal.`id_goal`                          AS `goalId`,
    goal.`id_goal_parent`                   AS `goalParentId`,
    goal.`id_student`                       AS `studentId`,
    goal.`title`                            AS `title`,
    goal.`description`                      AS `description`,
    goal.`category`                         AS `category`,
    COUNT(pe.`id_progress_event`)           AS `progressEventCount`
FROM `goal`
LEFT JOIN `progress_event` pe ON pe.`id_goal` = goal.`id_goal`
GROUP BY
    goal.`id_goal`,
    goal.`id_goal_parent`,
    goal.`id_student`,
    goal.`title`,
    goal.`description`,
    goal.`category`;
