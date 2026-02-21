CREATE TABLE `role_permission` (
  `id_role_permission` char(36) NOT NULL,
  `id_role` char(36) DEFAULT NULL,
  `id_permission` char(36) DEFAULT NULL,
  PRIMARY KEY (`id_role_permission`),
  KEY `role_permission_ibfk_1` (`id_role`),
  KEY `role_permission_ibfk_2` (`id_permission`),
  CONSTRAINT `role_permission_ibfk_1` FOREIGN KEY (`id_role`) REFERENCES `role` (`id_role`),
  CONSTRAINT `role_permission_ibfk_2` FOREIGN KEY (`id_permission`) REFERENCES `permission` (`id_permission`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
