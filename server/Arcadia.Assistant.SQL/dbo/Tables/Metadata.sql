CREATE TABLE Metadata (
  PersistenceID NVARCHAR(255) NOT NULL,
  SequenceNr BIGINT NOT NULL,
  CONSTRAINT PK_Metadata PRIMARY KEY (PersistenceID, SequenceNr)
);