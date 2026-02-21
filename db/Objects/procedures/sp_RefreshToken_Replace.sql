DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_RefreshToken_Replace`(
    IN p_old_token_id INT,
    IN p_id_user INT,
    IN p_token_hash VARCHAR(512),
    IN p_token_salt VARCHAR(512),
    IN p_expires_in_seconds INT,
    IN p_device_info VARCHAR(255),
    IN p_user_agent VARCHAR(512)
)
BEGIN
    -- Revoke the old token
    UPDATE refresh_token
    SET revoked_at = UTC_TIMESTAMP()
    WHERE id_refresh_token = p_old_token_id
      AND revoked_at IS NULL;
    -- Create the new token
    INSERT INTO refresh_token
    (
        id_user,
        token_hash,
        token_salt,
        expires_at,
        device_info,
        user_agent
    )
    VALUES
    (
        p_id_user,
        p_token_hash,
        p_token_salt,
        DATE_ADD(UTC_TIMESTAMP(), INTERVAL p_expires_in_seconds SECOND),
        p_device_info,
        p_user_agent
    );
    -- Link old token to new one
    SET @new_id = LAST_INSERT_ID();
    UPDATE refresh_token
    SET replaced_by_token_id = @new_id
    WHERE id_refresh_token = p_old_token_id;
    SELECT @new_id AS id_refresh_token;
utf8mb4_0900_ai_ci;;
DELIMITER ;
