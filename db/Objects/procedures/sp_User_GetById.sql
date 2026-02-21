DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_User_GetById`(IN p_id_user INT)
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
        r.name AS role_name
    FROM `user` u
    LEFT JOIN role r ON u.id_role = r.id_role
    WHERE u.id_user = p_id_user
    LIMIT 1;
utf8mb4_0900_ai_ci;;
DELIMITER ;
