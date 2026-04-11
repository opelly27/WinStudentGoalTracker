DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Student_Delete`(IN p_id_student CHAR(36))
BEGIN
    -- Remove progress-event/benchmark associations
    DELETE peb FROM progress_event_benchmark peb
    INNER JOIN progress_event pe ON pe.id_progress_event = peb.id_progress_event
    INNER JOIN goal g ON g.id_goal = pe.id_goal
    WHERE g.id_student = p_id_student;
    -- Remove progress events
    DELETE pe FROM progress_event pe
    INNER JOIN goal g ON g.id_goal = pe.id_goal
    WHERE g.id_student = p_id_student;
    -- Remove benchmarks
    DELETE b FROM benchmark b
    INNER JOIN goal g ON g.id_goal = b.id_goal
    WHERE g.id_student = p_id_student;
    -- Remove goals
    DELETE FROM goal
    WHERE id_student = p_id_student;
    -- Remove user-student associations
    DELETE FROM user_student
    WHERE id_student = p_id_student;
    -- Remove the student
    DELETE FROM student
    WHERE id_student = p_id_student;
    SELECT ROW_COUNT() AS rows_affected;
END;;
DELIMITER ;
