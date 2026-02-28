CREATE TABLE `student` (
  `id_student` char(36) NOT NULL,
  `id_program` char(36) DEFAULT NULL,
  `id_user` char(36) DEFAULT NULL,
  `identifier` varchar(50) DEFAULT NULL,
  `program_year` int DEFAULT NULL,
  `enrollment_date` date DEFAULT NULL,
  `expected_grad` date DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id_student`),
  KEY `student_ibfk_1` (`id_program`),
  KEY `student_ibfk_2` (`id_user`),
  CONSTRAINT `student_ibfk_1` FOREIGN KEY (`id_program`) REFERENCES `program` (`id_program`),
  CONSTRAINT `student_ibfk_2` FOREIGN KEY (`id_user`) REFERENCES `user` (`id_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
