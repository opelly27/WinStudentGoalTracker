DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_ReportPrompt_GetAll`()
BEGIN
    SELECT
        `id_report_prompt`  AS `reportPromptId`,
        `id_program`        AS `programId`,
        `prompt`            AS `prompt`,
        `reportname`        AS `reportname`
    FROM `ReportPrompt`
    ORDER BY `reportname`;
END;;
DELIMITER ;
