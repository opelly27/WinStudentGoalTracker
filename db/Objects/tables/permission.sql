CREATE TABLE `permission` (
  `id_permission` char(36) NOT NULL,
  `name` varchar(100) DEFAULT NULL,
  `description` text,
  `resource` varchar(100) DEFAULT NULL,
  `action` varchar(50) DEFAULT NULL,
  `scope` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id_permission`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
