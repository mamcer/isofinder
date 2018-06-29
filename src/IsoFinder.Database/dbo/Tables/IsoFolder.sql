CREATE TABLE [dbo].[IsoFolder] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Path]        NVARCHAR (255) NOT NULL,
    [Parent_Id]   INT            NULL,
    [IsoVolumeId] INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.IsoFolder] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.IsoFolder_dbo.IsoFolder_Parent_Id] FOREIGN KEY ([Parent_Id]) REFERENCES [dbo].[IsoFolder] ([Id])
);










GO
CREATE NONCLUSTERED INDEX [IX_Parent_Id]
    ON [dbo].[IsoFolder]([Parent_Id] ASC);

