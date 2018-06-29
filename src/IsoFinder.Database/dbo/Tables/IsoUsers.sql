CREATE TABLE [dbo].[IsoUsers] (
    [Id]      INT            IDENTITY (1, 1) NOT NULL,
    [Name]    NVARCHAR (255) NOT NULL,
    [Created] DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.IsoUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);

