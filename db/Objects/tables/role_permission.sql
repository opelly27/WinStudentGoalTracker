CREATE TABLE `role_permission` (
  `id_role_permission` int NOT NULL,
  `id_role` int DEFAULT NULL,
  `id_permission` int DEFAULT NULL,
  PRIMARY KEY (`id_role_permission`),
  KEY `id_role` (`id_role`),
  KEY `id_permission` (`id_permission`),
  CONSTRAINT `role_permission_ibfk_1` FOREIGN KEY (`id_role`) REFERENCES `role` (`id_role`),
  CONSTRAINT `role_permission_ibfk_2` FOREIGN KEY (`id_permission`) REFERENCES `permission` (`id_permission`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
