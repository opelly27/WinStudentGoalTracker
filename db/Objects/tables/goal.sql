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
