DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_RefreshToken_Revoke`(IN p_id_refresh_token INT)
BEGIN
    UPDATE refresh_token
    SET revoked_at = UTC_TIMESTAMP()
    WHERE id_refresh_token = p_id_refresh_token
      AND revoked_at IS NULL;
    SELECT ROW_COUNT() AS rows_affected;
utf8mb4_0900_ai_ci;;
DELIMITER ;
