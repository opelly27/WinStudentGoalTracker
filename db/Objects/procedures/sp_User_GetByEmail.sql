DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_User_GetByEmail`(IN p_email VARCHAR(255))
BEGIN
    SELECT
        u.id_user,
        u.id_role,
        u.email,
        u.name,
        u.password_hash,
        u.password_salt,
        u.failed_login_attempts,
        u.locked_until,
        u.created_at,
        r.internal_name AS role_internal_name,
        r.name AS role_display_name
    FROM `user` u
    LEFT JOIN role r ON u.id_role = r.id_role
    WHERE u.email = p_email
    LIMIT 1;
END;;
DELIMITER ;
