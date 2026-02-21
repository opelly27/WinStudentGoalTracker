CREATE TABLE `password_reset_token` (
  `id_password_reset_token` char(36) NOT NULL,
  `id_user` char(36) DEFAULT NULL,
  `token_hash` varchar(255) DEFAULT NULL,
  `expires_at` timestamp NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `used_at` timestamp NULL DEFAULT NULL,
  `invalidated_at` timestamp NULL DEFAULT NULL,
  `request_ip` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id_password_reset_token`),
  UNIQUE KEY `uq_token_hash` (`token_hash`),
  KEY `idx_user_created` (`id_user`,`created_at`),
  CONSTRAINT `password_reset_token_ibfk_1` FOREIGN KEY (`id_user`) REFERENCES `user` (`id_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
