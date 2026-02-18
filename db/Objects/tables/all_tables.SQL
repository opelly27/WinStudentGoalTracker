-- ============================================================
-- WinStudentGoalTracker - All Table Definitions
-- Generated: 2026-02-18
-- ============================================================

-- -----------------------------------------------------------
-- Tables with no foreign key dependencies
-- -----------------------------------------------------------

-- permission
CREATE TABLE `permission` (
  `id_permission` int NOT NULL,
  `name` varchar(100) DEFAULT NULL,
  `description` text,
  `resource` varchar(100) DEFAULT NULL,
  `action` varchar(50) DEFAULT NULL,
  `scope` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id_permission`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- role
CREATE TABLE `role` (
  `id_role` int NOT NULL,
  `name` varchar(100) DEFAULT NULL,
  `description` text,
  `created_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id_role`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- school_district
CREATE TABLE `school_district` (
  `id_school_district` int NOT NULL,
  `name` varchar(255) DEFAULT NULL,
  `contact_email` varchar(255) DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id_school_district`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- -----------------------------------------------------------
-- Tables depending on role
-- -----------------------------------------------------------

-- role_permission
CREATE TABLE `role_permission` (
  `id_role_permission` int NOT NULL,
  `id_role` int DEFAULT NULL,
  `id_permission` int DEFAULT NULL,
  PRIMARY KEY (`id_role_permission`),
  KEY `id_role` (`id_role`),
  KEY `id_permission` (`id_permission`),
  CONSTRAINT `role_permission_ibfk_1` FOREIGN KEY (`id_role`) REFERENCES `role` (`id_role`),
  CONSTRAINT `role_permission_ibfk_2` FOREIGN KEY (`id_permission`) REFERENCES `permission` (`id_permission`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- user
CREATE TABLE `user` (
  `id_user` int NOT NULL,
  `id_role` int DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `name` varchar(255) DEFAULT NULL,
  `password_hash` varchar(255) DEFAULT NULL,
  `password_updated_at` timestamp NULL DEFAULT NULL,
  `failed_login_attempts` int DEFAULT '0',
  `locked_until` timestamp NULL DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id_user`),
  KEY `id_role` (`id_role`),
  CONSTRAINT `user_ibfk_1` FOREIGN KEY (`id_role`) REFERENCES `role` (`id_role`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- -----------------------------------------------------------
-- Tables depending on school_district
-- -----------------------------------------------------------

-- program
CREATE TABLE `program` (
  `id_program` int NOT NULL,
  `id_school_district` int DEFAULT NULL,
  `name` varchar(255) DEFAULT NULL,
  `description` text,
  `created_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id_program`),
  KEY `id_school_district` (`id_school_district`),
  CONSTRAINT `program_ibfk_1` FOREIGN KEY (`id_school_district`) REFERENCES `school_district` (`id_school_district`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- -----------------------------------------------------------
-- Tables depending on user and/or program
-- -----------------------------------------------------------

-- password_history
CREATE TABLE `password_history` (
  `id_password_history` int NOT NULL,
  `id_user` int DEFAULT NULL,
  `password_hash` varchar(255) NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id_password_history`),
  KEY `idx_user_created` (`id_user`,`created_at`),
  CONSTRAINT `password_history_ibfk_1` FOREIGN KEY (`id_user`) REFERENCES `user` (`id_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- password_reset_token
CREATE TABLE `password_reset_token` (
  `id_password_reset_token` int NOT NULL,
  `id_user` int DEFAULT NULL,
  `token_hash` varchar(255) DEFAULT NULL,
  `expires_at` timestamp NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `used_at` timestamp NULL DEFAULT NULL,
  `invalidated_at` timestamp NULL DEFAULT NULL,
  `request_ip` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id_password_reset_token`),
  UNIQUE KEY `uq_token_hash` (`token_hash`),
  KEY `idx_user_created` (`id_user`,`created_at`),
  CONSTRAINT `password_reset_token_ibfk_1` FOREIGN KEY (`id_user`) REFERENCES `user` (`id_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- student
CREATE TABLE `student` (
  `id_student` int NOT NULL,
  `id_program` int DEFAULT NULL,
  `identifier` varchar(50) DEFAULT NULL,
  `program_year` int DEFAULT NULL,
  `enrollment_date` date DEFAULT NULL,
  `expected_grad` date DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id_student`),
  KEY `id_program` (`id_program`),
  CONSTRAINT `student_ibfk_1` FOREIGN KEY (`id_program`) REFERENCES `program` (`id_program`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- user_program
CREATE TABLE `user_program` (
  `id_user_program` int NOT NULL,
  `id_user` int DEFAULT NULL,
  `id_program` int DEFAULT NULL,
  `is_primary` tinyint(1) DEFAULT '0',
  `status` varchar(20) DEFAULT 'active',
  `joined_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id_user_program`),
  UNIQUE KEY `uq_user_program` (`id_user`,`id_program`),
  KEY `idx_id_program` (`id_program`),
  CONSTRAINT `user_program_ibfk_1` FOREIGN KEY (`id_user`) REFERENCES `user` (`id_user`),
  CONSTRAINT `user_program_ibfk_2` FOREIGN KEY (`id_program`) REFERENCES `program` (`id_program`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- -----------------------------------------------------------
-- Tables depending on student and user
-- -----------------------------------------------------------

-- user_student
CREATE TABLE `user_student` (
  `id_user_student` int NOT NULL,
  `id_user` int DEFAULT NULL,
  `id_student` int DEFAULT NULL,
  `access_level` varchar(50) DEFAULT NULL,
  `is_primary` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`id_user_student`),
  KEY `id_user` (`id_user`),
  KEY `id_student` (`id_student`),
  CONSTRAINT `user_student_ibfk_1` FOREIGN KEY (`id_user`) REFERENCES `user` (`id_user`),
  CONSTRAINT `user_student_ibfk_2` FOREIGN KEY (`id_student`) REFERENCES `student` (`id_student`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- goal (self-referencing + student + user)
CREATE TABLE `goal` (
  `id_goal` int NOT NULL,
  `id_goal_parent` int DEFAULT NULL,
  `id_student` int DEFAULT NULL,
  `id_user_created` int DEFAULT NULL,
  `title` varchar(255) DEFAULT NULL,
  `description` text,
  `category` varchar(100) DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id_goal`),
  KEY `id_goal_parent` (`id_goal_parent`),
  KEY `id_student` (`id_student`),
  KEY `id_user_created` (`id_user_created`),
  CONSTRAINT `goal_ibfk_1` FOREIGN KEY (`id_goal_parent`) REFERENCES `goal` (`id_goal`),
  CONSTRAINT `goal_ibfk_2` FOREIGN KEY (`id_student`) REFERENCES `student` (`id_student`),
  CONSTRAINT `goal_ibfk_3` FOREIGN KEY (`id_user_created`) REFERENCES `user` (`id_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- health_note
CREATE TABLE `health_note` (
  `id_health_note` int NOT NULL,
  `id_student` int DEFAULT NULL,
  `id_user_created` int DEFAULT NULL,
  `content` text,
  `created_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id_health_note`),
  KEY `id_student` (`id_student`),
  KEY `id_user_created` (`id_user_created`),
  CONSTRAINT `health_note_ibfk_1` FOREIGN KEY (`id_student`) REFERENCES `student` (`id_student`),
  CONSTRAINT `health_note_ibfk_2` FOREIGN KEY (`id_user_created`) REFERENCES `user` (`id_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- -----------------------------------------------------------
-- Tables depending on goal, student, and user
-- -----------------------------------------------------------

-- progress_event
CREATE TABLE `progress_event` (
  `id_progress_event` int NOT NULL,
  `id_student` int DEFAULT NULL,
  `id_goal` int DEFAULT NULL,
  `id_user_created` int DEFAULT NULL,
  `content` text,
  `is_sensitive` tinyint(1) DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id_progress_event`),
  KEY `id_student` (`id_student`),
  KEY `id_goal` (`id_goal`),
  KEY `id_user_created` (`id_user_created`),
  CONSTRAINT `progress_event_ibfk_1` FOREIGN KEY (`id_student`) REFERENCES `student` (`id_student`),
  CONSTRAINT `progress_event_ibfk_2` FOREIGN KEY (`id_goal`) REFERENCES `goal` (`id_goal`),
  CONSTRAINT `progress_event_ibfk_3` FOREIGN KEY (`id_user_created`) REFERENCES `user` (`id_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- progress_report
CREATE TABLE `progress_report` (
  `id_progress_report` int NOT NULL,
  `id_student` int DEFAULT NULL,
  `id_goal` int DEFAULT NULL,
  `id_user_created` int DEFAULT NULL,
  `period` varchar(10) DEFAULT NULL,
  `year` int DEFAULT NULL,
  `summary` text,
  `generated_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id_progress_report`),
  KEY `id_student` (`id_student`),
  KEY `id_goal` (`id_goal`),
  KEY `id_user_created` (`id_user_created`),
  CONSTRAINT `progress_report_ibfk_1` FOREIGN KEY (`id_student`) REFERENCES `student` (`id_student`),
  CONSTRAINT `progress_report_ibfk_2` FOREIGN KEY (`id_goal`) REFERENCES `goal` (`id_goal`),
  CONSTRAINT `progress_report_ibfk_3` FOREIGN KEY (`id_user_created`) REFERENCES `user` (`id_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
