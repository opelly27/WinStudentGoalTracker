-- =====================================================================
-- Seed 10 Programs with Peer Reviewer users under the existing
-- school district: a1b2c3d4-0001-4000-a000-000000000001
--
-- Each program gets:
--   1. A program record
--   2. A user with the provided password hash/salt
--   3. A user_program link with the Teacher role
--
-- Designed to run in TablePlus against a MySQL database.
-- =====================================================================

-- -------------------------
-- 1. PROGRAMS
-- -------------------------
INSERT INTO program (id_program, id_school_district, name, description, created_at) VALUES
('b2c3d4e5-1001-4000-a000-000000000001', 'a1b2c3d4-0001-4000-a000-000000000001', 'Program 1',  'This is program 1',  UTC_TIMESTAMP()),
('b2c3d4e5-1002-4000-a000-000000000001', 'a1b2c3d4-0001-4000-a000-000000000001', 'Program 2',  'This is program 2',  UTC_TIMESTAMP()),
('b2c3d4e5-1003-4000-a000-000000000001', 'a1b2c3d4-0001-4000-a000-000000000001', 'Program 3',  'This is program 3',  UTC_TIMESTAMP()),
('b2c3d4e5-1004-4000-a000-000000000001', 'a1b2c3d4-0001-4000-a000-000000000001', 'Program 4',  'This is program 4',  UTC_TIMESTAMP()),
('b2c3d4e5-1005-4000-a000-000000000001', 'a1b2c3d4-0001-4000-a000-000000000001', 'Program 5',  'This is program 5',  UTC_TIMESTAMP()),
('b2c3d4e5-1006-4000-a000-000000000001', 'a1b2c3d4-0001-4000-a000-000000000001', 'Program 6',  'This is program 6',  UTC_TIMESTAMP()),
('b2c3d4e5-1007-4000-a000-000000000001', 'a1b2c3d4-0001-4000-a000-000000000001', 'Program 7',  'This is program 7',  UTC_TIMESTAMP()),
('b2c3d4e5-1008-4000-a000-000000000001', 'a1b2c3d4-0001-4000-a000-000000000001', 'Program 8',  'This is program 8',  UTC_TIMESTAMP()),
('b2c3d4e5-1009-4000-a000-000000000001', 'a1b2c3d4-0001-4000-a000-000000000001', 'Program 9',  'This is program 9',  UTC_TIMESTAMP()),
('b2c3d4e5-1010-4000-a000-000000000001', 'a1b2c3d4-0001-4000-a000-000000000001', 'Program 10', 'This is program 10', UTC_TIMESTAMP());

-- -------------------------
-- 2. USERS
-- -------------------------
INSERT INTO user (id_user, email, name, password_hash, password_salt, created_at) VALUES
('d4e5f6a7-1001-4000-a000-000000000001', 'peer_reviewer_1@gmail.com',  'Peer Reviewer 1',  'FEp+zRcVsX3wAtwriCh2WCnz4DfIZ/vw3M+Ke30VneM=', 'giXYhXeRNxh0OuQ3KQzLcA==', UTC_TIMESTAMP()),
('d4e5f6a7-1002-4000-a000-000000000001', 'peer_reviewer_2@gmail.com',  'Peer Reviewer 2',  'FEp+zRcVsX3wAtwriCh2WCnz4DfIZ/vw3M+Ke30VneM=', 'giXYhXeRNxh0OuQ3KQzLcA==', UTC_TIMESTAMP()),
('d4e5f6a7-1003-4000-a000-000000000001', 'peer_reviewer_3@gmail.com',  'Peer Reviewer 3',  'FEp+zRcVsX3wAtwriCh2WCnz4DfIZ/vw3M+Ke30VneM=', 'giXYhXeRNxh0OuQ3KQzLcA==', UTC_TIMESTAMP()),
('d4e5f6a7-1004-4000-a000-000000000001', 'peer_reviewer_4@gmail.com',  'Peer Reviewer 4',  'FEp+zRcVsX3wAtwriCh2WCnz4DfIZ/vw3M+Ke30VneM=', 'giXYhXeRNxh0OuQ3KQzLcA==', UTC_TIMESTAMP()),
('d4e5f6a7-1005-4000-a000-000000000001', 'peer_reviewer_5@gmail.com',  'Peer Reviewer 5',  'FEp+zRcVsX3wAtwriCh2WCnz4DfIZ/vw3M+Ke30VneM=', 'giXYhXeRNxh0OuQ3KQzLcA==', UTC_TIMESTAMP()),
('d4e5f6a7-1006-4000-a000-000000000001', 'peer_reviewer_6@gmail.com',  'Peer Reviewer 6',  'FEp+zRcVsX3wAtwriCh2WCnz4DfIZ/vw3M+Ke30VneM=', 'giXYhXeRNxh0OuQ3KQzLcA==', UTC_TIMESTAMP()),
('d4e5f6a7-1007-4000-a000-000000000001', 'peer_reviewer_7@gmail.com',  'Peer Reviewer 7',  'FEp+zRcVsX3wAtwriCh2WCnz4DfIZ/vw3M+Ke30VneM=', 'giXYhXeRNxh0OuQ3KQzLcA==', UTC_TIMESTAMP()),
('d4e5f6a7-1008-4000-a000-000000000001', 'peer_reviewer_8@gmail.com',  'Peer Reviewer 8',  'FEp+zRcVsX3wAtwriCh2WCnz4DfIZ/vw3M+Ke30VneM=', 'giXYhXeRNxh0OuQ3KQzLcA==', UTC_TIMESTAMP()),
('d4e5f6a7-1009-4000-a000-000000000001', 'peer_reviewer_9@gmail.com',  'Peer Reviewer 9',  'FEp+zRcVsX3wAtwriCh2WCnz4DfIZ/vw3M+Ke30VneM=', 'giXYhXeRNxh0OuQ3KQzLcA==', UTC_TIMESTAMP()),
('d4e5f6a7-1010-4000-a000-000000000001', 'peer_reviewer_10@gmail.com', 'Peer Reviewer 10', 'FEp+zRcVsX3wAtwriCh2WCnz4DfIZ/vw3M+Ke30VneM=', 'giXYhXeRNxh0OuQ3KQzLcA==', UTC_TIMESTAMP());

