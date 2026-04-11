DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_ReportPrompt_GetByReportname`(
    IN p_reportname CHAR(100),
    IN p_id_program CHAR(36)
)
BEGIN
    SELECT
        `id_ReportPrompt`   AS `reportPromptId`,
        `id_program`        AS `programId`,
        `prompt`            AS `prompt`,
        `reportname`        AS `reportname`
    FROM `ReportPrompt`
    WHERE `reportname` = p_reportname
      AND `id_program` = p_id_program
    LIMIT 1;
END;;
DELIMITER ;
