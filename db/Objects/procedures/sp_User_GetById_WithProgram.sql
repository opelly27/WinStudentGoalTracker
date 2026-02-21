DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_User_GetById_WithProgram`(
    IN p_id_user CHAR(36),
    IN p_id_program CHAR(36)
)
BEGIN
    SELECT
        u.id_user,
        u.email,
        u.name,
        up.id_program,
        p.name AS program_name,
        r.internal_name AS role_internal_name,
        r.name AS role_display_name,
        up.status
    FROM `user` u
    JOIN user_program up ON u.id_user = up.id_user AND up.id_program = p_id_program
    JOIN role r ON up.id_role = r.id_role
    JOIN program p ON up.id_program = p.id_program
    WHERE u.id_user = p_id_user
    LIMIT 1;
END;;
DELIMITER ;
