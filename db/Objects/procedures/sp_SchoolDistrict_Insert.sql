DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_SchoolDistrict_Insert`(
    IN p_id_school_district CHAR(36),
    IN p_name VARCHAR(255),
    IN p_contact_email VARCHAR(255)
)
BEGIN
    INSERT INTO school_district
    (
        id_school_district,
        name,
        contact_email,
        created_at
    )
    VALUES
    (
        p_id_school_district,
        p_name,
        p_contact_email,
        UTC_TIMESTAMP()
    );
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
