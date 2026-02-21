CREATE TABLE `progress_event` (
  `id_progress_event` char(36) NOT NULL,
  `id_student` char(36) DEFAULT NULL,
  `id_goal` char(36) DEFAULT NULL,
  `id_user_created` char(36) DEFAULT NULL,
  `content` text,
  `is_sensitive` tinyint(1) DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id_progress_event`),
  KEY `progress_event_ibfk_1` (`id_student`),
  KEY `progress_event_ibfk_2` (`id_goal`),
  KEY `progress_event_ibfk_3` (`id_user_created`),
  CONSTRAINT `progress_event_ibfk_1` FOREIGN KEY (`id_student`) REFERENCES `student` (`id_student`),
  CONSTRAINT `progress_event_ibfk_2` FOREIGN KEY (`id_goal`) REFERENCES `goal` (`id_goal`),
  CONSTRAINT `progress_event_ibfk_3` FOREIGN KEY (`id_user_created`) REFERENCES `user` (`id_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
