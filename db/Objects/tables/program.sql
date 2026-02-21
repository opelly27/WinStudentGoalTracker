CREATE TABLE `program` (
  `id_program` char(36) NOT NULL,
  `id_school_district` char(36) DEFAULT NULL,
  `name` varchar(255) DEFAULT NULL,
  `description` text,
  `created_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id_program`),
  KEY `program_ibfk_1` (`id_school_district`),
  CONSTRAINT `program_ibfk_1` FOREIGN KEY (`id_school_district`) REFERENCES `school_district` (`id_school_district`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
