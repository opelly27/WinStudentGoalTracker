DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_Program_Insert`(
    IN p_id_program CHAR(36),
    IN p_id_school_district CHAR(36),
    IN p_name VARCHAR(255),
    IN p_description TEXT
)
BEGIN
    INSERT INTO program
    (
        id_program,
        id_school_district,
        name,
        description,
        created_at
    )
    VALUES
    (
        p_id_program,
        p_id_school_district,
        p_name,
        p_description,
        UTC_TIMESTAMP()
    );
    SELECT
        id_program,
        id_school_district,
        name,
        description,
        created_at
    FROM program
    WHERE id_program = p_id_program
    LIMIT 1;
END;;
DELIMITER ;
