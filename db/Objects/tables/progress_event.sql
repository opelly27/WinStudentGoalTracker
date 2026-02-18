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
