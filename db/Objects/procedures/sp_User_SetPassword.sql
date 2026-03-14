DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_User_SetPassword`(
    IN p_id_user CHAR(36),
    IN p_password_hash VARCHAR(255),
    IN p_password_salt VARCHAR(255)
)
BEGIN
    UPDATE user
    SET password_hash = p_password_hash,
        password_salt = p_password_salt,
        password_updated_at = UTC_TIMESTAMP()
    WHERE id_user = p_id_user;
    SELECT ROW_COUNT() AS rows_affected;
END;;
DELIMITER ;
