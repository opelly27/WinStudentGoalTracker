DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_RefreshToken_Create`(
    IN p_id_refresh_token CHAR(36),
    IN p_id_user CHAR(36),
    IN p_id_program CHAR(36),
    IN p_token_hash VARCHAR(512),
    IN p_token_salt VARCHAR(512),
    IN p_expires_in_seconds INT,
    IN p_device_info VARCHAR(255),
    IN p_user_agent VARCHAR(512)
)
BEGIN
    INSERT INTO refresh_token
    (
        id_refresh_token,
        id_user,
        id_program,
        token_hash,
        token_salt,
        expires_at,
        device_info,
        user_agent
    )
    VALUES
    (
        p_id_refresh_token,
        p_id_user,
        p_id_program,
        p_token_hash,
        p_token_salt,
        DATE_ADD(UTC_TIMESTAMP(), INTERVAL p_expires_in_seconds SECOND),
        p_device_info,
        p_user_agent
    );
    SELECT p_id_refresh_token AS id_refresh_token;
END;;
DELIMITER ;
