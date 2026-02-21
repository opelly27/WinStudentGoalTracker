DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_SchoolDistrict_Delete`(IN p_id_school_district CHAR(36))
BEGIN
    DELETE FROM school_district
    WHERE id_school_district = p_id_school_district;
    SELECT ROW_COUNT() AS rows_affected;
END;;
DELIMITER ;
