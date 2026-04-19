DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Role_GetAll`()
BEGIN
    SELECT
        id_role,
        name,
        internal_name,
        description
    FROM role
    ORDER BY name;
END;;
DELIMITER ;
