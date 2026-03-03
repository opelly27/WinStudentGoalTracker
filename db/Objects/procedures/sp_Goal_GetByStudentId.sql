DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Goal_GetByStudentId`(IN p_id_student CHAR(36))
BEGIN
    SELECT
        s.`identifier`          AS `studentIdentifier`,
        vc.`goalId`,
        vc.`goalParentId`,
        vc.`title`,
        vc.`description`,
        vc.`category`,
        vc.`progressEventCount`
    FROM `v_goal_card` vc
    INNER JOIN `student` s ON s.`id_student` = vc.`studentId`
    WHERE vc.`studentId` = p_id_student
    ORDER BY vc.`goalId`;
END;;
DELIMITER ;
