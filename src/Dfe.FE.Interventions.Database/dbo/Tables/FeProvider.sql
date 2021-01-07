CREATE TABLE [dbo].[FeProvider]
(
  [UKPRN] INT NOT NULL PRIMARY KEY,
  [LegalName] NVARCHAR(255) NOT NULL,
  [Status] NVARCHAR(50) NOT NULL,
  [PrimaryTradingName] NVARCHAR(255),
  [CompanyRegistrationNumber] NVARCHAR(10),
  [LegalAddressLine1] NVARCHAR(255),
  [LegalAddressLine2] NVARCHAR(255),
  [LegalAddressLine3] NVARCHAR(255),
  [LegalAddressLine4] NVARCHAR(255),
  [LegalAddressTown] NVARCHAR(255),
  [LegalAddressCounty] NVARCHAR(255),
  [LegalAddressPostcode] NVARCHAR(255),
)
