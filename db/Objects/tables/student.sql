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
