CREATE TABLE [dbo].[DateDimension](
	[DateId] [datetime] NOT NULL,
	[DateName] [varchar](25) NULL,
	[DateNameAbbrev] [varchar](25) NULL,
	[DayOfYear] [int] NULL,
	[CalendarYearBegin] [datetime] NULL,
	[CalendarYearEnd] [datetime] NULL,
	[CalendarYear] [int] NULL,
	[CalendarMonthBegin] [datetime] NULL,
	[CalendarMonthEnd] [datetime] NULL,
	[CalendarMonthNumber] [int] NULL,
	[CalendarMonthName] [varchar](25) NULL,
	[CalendarMonthNameAbbrev] [varchar](25) NULL,
	[FiscalYear] [int] NULL,
	[FiscalYearBegin] [datetime] NULL,
	[FiscalYearEnd] [datetime] NULL,
	[WeekBeginDate] [datetime] NULL,
	[WeekEndDate] [datetime] NULL,
	[IsWeekend] [bit] NULL,
 CONSTRAINT [PK_DateDimension] PRIMARY KEY CLUSTERED 
(
	[DateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
