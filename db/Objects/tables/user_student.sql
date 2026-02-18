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
