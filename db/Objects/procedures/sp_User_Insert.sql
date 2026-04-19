DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_User_Insert`(
    IN p_id_user CHAR(36),
    IN p_email VARCHAR(255),
    IN p_name VARCHAR(255),
    IN p_password_hash VARCHAR(255),
    IN p_password_salt VARCHAR(255)
)
BEGIN
    INSERT INTO `user`
    (
        id_user,
        email,
        name,
        password_hash,
        password_salt,
        password_updated_at,
        failed_login_attempts,
        created_at
    )
    VALUES
    (
        p_id_user,
        p_email,
        p_name,
        p_password_hash,
        p_password_salt,
        UTC_TIMESTAMP(),
        0,
        UTC_TIMESTAMP()
    );
    SELECT
        id_user,
        email,
        name,
        created_at
    FROM `user`
    WHERE id_user = p_id_user
    LIMIT 1;
END;;
DELIMITER ;
