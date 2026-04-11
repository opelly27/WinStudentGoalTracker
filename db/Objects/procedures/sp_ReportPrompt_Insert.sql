DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_ReportPrompt_Insert`(
    IN p_id_report_prompt CHAR(36),
    IN p_id_program CHAR(36),
    IN p_prompt TEXT,
    IN p_reportname CHAR(100)
)
BEGIN
    INSERT INTO `ReportPrompt`
    (
        `id_ReportPrompt`,
        `id_program`,
        `prompt`,
        `reportname`
    )
    VALUES
    (
        p_id_report_prompt,
        p_id_program,
        p_prompt,
        p_reportname
    );
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
