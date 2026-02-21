CREATE TABLE `role` (
  `id_role` char(36) NOT NULL,
  `name` varchar(100) DEFAULT NULL,
  `internal_name` varchar(100) DEFAULT NULL,
  `description` text,
  `created_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id_role`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
