DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_ReportPrompt_GetById`(
    IN p_id_report_prompt CHAR(36)
)
BEGIN
    SELECT
        `id_ReportPrompt`   AS `reportPromptId`,
        `id_program`        AS `programId`,
        `prompt`            AS `prompt`,
        `reportname`        AS `reportname`
    FROM `ReportPrompt`
    WHERE `id_ReportPrompt` = p_id_report_prompt
    LIMIT 1;
END;;
DELIMITER ;
