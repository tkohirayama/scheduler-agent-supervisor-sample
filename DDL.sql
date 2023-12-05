  /****** Object:  Table [ordering].[order]    Script Date: 2023/11/25 20:25:21 ******/
  CREATE SCHEMA [ordering]

  CREATE TABLE [ordering].[order]
  (
    [order_id] [int] NOT NULL,
    [order_detail] [nvarchar](max) NULL
  ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
  GO

  CREATE TABLE [ordering].[ordering_job_state]
  (
    [order_id] [int] NOT NULL,
    [locked_by] [nchar](10) NULL, 
    [complete_by] [datetimeoffset](7) NULL,
    [process_state] [int] NOT NULL,
    [failure_count] [int] NOT NULL
  ) ON [PRIMARY]
  GO