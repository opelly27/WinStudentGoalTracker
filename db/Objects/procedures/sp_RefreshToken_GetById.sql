DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_RefreshToken_GetById`(IN p_id_refresh_token INT)
BEGIN
    SELECT
        id_refresh_token,
        id_user,
        token_hash,
        token_salt,
        expires_at,
        last_used_at,
        revoked_at,
        device_info,
        user_agent,
        replaced_by_token_id,
        created_at,
        updated_at
    FROM refresh_token
    WHERE id_refresh_token = p_id_refresh_token
    LIMIT 1;
utf8mb4_0900_ai_ci;;
DELIMITER ;
