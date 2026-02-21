DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Program_GetAll`()
BEGIN
    SELECT
        id_program,
        id_school_district,
        name,
        description,
        created_at
    FROM program
    ORDER BY id_program;
END;;
DELIMITER ;
