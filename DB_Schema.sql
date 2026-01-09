CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Full_Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL,
    Password_Hash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL,

    Created_On DATETIME NOT NULL,
    Modified_On DATETIME NOT NULL,
    Is_Active BIT NOT NULL,
    Is_Deleted BIT NOT NULL
);

CREATE TABLE Website_Scans (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    User_Id INT NOT NULL,
    Website_Url NVARCHAR(500) NOT NULL,
    Scan_Type NVARCHAR(50) NOT NULL,
    Scan_Status NVARCHAR(20) NOT NULL,
    Performance_Score INT NULL,
    Security_Score INT NULL,
    Code_Quality_Score INT NULL,

    Created_On DATETIME NOT NULL,
    Modified_On DATETIME NULL,
    Is_Active BIT NOT NULL,
    Is_Deleted BIT NOT NULL,

    CONSTRAINT FK_WebsiteScans_Users
        FOREIGN KEY (User_Id) REFERENCES Users(Id)
);

CREATE TABLE Scan_Pages (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Scan_Id INT NOT NULL,
    Page_Url NVARCHAR(500) NOT NULL,
    Http_Status_Code INT NULL,
    Page_Load_Time_MS INT NULL,

    Created_On DATETIME NOT NULL,
    Modified_On DATETIME NULL,
    Is_Active BIT NOT NULL,
    Is_Deleted BIT NOT NULL,

    CONSTRAINT FK_ScanPages_WebsiteScans
        FOREIGN KEY (Scan_Id) REFERENCES Website_Scans(Id)
);

CREATE TABLE Automated_Issues (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Scan_Id INT NOT NULL,
    Page_Id INT NULL,
    Issue_Category NVARCHAR(50) NOT NULL,
    Issue_Title NVARCHAR(200) NOT NULL,
    Issue_Description NVARCHAR(MAX) NULL,
    Severity NVARCHAR(20) NOT NULL,
    AI_Explanation NVARCHAR(MAX) NULL,
    Suggested_Fix NVARCHAR(MAX) NULL,

    Created_On DATETIME NOT NULL,
    Modified_On DATETIME NULL,
    Is_Active BIT NOT NULL,
    Is_Deleted BIT NOT NULL,

    CONSTRAINT FK_AutoIssues_WebsiteScans
        FOREIGN KEY (Scan_Id) REFERENCES Website_Scans(Id),

    CONSTRAINT FK_AutoIssues_ScanPages
        FOREIGN KEY (Page_Id) REFERENCES Scan_Pages(Id)
);

CREATE TABLE Manual_Bugs (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Scan_Id INT NOT NULL,
    Page_Url NVARCHAR(500) NOT NULL,
    Bug_Title NVARCHAR(200) NOT NULL,
    Bug_Description NVARCHAR(MAX) NULL,
    Severity NVARCHAR(20) NOT NULL,
    Screenshot_Path NVARCHAR(500) NULL,
    Reported_By INT NOT NULL,

    Created_On DATETIME NOT NULL,
    Modified_On DATETIME NULL,
    Is_Active BIT NOT NULL,
    Is_Deleted BIT NOT NULL,

    CONSTRAINT FK_ManualBugs_WebsiteScans
        FOREIGN KEY (Scan_Id) REFERENCES Website_Scans(Id),

    CONSTRAINT FK_ManualBugs_Users
        FOREIGN KEY (Reported_By) REFERENCES Users(Id)
);

CREATE TABLE Security_Headers (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Scan_Id INT NOT NULL,
    Header_Name NVARCHAR(100) NOT NULL,
    Header_Value NVARCHAR(500) NULL,
    Status NVARCHAR(20) NOT NULL,
    Risk_Level NVARCHAR(20) NOT NULL,

    Created_On DATETIME NOT NULL,
    Modified_On DATETIME NULL,
    Is_Active BIT NOT NULL,
    Is_Deleted BIT NOT NULL,

    CONSTRAINT FK_SecurityHeaders_WebsiteScans
        FOREIGN KEY (Scan_Id) REFERENCES Website_Scans(Id)
);

CREATE TABLE Performance_Metrics (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Scan_Id INT NOT NULL,
    Page_Url NVARCHAR(500) NULL,
    Load_Time_MS INT NULL,
    Page_Size_KB INT NULL,
    Total_Requests INT NULL,

    Created_On DATETIME NOT NULL,
    Modified_On DATETIME NULL,
    Is_Active BIT NOT NULL,
    Is_Deleted BIT NOT NULL,

    CONSTRAINT FK_PerformanceMetrics_WebsiteScans
        FOREIGN KEY (Scan_Id) REFERENCES Website_Scans(Id)
);

CREATE TABLE Reports (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Scan_Id INT NOT NULL,
    Report_Type NVARCHAR(20) NOT NULL,
    File_Path NVARCHAR(500) NOT NULL,

    Created_On DATETIME NOT NULL,
    Modified_On DATETIME NULL,
    Is_Active BIT NOT NULL,
    Is_Deleted BIT NOT NULL,

    CONSTRAINT FK_Reports_WebsiteScans
        FOREIGN KEY (Scan_Id) REFERENCES Website_Scans(Id)
);

CREATE TABLE AI_Recommendations (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Issue_Id INT NOT NULL,
    Recommendation_Text NVARCHAR(MAX) NOT NULL,
    Confidence_Score INT NULL,

    Created_On DATETIME NOT NULL,
    Modified_On DATETIME NULL,
    Is_Active BIT NOT NULL,
    Is_Deleted BIT NOT NULL,

    CONSTRAINT FK_AIRecommendations_AutomatedIssues
        FOREIGN KEY (Issue_Id) REFERENCES Automated_Issues(Id)
);
