DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_UserPrograms_GetByUserId`(IN p_id_user CHAR(36))
BEGIN
    SELECT
        up.id_program,
        p.name AS program_name,
        r.internal_name AS role_internal_name,
        r.name AS role_display_name,
        up.is_primary,
        up.status
    FROM user_program up
    JOIN program p ON up.id_program = p.id_program
    JOIN role r ON up.id_role = r.id_role
    WHERE up.id_user = p_id_user
      AND up.status = 'active'
    ORDER BY up.is_primary DESC, p.name ASC;
END;;
DELIMITER ;
