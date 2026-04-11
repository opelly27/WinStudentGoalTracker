DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Goal_Delete`(IN p_id_goal CHAR(36))
BEGIN
    -- Remove benchmark/event associations for progress events under this goal
    DELETE peb FROM progress_event_benchmark peb
    INNER JOIN progress_event pe ON pe.id_progress_event = peb.id_progress_event
    WHERE pe.id_goal = p_id_goal;
    -- Remove progress events under this goal
    DELETE FROM progress_event
    WHERE id_goal = p_id_goal;
    -- Remove benchmarks under this goal
    DELETE FROM benchmark
    WHERE id_goal = p_id_goal;
    -- Remove child goals (one level)
    DELETE FROM goal
    WHERE id_goal_parent = p_id_goal;
    -- Remove the goal itself
    DELETE FROM goal
    WHERE id_goal = p_id_goal;
    SELECT ROW_COUNT() AS rows_affected;
END;;
DELIMITER ;
