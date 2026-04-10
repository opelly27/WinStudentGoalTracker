DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_ReportPrompt_Update`(
    IN p_id_report_prompt CHAR(36),
    IN p_prompt TEXT,
    IN p_reportname CHAR(100)
)
BEGIN
    UPDATE `ReportPrompt`
    SET
        `prompt` = p_prompt,
        `reportname` = p_reportname
    WHERE `id_report_prompt` = p_id_report_prompt;
    SELECT ROW_COUNT() AS rowsAffected;
END;;
DELIMITER ;
