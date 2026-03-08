CREATE TABLE `progress_event_benchmark` (
  `id_progress_event_benchmark` char(36) NOT NULL DEFAULT (uuid()),
  `id_progress_event` char(36) NOT NULL,
  `id_benchmark` char(36) NOT NULL,
  `created_at` datetime NOT NULL,
  PRIMARY KEY (`id_progress_event_benchmark`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
