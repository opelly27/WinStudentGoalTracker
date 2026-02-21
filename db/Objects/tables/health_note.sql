CREATE TABLE `health_note` (
  `id_health_note` char(36) NOT NULL,
  `id_student` char(36) DEFAULT NULL,
  `id_user_created` char(36) DEFAULT NULL,
  `content` text,
  `created_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id_health_note`),
  KEY `health_note_ibfk_1` (`id_student`),
  KEY `health_note_ibfk_2` (`id_user_created`),
  CONSTRAINT `health_note_ibfk_1` FOREIGN KEY (`id_student`) REFERENCES `student` (`id_student`),
  CONSTRAINT `health_note_ibfk_2` FOREIGN KEY (`id_user_created`) REFERENCES `user` (`id_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
