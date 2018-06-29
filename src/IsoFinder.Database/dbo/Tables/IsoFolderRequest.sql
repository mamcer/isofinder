CREATE TABLE [dbo].[IsoFolderRequest] (
    [IsoRequestId] INT NOT NULL,
    [IsoFolderId]  INT NOT NULL,
    CONSTRAINT [PK_dbo.IsoFolderRequest] PRIMARY KEY CLUSTERED ([IsoRequestId] ASC, [IsoFolderId] ASC),
    CONSTRAINT [FK_dbo.IsoFolderRequest_dbo.IsoFolder_IsoFolderId] FOREIGN KEY ([IsoFolderId]) REFERENCES [dbo].[IsoFolder] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.IsoFolderRequest_dbo.IsoRequests_IsoRequestId] FOREIGN KEY ([IsoRequestId]) REFERENCES [dbo].[IsoRequest] ([Id]) ON DELETE CASCADE
);




GO
CREATE NONCLUSTERED INDEX [IX_IsoFolderId]
    ON [dbo].[IsoFolderRequest]([IsoFolderId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IsoRequestId]
    ON [dbo].[IsoFolderRequest]([IsoRequestId] ASC);

