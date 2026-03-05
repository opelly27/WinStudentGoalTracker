DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_ProgressEvent_GetByGoalId`(IN p_id_goal CHAR(36))
BEGIN
    SELECT
        vc.`progressEventId`,
        vc.`content`,
        vc.`createdAt`,
        vc.`createdByName`
    FROM `v_progress_event_card` vc
    WHERE vc.`goalId` = p_id_goal
    ORDER BY vc.`createdAt` DESC;
END;;
DELIMITER ;
