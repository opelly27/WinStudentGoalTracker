CREATE TABLE `benchmark` (
  `id_benchmark` char(36) NOT NULL DEFAULT (uuid()),
  `id_goal` char(36) NOT NULL,
  `id_user_created` char(36) NOT NULL,
  `benchmark` text NOT NULL,
  `short_name` varchar(50) DEFAULT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id_benchmark`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
