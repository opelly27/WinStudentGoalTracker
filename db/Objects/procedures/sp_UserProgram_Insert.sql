DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_UserProgram_Insert`(
    IN p_id_user_program CHAR(36),
    IN p_id_user CHAR(36),
    IN p_id_program CHAR(36),
    IN p_id_role CHAR(36),
    IN p_is_primary TINYINT
)
BEGIN
    INSERT INTO user_program
    (
        id_user_program,
        id_user,
        id_program,
        id_role,
        is_primary,
        status,
        joined_at
    )
    VALUES
    (
        p_id_user_program,
        p_id_user,
        p_id_program,
        p_id_role,
        p_is_primary,
        'active',
        UTC_TIMESTAMP()
    );
    SELECT ROW_COUNT() AS rows_affected;
END;;
DELIMITER ;
