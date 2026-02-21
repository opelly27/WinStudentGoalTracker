DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Program_Update`(
    IN p_id_program CHAR(36),
    IN p_id_school_district CHAR(36),
    IN p_name VARCHAR(255),
    IN p_description TEXT
)
BEGIN
    UPDATE program
    SET
        id_school_district = COALESCE(p_id_school_district, id_school_district),
        name = COALESCE(p_name, name),
        description = COALESCE(p_description, description)
    WHERE id_program = p_id_program;
    SELECT ROW_COUNT() AS rows_affected;
END;;
DELIMITER ;
