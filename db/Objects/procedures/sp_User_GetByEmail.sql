DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_User_GetByEmail`(IN p_email VARCHAR(255))
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
    WHERE u.email = p_email
    LIMIT 1;
END;;
DELIMITER ;
