DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Program_Delete`(IN p_id_program CHAR(36))
BEGIN
    DELETE FROM program
    WHERE id_program = p_id_program;
    SELECT ROW_COUNT() AS rows_affected;
END;;
DELIMITER ;
