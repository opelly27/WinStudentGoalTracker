DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_SeedData_1`()
BEGIN
    -- =============================================================================
	-- Seed Data Script - create minimal set of working data
	-- =============================================================================
	-- =============================================================================
	-- 1. SCHOOL DISTRICT
	-- =============================================================================
	INSERT INTO school_district (id_school_district, name, contact_email, created_at)
	VALUES ('a1b2c3d4-0001-4000-a000-000000000001', 'Parent District', 'contact@dummydistrict.edu', UTC_TIMESTAMP());
	-- =============================================================================
	-- 2. PROGRAM (child of the district above)
	-- =============================================================================
	INSERT INTO program (id_program, id_school_district, name, description, created_at)
	VALUES ('b2c3d4e5-0001-4000-a000-000000000001', 'a1b2c3d4-0001-4000-a000-000000000001', 'Test Program', 'A sample program for testing', UTC_TIMESTAMP());
	-- =============================================================================
	-- 3. ROLES
	-- =============================================================================
	INSERT INTO role (id_role, name, internal_name, description, created_at) VALUES
	('c3d4e5f6-0001-4000-a000-000000000001', 'Super Admin',    'super_admin',    'Full system access',             UTC_TIMESTAMP()),
	('c3d4e5f6-0002-4000-a000-000000000001', 'District Admin', 'district_admin', 'District-level administration',   UTC_TIMESTAMP()),
	('c3d4e5f6-0003-4000-a000-000000000001', 'Program Admin',  'program_admin',  'Program-level administration',    UTC_TIMESTAMP()),
	('c3d4e5f6-0004-4000-a000-000000000001', 'Teacher',        'teacher',        'Teacher role',                    UTC_TIMESTAMP()),
	('c3d4e5f6-0005-4000-a000-000000000001', 'Paraeducator',   'paraeducator',   'Paraeducator role',               UTC_TIMESTAMP()),
	('c3d4e5f6-0006-4000-a000-000000000001', 'Student',        'student',        'Student role',                    UTC_TIMESTAMP());
	-- =============================================================================
	-- 4. USERS ? 2 Teachers + 1 Paraeducator
	-- =============================================================================
	-- Teacher 1
	INSERT INTO user (id_user, id_role, email, name, password_hash, password_salt, created_at)
	VALUES ('d4e5f6a7-0001-4000-a000-000000000001', 'c3d4e5f6-0004-4000-a000-000000000001',
	        'teacher1@example.com', 'Teacher One', NULL, NULL, UTC_TIMESTAMP());
	-- Teacher 2
	INSERT INTO user (id_user, id_role, email, name, password_hash, password_salt, created_at)
	VALUES ('d4e5f6a7-0002-4000-a000-000000000001', 'c3d4e5f6-0004-4000-a000-000000000001',
	        'teacher2@example.com', 'Teacher Two', NULL, NULL, UTC_TIMESTAMP());
	-- Paraeducator 1
	INSERT INTO user (id_user, id_role, email, name, password_hash, password_salt, created_at)
	VALUES ('d4e5f6a7-0003-4000-a000-000000000001', 'c3d4e5f6-0005-4000-a000-000000000001',
	        'para1@example.com', 'Para One', NULL, NULL, UTC_TIMESTAMP());
	-- =============================================================================
	-- 5. USER_PROGRAM ? link all 3 users to the program
	-- =============================================================================
	INSERT INTO user_program (id_user_program, id_user, id_program, is_primary, status, joined_at) VALUES
	('e5f6a7b8-0001-4000-a000-000000000001', 'd4e5f6a7-0001-4000-a000-000000000001', 'b2c3d4e5-0001-4000-a000-000000000001', 1, 'active', UTC_TIMESTAMP()),
	('e5f6a7b8-0002-4000-a000-000000000001', 'd4e5f6a7-0002-4000-a000-000000000001', 'b2c3d4e5-0001-4000-a000-000000000001', 1, 'active', UTC_TIMESTAMP()),
	('e5f6a7b8-0003-4000-a000-000000000001', 'd4e5f6a7-0003-4000-a000-000000000001', 'b2c3d4e5-0001-4000-a000-000000000001', 1, 'active', UTC_TIMESTAMP());
	-- =============================================================================
	-- 6. STUDENTS ? 5 students under the program
	-- =============================================================================
	INSERT INTO student (id_student, id_program, identifier, program_year, enrollment_date, expected_grad, created_at) VALUES
	('f6a7b8c9-0001-4000-a000-000000000001', 'b2c3d4e5-0001-4000-a000-000000000001', 'STU-001', 1, '2025-09-01', '2029-06-15', UTC_TIMESTAMP()),
	('f6a7b8c9-0002-4000-a000-000000000001', 'b2c3d4e5-0001-4000-a000-000000000001', 'STU-002', 1, '2025-09-01', '2029-06-15', UTC_TIMESTAMP()),
	('f6a7b8c9-0003-4000-a000-000000000001', 'b2c3d4e5-0001-4000-a000-000000000001', 'STU-003', 2, '2024-09-01', '2028-06-15', UTC_TIMESTAMP()),
	('f6a7b8c9-0004-4000-a000-000000000001', 'b2c3d4e5-0001-4000-a000-000000000001', 'STU-004', 2, '2024-09-01', '2028-06-15', UTC_TIMESTAMP()),
	('f6a7b8c9-0005-4000-a000-000000000001', 'b2c3d4e5-0001-4000-a000-000000000001', 'STU-005', 3, '2023-09-01', '2027-06-15', UTC_TIMESTAMP());
END;;
DELIMITER ;
