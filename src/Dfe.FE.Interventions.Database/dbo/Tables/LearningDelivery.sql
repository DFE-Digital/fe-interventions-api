CREATE TABLE [dbo].[LearningDelivery]
(
  [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
  [LearnerId] UNIQUEIDENTIFIER NOT NULL,
  [AimType] INT NULL,
  [StartDate] DATETIME NULL,
  [PlannedEndDate] DATETIME NULL,
  [ActualEndDate] DATETIME NULL,
  [FundingModel] INT NULL,
  [StandardCode] INT NULL,
  [CompletionStatus] INT NULL,
  [Outcome] INT NULL,
  [OutcomeGrade] NVARCHAR(50) NULL,
  [WithdrawalReason] INT NULL,
  [DeliveryLocationPostcode] VARCHAR(10),

  CONSTRAINT [FK_LearningDelivery_Learner] FOREIGN KEY (LearnerId) REFERENCES [dbo].[Learner](Id)
)
