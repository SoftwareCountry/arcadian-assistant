CREATE TABLE EventJournal (
  Ordering BIGINT IDENTITY(1,1) NOT NULL,
  PersistenceID NVARCHAR(255) NOT NULL,
  SequenceNr BIGINT NOT NULL,
  Timestamp BIGINT NOT NULL,
  IsDeleted BIT NOT NULL,
  Manifest NVARCHAR(500) NOT NULL,
  Payload VARBINARY(MAX) NOT NULL,
  Tags NVARCHAR(100) NULL,
  SerializerId INTEGER NULL
	CONSTRAINT PK_EventJournal PRIMARY KEY (Ordering),
  CONSTRAINT QU_EventJournal UNIQUE (PersistenceID, SequenceNr)
);