CREATE TABLE [dbo].[IsoFileRequest] (
    [IsoRequestId] INT NOT NULL,
    [IsoFileId]    INT NOT NULL,
    CONSTRAINT [PK_dbo.IsoFileRequest] PRIMARY KEY CLUSTERED ([IsoRequestId] ASC, [IsoFileId] ASC),
    CONSTRAINT [FK_dbo.IsoFileRequest_dbo.IsoFile_IsoFileId] FOREIGN KEY ([IsoFileId]) REFERENCES [dbo].[IsoFile] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.IsoFileRequest_dbo.IsoRequests_IsoRequestId] FOREIGN KEY ([IsoRequestId]) REFERENCES [dbo].[IsoRequest] ([Id]) ON DELETE CASCADE
);




GO
CREATE NONCLUSTERED INDEX [IX_IsoFileId]
    ON [dbo].[IsoFileRequest]([IsoFileId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_IsoRequestId]
    ON [dbo].[IsoFileRequest]([IsoRequestId] ASC);

