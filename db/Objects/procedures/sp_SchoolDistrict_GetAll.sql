DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_SchoolDistrict_GetAll`()
BEGIN
    SELECT
        id_school_district,
        name,
        contact_email,
        created_at
    FROM school_district
    ORDER BY id_school_district;
END;;
DELIMITER ;
