CREATE TABLE [dbo].[IsoFile] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255)  NOT NULL,
    [Extension]   NVARCHAR (255)  NULL,
    [Created]     DATETIME        NOT NULL,
    [Modified]    DATETIME        NOT NULL,
    [Size]        DECIMAL (18, 2) NOT NULL,
    [Parent_Id]   INT             NOT NULL,
    [IsoVolumeId] INT             DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.IsoFile] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.IsoFile_dbo.IsoFolder_Parent_Id] FOREIGN KEY ([Parent_Id]) REFERENCES [dbo].[IsoFolder] ([Id]) ON DELETE CASCADE
);






GO
CREATE NONCLUSTERED INDEX [IX_Parent_Id]
    ON [dbo].[IsoFile]([Parent_Id] ASC);

