DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_User_GetById`(IN p_id_user CHAR(36))
BEGIN
    SELECT
        u.id_user,
        u.email,
        u.name,
        u.password_hash,
        u.password_salt,
        u.failed_login_attempts,
        u.locked_until,
        u.created_at
    FROM `user` u
    WHERE u.id_user = p_id_user
    LIMIT 1;
END;;
DELIMITER ;
