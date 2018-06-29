CREATE TABLE [dbo].[IsoRequest] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Created]   DATETIME       NOT NULL,
    [User_Id]   INT            NOT NULL,
    [Status]    INT            DEFAULT ((0)) NOT NULL,
    [Completed] DATETIME       DEFAULT ('1900-01-01T00:00:00.000') NOT NULL,
    [FileName]  NVARCHAR (255) NULL,
    [Query]     NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.IsoRequest] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.IsoRequests_dbo.IsoUsers_User_Id] FOREIGN KEY ([User_Id]) REFERENCES [dbo].[IsoUsers] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_User_Id]
    ON [dbo].[IsoRequest]([User_Id] ASC);

