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
