CREATE TABLE `password_history` (
  `id_password_history` char(36) NOT NULL,
  `id_user` char(36) DEFAULT NULL,
  `password_hash` varchar(255) NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id_password_history`),
  KEY `idx_user_created` (`id_user`,`created_at`),
  CONSTRAINT `password_history_ibfk_1` FOREIGN KEY (`id_user`) REFERENCES `user` (`id_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
