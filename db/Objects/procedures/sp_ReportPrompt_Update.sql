DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_ReportPrompt_Update`(
    IN p_id_report_prompt CHAR(36),
    IN p_prompt TEXT,
    IN p_reportname CHAR(100)
)
BEGIN
    UPDATE `ReportPrompt`
    SET
        `prompt` = COALESCE(p_prompt, `prompt`),
        `reportname` = COALESCE(p_reportname, `reportname`)
    WHERE `id_ReportPrompt` = p_id_report_prompt;
    SELECT ROW_COUNT() AS rowsAffected;
END;;
DELIMITER ;
