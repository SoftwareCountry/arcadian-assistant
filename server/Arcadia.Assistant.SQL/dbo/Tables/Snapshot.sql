CREATE TABLE Snapshot (
  PersistenceID NVARCHAR(255) NOT NULL,
  SequenceNr BIGINT NOT NULL,
  Timestamp DATETIME2 NOT NULL,
  Manifest NVARCHAR(500) NOT NULL,
  Snapshot VARBINARY(MAX) NOT NULL,
  SerializerId INTEGER NULL
  CONSTRAINT PK_Snapshot PRIMARY KEY (PersistenceID, SequenceNr)
);