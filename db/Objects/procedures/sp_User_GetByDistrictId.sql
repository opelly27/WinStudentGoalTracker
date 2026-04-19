DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_User_GetByDistrictId`(
    IN p_id_school_district CHAR(36)
)
BEGIN
    SELECT DISTINCT
        u.id_user,
        u.email,
        u.name,
        u.created_at,
        up.id_program,
        p.name AS program_name,
        r.id_role,
        r.name AS role_name,
        r.internal_name AS role_internal_name
    FROM `user` u
    INNER JOIN user_program up ON up.id_user = u.id_user
    INNER JOIN program p ON p.id_program = up.id_program
    INNER JOIN role r ON r.id_role = up.id_role
    WHERE p.id_school_district = p_id_school_district
      AND up.status = 'active'
    ORDER BY u.name, p.name;
END;;
DELIMITER ;
