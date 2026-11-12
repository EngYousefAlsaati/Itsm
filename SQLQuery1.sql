SELECT (SELECT COUNT(*) FROM TicketRatings) AS OriginalCount,
       (SELECT COUNT(*) FROM TicketRatings_backup) AS BackupCount;