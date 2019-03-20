CREATE TABLE [dbo].[Mailings] (
    [TrackingNumber]   NVARCHAR (14)  NOT NULL,
    [TypeOfMailing]    NVARCHAR (40)  NOT NULL,
    [DestinationIndex] INT            NOT NULL,
    [Address]          NVARCHAR (100) NOT NULL,
    [Delivered]        BIT            DEFAULT ((0)) NOT NULL,
    [AddresserEmail]   NVARCHAR (50)  NULL,
    [AddresseeEmail]   NVARCHAR (50)  NULL,
    PRIMARY KEY CLUSTERED ([TrackingNumber] ASC),
    CONSTRAINT [CK_Mailings_Column] CHECK ([DestinationIndex]>(99999) AND [DestinationIndex]<(1000000))
);

CREATE TABLE [dbo].[PostOffices] (
    [Index]        INT           NOT NULL,
    [Address]      NVARCHAR (100) NOT NULL,
    [Phone]        NVARCHAR (18)  NOT NULL,
    [WorkingHours] NVARCHAR (200) DEFAULT (N'пн — пт: 08:00—20:00@сб: 09:00—18:00@вс — выходной') NOT NULL,
    PRIMARY KEY CLUSTERED ([Index] ASC),
    CONSTRAINT [CK_PostOffices_Column] CHECK ([Index]>(99999) AND [Index]<(1000000) AND [Phone] like '8 ([3489][0-9][0-9]) [0-9][0-9][0-9]-[0-9][0-9]-[0-9][0-9]')
);

CREATE TABLE [dbo].[Tracking] (
    [TrackNumber]          NVARCHAR (14) NOT NULL,
    [Index]                INT           NOT NULL,
    [IsSent]               BIT           DEFAULT ((0)) NOT NULL,
    [RegistrationDateTime] DATETIME      DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Tracking_Column] PRIMARY KEY CLUSTERED ([TrackNumber] ASC, [Index] ASC, [IsSent] ASC),
    CONSTRAINT [FK_Tracking_Mailings] FOREIGN KEY ([TrackNumber]) REFERENCES [dbo].[Mailings] ([TrackingNumber]),
    CONSTRAINT [FK_Tracking_PostOffices] FOREIGN KEY ([Index]) REFERENCES [dbo].[PostOffices] ([Index])
);

CREATE TABLE [dbo].[Users] (
    [EMail]       NVARCHAR (50)   NOT NULL,
    [MD5Password] VARBINARY (32) NOT NULL,
    [FirstName]   NVARCHAR (20)   NOT NULL,
    [MiddleName]  NVARCHAR (20)   NULL,
    [LastName]    NVARCHAR (20)   NOT NULL,
    [Address]     NVARCHAR (100)  NULL,
    [Admin]       BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([EMail] ASC),
    CONSTRAINT [CK_Users_Column] CHECK ([EMail] like '%_@_%_.__%')
);