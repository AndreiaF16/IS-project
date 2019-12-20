DROP TABLE [dbo].[SensoresPessoais]
DROP TABLE [dbo].[Utilizadores]

CREATE TABLE [dbo].[Utilizadores]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[Username] NVARCHAR (20) NOT NULL,
	[Nome] NVARCHAR (20) NOT NULL,
	[Email] NVARCHAR (40) NOT NULL,
	[Password] NVARCHAR (64) NOT NULL,
	[Role] NVARCHAR (5) NULL,
	CONSTRAINT [PK_Utilizador] PRIMARY KEY ([Id]),
	CONSTRAINT [Unique_Username] UNIQUE ([Username]),
	CONSTRAINT [Unique_Email] UNIQUE ([Email])
)

CREATE TABLE [dbo].[SensoresPessoais]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[Temperatura] DECIMAL (5, 2) NOT NULL,
	[Humidade] DECIMAL (5, 2) NOT NULL,
	[Data] DATETIME NOT NULL,
	[Valido] BIT  NOT NULL,
	[UtilizadorID] INT NOT NULL,
	[Local] NVARCHAR (50) NOT NULL,
	[ValidatedBy] INT NULL,
	[DateValidatedBy] DATETIME NULL,
	CONSTRAINT [PK_SensorPessoal] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_Utilizador_Created] FOREIGN KEY ([UtilizadorID]) REFERENCES [dbo].[Utilizadores]([Id]),
	CONSTRAINT [FK_Utilizador_Validated] FOREIGN KEY ([ValidatedBy]) REFERENCES [dbo].[Utilizadores]([Id])
)


INSERT INTO [dbo].[Utilizadores] ([Username], [Nome], [Email], [Password], [Role]) 
	VALUES ('admin', 'Admin', 'admin@ipleiriasmartcampus.pt', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'Admin');

INSERT INTO [dbo].[Utilizadores] ([Username], [Nome], [Email], [Password], [Role]) 
	VALUES ('guest', 'Guest', 'guest@ipleiriasmartcampus.pt', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'User');


INSERT INTO [dbo].[SensoresPessoais] ([Temperatura], [Humidade], [Data], [Valido], [UtilizadorID], [Local]) 
	VALUES (14.2, 66, CONVERT(DATETIME,'18-06-19 10:34:09',5), 1, 2, 'SALA A.0.0');

INSERT INTO [dbo].[SensoresPessoais] ([Temperatura], [Humidade], [Data], [Valido], [UtilizadorID], [Local]) 
	VALUES (18.2, 60, CONVERT(DATETIME,'19-06-19 14:34:09',5), 1, 2, 'BAR');

INSERT INTO [dbo].[SensoresPessoais] ([Temperatura], [Humidade], [Data], [Valido], [UtilizadorID], [Local]) 
	VALUES (22.2, 50, CONVERT(DATETIME,'15-09-19 11:34:59',5), 1, 2, 'SALA A.1.5');

SELECT * FROM [dbo].[Utilizadores];
SELECT * FROM [dbo].[SensoresPessoais];
SELECT * FROM [dbo].[Sensores];