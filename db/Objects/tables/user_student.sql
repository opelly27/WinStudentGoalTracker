CREATE TABLE `user_student` (
  `id_user_student` char(36) NOT NULL,
  `id_user` char(36) DEFAULT NULL,
  `id_student` char(36) DEFAULT NULL,
  `is_primary` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`id_user_student`),
  KEY `user_student_ibfk_1` (`id_user`),
  KEY `user_student_ibfk_2` (`id_student`),
  CONSTRAINT `user_student_ibfk_1` FOREIGN KEY (`id_user`) REFERENCES `user` (`id_user`),
  CONSTRAINT `user_student_ibfk_2` FOREIGN KEY (`id_student`) REFERENCES `student` (`id_student`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
