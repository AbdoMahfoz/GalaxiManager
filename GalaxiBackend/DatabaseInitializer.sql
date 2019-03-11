CREATE TABLE [dbo].[Faculties] (
    [facultyid] INT           NOT NULL,
    [name]      VARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([facultyid] ASC)
);

CREATE TABLE [dbo].[Clients] (
    [phoneNumber] CHAR (11)     NOT NULL,
    [name]        VARCHAR (MAX) NOT NULL,
    [facultyid]   INT           NOT NULL,
    [email]       VARCHAR (MAX) NULL,
    [year]        INT           NULL,
    PRIMARY KEY CLUSTERED ([phoneNumber] ASC),
    FOREIGN KEY ([facultyid]) REFERENCES [dbo].[Faculties] ([facultyid])
);

CREATE TABLE [dbo].[CheckInHistory] (
    [Phonenumber]  CHAR (11) NOT NULL,
    [CheckInDate]  DATETIME  NOT NULL,
    [CheckOutDate] DATETIME  NULL,
    [TotalPrice] FLOAT NULL, 
    CONSTRAINT [PK_CheckInHistory] PRIMARY KEY CLUSTERED ([Phonenumber] ASC, [CheckInDate] ASC),
    CONSTRAINT [FK_CheckInHistory] FOREIGN KEY ([Phonenumber]) REFERENCES [dbo].[Clients] ([phoneNumber])
);

CREATE TABLE [dbo].[Payments] (
    [Id]    INT           NOT NULL,
    [name]  VARCHAR (MAX) NOT NULL,
    [price] FLOAT (53)    NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[PaymentsCheckin] (
    [Paymentid]   INT        NOT NULL,
    [Phonenumber] CHAR (11)  NOT NULL,
    [CheckInDate] DATETIME   NOT NULL,
    [PaidPrice]   FLOAT (53) NULL,
    CONSTRAINT [pk_paymentsCheckin] PRIMARY KEY CLUSTERED ([Paymentid] ASC, [Phonenumber] ASC, [CheckInDate] ASC),
    CONSTRAINT [fk_paymentsCheckin_Clients_CheckInHistory] FOREIGN KEY ([Phonenumber], [CheckInDate]) REFERENCES [dbo].[CheckInHistory] ([Phonenumber], [CheckInDate]),
    CONSTRAINT [fk_paymentsCheckin_Payments] FOREIGN KEY ([Paymentid]) REFERENCES [dbo].[Payments] ([Id])
);

