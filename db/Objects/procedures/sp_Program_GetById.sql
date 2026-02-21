DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Program_GetById`(IN p_id_program CHAR(36))
BEGIN
    SELECT
        id_program,
        id_school_district,
        name,
        description,
        created_at
    FROM program
    WHERE id_program = p_id_program
    LIMIT 1;
END;;
DELIMITER ;
