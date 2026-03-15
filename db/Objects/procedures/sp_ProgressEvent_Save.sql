DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_ProgressEvent_Save`(
    IN p_id_progress_event CHAR(36),
    IN p_id_goal CHAR(36),
    IN p_id_user_created CHAR(36),
    IN p_content TEXT,
    IN p_is_sensitive TINYINT(1),
    IN p_is_new TINYINT(1),
    IN p_benchmark_ids TEXT
)
BEGIN
    -- Insert or update the progress event
    IF p_is_new = 1 THEN
        INSERT INTO progress_event
        (
            id_progress_event,
            id_goal,
            id_user_created,
            content,
            is_sensitive,
            created_at,
            updated_at
        )
        VALUES
        (
            p_id_progress_event,
            p_id_goal,
            p_id_user_created,
            p_content,
            p_is_sensitive,
            UTC_TIMESTAMP(),
            UTC_TIMESTAMP()
        );
    ELSE
        UPDATE progress_event
        SET
            content = COALESCE(p_content, content),
            updated_at = UTC_TIMESTAMP()
        WHERE id_progress_event = p_id_progress_event;
    END IF;
    -- Sync benchmark associations: remove those not in the list
    DELETE FROM progress_event_benchmark
    WHERE id_progress_event = p_id_progress_event
      AND (p_benchmark_ids IS NULL
           OR LENGTH(TRIM(p_benchmark_ids)) = 0
           OR FIND_IN_SET(id_benchmark, p_benchmark_ids) = 0);
    -- Add associations that are in the list but not yet in the table
    IF p_benchmark_ids IS NOT NULL AND LENGTH(TRIM(p_benchmark_ids)) > 0 THEN
        INSERT INTO progress_event_benchmark (id_progress_event_benchmark, id_progress_event, id_benchmark, created_at)
        SELECT UUID(), p_id_progress_event, b.id_benchmark, UTC_TIMESTAMP()
        FROM benchmark b
        WHERE FIND_IN_SET(b.id_benchmark, p_benchmark_ids) > 0
          AND b.id_benchmark NOT IN (
              SELECT peb.id_benchmark
              FROM progress_event_benchmark peb
              WHERE peb.id_progress_event = p_id_progress_event
          );
    END IF;
    -- Return the progress event ID for the caller
    SELECT p_id_progress_event AS progressEventId;
END;;
DELIMITER ;
