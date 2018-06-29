CREATE TABLE [dbo].[IsoVolume] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [FileName]      NVARCHAR (255)  NOT NULL,
    [VolumeLabel]   NVARCHAR (255)  NOT NULL,
    [FileCount]     INT             NOT NULL,
    [Size]          DECIMAL (18, 2) NOT NULL,
    [Created]       DATETIME        NOT NULL,
    [RootFolder_Id] INT             NOT NULL,
    CONSTRAINT [PK_dbo.IsoVolume] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.IsoVolume_dbo.IsoFolder_RootFolder_Id] FOREIGN KEY ([RootFolder_Id]) REFERENCES [dbo].[IsoFolder] ([Id]) ON DELETE CASCADE
);






GO
CREATE NONCLUSTERED INDEX [IX_RootFolder_Id]
    ON [dbo].[IsoVolume]([RootFolder_Id] ASC);

