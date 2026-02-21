CREATE TABLE `progress_report` (
  `id_progress_report` char(36) NOT NULL,
  `id_student` char(36) DEFAULT NULL,
  `id_goal` char(36) DEFAULT NULL,
  `id_user_created` char(36) DEFAULT NULL,
  `period` varchar(10) DEFAULT NULL,
  `year` int DEFAULT NULL,
  `summary` text,
  `generated_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id_progress_report`),
  KEY `progress_report_ibfk_1` (`id_student`),
  KEY `progress_report_ibfk_2` (`id_goal`),
  KEY `progress_report_ibfk_3` (`id_user_created`),
  CONSTRAINT `progress_report_ibfk_1` FOREIGN KEY (`id_student`) REFERENCES `student` (`id_student`),
  CONSTRAINT `progress_report_ibfk_2` FOREIGN KEY (`id_goal`) REFERENCES `goal` (`id_goal`),
  CONSTRAINT `progress_report_ibfk_3` FOREIGN KEY (`id_user_created`) REFERENCES `user` (`id_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
