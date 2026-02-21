DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_SchoolDistrict_Update`(
    IN p_id_school_district CHAR(36),
    IN p_name VARCHAR(255),
    IN p_contact_email VARCHAR(255)
)
BEGIN
    UPDATE school_district
    SET
        name = COALESCE(p_name, name),
        contact_email = COALESCE(p_contact_email, contact_email)
    WHERE id_school_district = p_id_school_district;
    SELECT ROW_COUNT() AS rows_affected;
END;;
DELIMITER ;
