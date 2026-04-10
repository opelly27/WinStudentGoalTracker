CREATE TABLE `ReportPrompt` (
  `id_ReportPrompt` char(36) NOT NULL DEFAULT (uuid()),
  `prompt` text NOT NULL,
  `reportname` char(100) NOT NULL,
  `id_program` char(36) DEFAULT 'NULL',
  PRIMARY KEY (`id_ReportPrompt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
