CREATE TABLE `user_program` (
  `id_user_program` char(36) NOT NULL,
  `id_user` char(36) DEFAULT NULL,
  `id_program` char(36) DEFAULT NULL,
  `is_primary` tinyint(1) DEFAULT '0',
  `status` varchar(20) DEFAULT 'active',
  `joined_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id_user_program`),
  UNIQUE KEY `uq_user_program` (`id_user`,`id_program`),
  KEY `idx_id_program` (`id_program`),
  CONSTRAINT `user_program_ibfk_1` FOREIGN KEY (`id_user`) REFERENCES `user` (`id_user`),
  CONSTRAINT `user_program_ibfk_2` FOREIGN KEY (`id_program`) REFERENCES `program` (`id_program`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
