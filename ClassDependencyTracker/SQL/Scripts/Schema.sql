CREATE TABLE [classes] (
  [id] integer PRIMARY KEY NOT NULL,
  [name] nvarchar(255),
  [url] nvarchar(255)
)
GO

CREATE TABLE [requirements] (
  [id] integer PRIMARY KEY NOT NULL,
  [source_id] integer NOT NULL,
  [required_id] integer NOT NULL
)
GO

ALTER TABLE [requirements] ADD FOREIGN KEY ([source_id]) REFERENCES [classes] ([id])
GO

ALTER TABLE [requirements] ADD FOREIGN KEY ([required_id]) REFERENCES [classes] ([id])
GO
