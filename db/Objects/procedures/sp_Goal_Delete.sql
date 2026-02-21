DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Goal_Delete`(IN p_id_goal CHAR(36))
BEGIN
    DELETE FROM goal
    WHERE id_goal = p_id_goal;
    SELECT ROW_COUNT() AS rows_affected;
END;;
DELIMITER ;
