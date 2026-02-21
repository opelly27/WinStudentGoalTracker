DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_SchoolDistrict_GetById`(IN p_id_school_district CHAR(36))
BEGIN
    SELECT
        id_school_district,
        name,
        contact_email,
        created_at
    FROM school_district
    WHERE id_school_district = p_id_school_district
    LIMIT 1;
END;;
DELIMITER ;
