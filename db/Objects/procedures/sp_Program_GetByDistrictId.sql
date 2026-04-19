DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Program_GetByDistrictId`(
    IN p_id_school_district CHAR(36)
)
BEGIN
    SELECT
        id_program,
        id_school_district,
        name,
        description,
        created_at
    FROM program
    WHERE id_school_district = p_id_school_district
    ORDER BY name;
END;;
DELIMITER ;