-- -------------------------
-- 3. USER_PROGRAM (Teacher role)
--    Teacher role ID: c3d4e5f6-0004-4000-a000-000000000001
-- -------------------------
INSERT INTO user_program (id_user_program, id_user, id_program, id_role, is_primary, status, joined_at) VALUES
('e5f6a7b8-1001-4000-a000-000000000001', 'd4e5f6a7-1001-4000-a000-000000000001', 'b2c3d4e5-1001-4000-a000-000000000001', 'c3d4e5f6-0004-4000-a000-000000000001', 1, 'active', UTC_TIMESTAMP()),
('e5f6a7b8-1002-4000-a000-000000000001', 'd4e5f6a7-1002-4000-a000-000000000001', 'b2c3d4e5-1002-4000-a000-000000000001', 'c3d4e5f6-0004-4000-a000-000000000001', 1, 'active', UTC_TIMESTAMP()),
('e5f6a7b8-1003-4000-a000-000000000001', 'd4e5f6a7-1003-4000-a000-000000000001', 'b2c3d4e5-1003-4000-a000-000000000001', 'c3d4e5f6-0004-4000-a000-000000000001', 1, 'active', UTC_TIMESTAMP()),
('e5f6a7b8-1004-4000-a000-000000000001', 'd4e5f6a7-1004-4000-a000-000000000001', 'b2c3d4e5-1004-4000-a000-000000000001', 'c3d4e5f6-0004-4000-a000-000000000001', 1, 'active', UTC_TIMESTAMP()),
('e5f6a7b8-1005-4000-a000-000000000001', 'd4e5f6a7-1005-4000-a000-000000000001', 'b2c3d4e5-1005-4000-a000-000000000001', 'c3d4e5f6-0004-4000-a000-000000000001', 1, 'active', UTC_TIMESTAMP()),
('e5f6a7b8-1006-4000-a000-000000000001', 'd4e5f6a7-1006-4000-a000-000000000001', 'b2c3d4e5-1006-4000-a000-000000000001', 'c3d4e5f6-0004-4000-a000-000000000001', 1, 'active', UTC_TIMESTAMP()),
('e5f6a7b8-1007-4000-a000-000000000001', 'd4e5f6a7-1007-4000-a000-000000000001', 'b2c3d4e5-1007-4000-a000-000000000001', 'c3d4e5f6-0004-4000-a000-000000000001', 1, 'active', UTC_TIMESTAMP()),
('e5f6a7b8-1008-4000-a000-000000000001', 'd4e5f6a7-1008-4000-a000-000000000001', 'b2c3d4e5-1008-4000-a000-000000000001', 'c3d4e5f6-0004-4000-a000-000000000001', 1, 'active', UTC_TIMESTAMP()),
('e5f6a7b8-1009-4000-a000-000000000001', 'd4e5f6a7-1009-4000-a000-000000000001', 'b2c3d4e5-1009-4000-a000-000000000001', 'c3d4e5f6-0004-4000-a000-000000000001', 1, 'active', UTC_TIMESTAMP()),
('e5f6a7b8-1010-4000-a000-000000000001', 'd4e5f6a7-1010-4000-a000-000000000001', 'b2c3d4e5-1010-4000-a000-000000000001', 'c3d4e5f6-0004-4000-a000-000000000001', 1, 'active', UTC_TIMESTAMP());
