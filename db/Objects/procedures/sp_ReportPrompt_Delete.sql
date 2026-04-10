DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_ReportPrompt_Delete`(
    IN p_id_report_prompt CHAR(36)
)
BEGIN
    DELETE FROM `ReportPrompt`
    WHERE `id_report_prompt` = p_id_report_prompt;
    SELECT ROW_COUNT() AS rowsAffected;
END;;
DELIMITER ;
