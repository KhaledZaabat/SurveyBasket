---------------------------------------------------------
-- SOFT DELETE CASCADE TRIGGERS
---------------------------------------------------------

---------------------------------------------------------
-- 1. Survey Soft Delete → Cascade to Questions & Submissions
---------------------------------------------------------
CREATE OR ALTER TRIGGER trg_Survey_CascadeSoftDelete
ON Surveys
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    -- Only proceed if IsDeleted column was updated
    IF NOT UPDATE(IsDeleted)
        RETURN;

    -- Only proceed if at least one survey transitioned from not-deleted to deleted
    IF NOT EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE i.IsDeleted = 1 AND d.IsDeleted = 0
    )
        RETURN;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Soft delete all related SurveyQuestions
        UPDATE q
        SET 
            q.IsDeleted = 1,
            q.DeletedOn = GETUTCDATE(),
            q.DeletedById = i.DeletedById
        FROM SurveyQuestions q
        INNER JOIN inserted i ON q.SurveyId = i.Id
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE 
            i.IsDeleted = 1
            AND d.IsDeleted = 0
            AND q.IsDeleted = 0;

        -- Soft delete all related UserSubmissions
        UPDATE us
        SET 
            us.IsDeleted = 1,
            us.DeletedOn = GETUTCDATE(),
            us.DeletedById = i.DeletedById
        FROM UserSubmissions us
        INNER JOIN inserted i ON us.SurveyId = i.Id
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE 
            i.IsDeleted = 1
            AND d.IsDeleted = 0
            AND us.IsDeleted = 0;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END
GO

---------------------------------------------------------
-- 2. SurveyQuestion Soft Delete → Cascade to Options
---------------------------------------------------------
CREATE OR ALTER TRIGGER trg_SurveyQuestion_CascadeSoftDelete
ON SurveyQuestions
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT UPDATE(IsDeleted)
        RETURN;

    IF NOT EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE i.IsDeleted = 1 AND d.IsDeleted = 0
    )
        RETURN;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Soft delete all related SurveyOptions
        UPDATE so
        SET 
            so.IsDeleted = 1,
            so.DeletedOn = GETUTCDATE(),
            so.DeletedById = i.DeletedById
        FROM SurveyOptions so
        INNER JOIN inserted i ON so.SurveyQuestionId = i.Id
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE 
            i.IsDeleted = 1
            AND d.IsDeleted = 0
            AND so.IsDeleted = 0;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END
GO

---------------------------------------------------------
-- 3. SurveyOption Soft Delete → Cascade to SubmissionDetails
---------------------------------------------------------
CREATE OR ALTER TRIGGER trg_SurveyOption_CascadeSoftDelete
ON SurveyOptions
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT UPDATE(IsDeleted)
        RETURN;

    IF NOT EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE i.IsDeleted = 1 AND d.IsDeleted = 0
    )
        RETURN;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Soft delete all related SubmissionDetails
        UPDATE sd
        SET 
            sd.IsDeleted = 1,
            sd.DeletedOn = GETUTCDATE(),
            sd.DeletedById = i.DeletedById
        FROM SubmissionDetails sd
        INNER JOIN inserted i ON sd.OptionId = i.Id
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE 
            i.IsDeleted = 1
            AND d.IsDeleted = 0
            AND sd.IsDeleted = 0;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END
GO

---------------------------------------------------------
-- 4. UserSubmission Soft Delete → Cascade to SubmissionDetails
---------------------------------------------------------
CREATE OR ALTER TRIGGER trg_UserSubmission_CascadeSoftDelete
ON UserSubmissions
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT UPDATE(IsDeleted)
        RETURN;

    IF NOT EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE i.IsDeleted = 1 AND d.IsDeleted = 0
    )
        RETURN;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Soft delete all related SubmissionDetails
        UPDATE sd
        SET 
            sd.IsDeleted = 1,
            sd.DeletedOn = GETUTCDATE(),
            sd.DeletedById = i.DeletedById
        FROM SubmissionDetails sd
        INNER JOIN inserted i ON sd.UserSubmissionId = i.Id
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE 
            i.IsDeleted = 1
            AND d.IsDeleted = 0
            AND sd.IsDeleted = 0;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END
GO

---------------------------------------------------------
-- RESTORE CASCADE TRIGGERS
---------------------------------------------------------

---------------------------------------------------------
-- 5. Survey Restore → Cascade to Questions & Submissions
---------------------------------------------------------
CREATE OR ALTER TRIGGER trg_Survey_CascadeRestore
ON Surveys
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT UPDATE(IsDeleted)
        RETURN;

    -- Only proceed if at least one survey transitioned from deleted to restored
    IF NOT EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE i.IsDeleted = 0 AND d.IsDeleted = 1
    )
        RETURN;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Restore all related SurveyQuestions
        UPDATE q
        SET 
            q.IsDeleted = 0,
            q.DeletedOn = NULL,
            q.DeletedById = NULL
        FROM SurveyQuestions q
        INNER JOIN inserted i ON q.SurveyId = i.Id
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE 
            i.IsDeleted = 0
            AND d.IsDeleted = 1
            AND q.IsDeleted = 1;

        -- Restore all related UserSubmissions
        UPDATE us
        SET 
            us.IsDeleted = 0,
            us.DeletedOn = NULL,
            us.DeletedById = NULL
        FROM UserSubmissions us
        INNER JOIN inserted i ON us.SurveyId = i.Id
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE 
            i.IsDeleted = 0
            AND d.IsDeleted = 1
            AND us.IsDeleted = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END
GO

---------------------------------------------------------
-- 6. SurveyQuestion Restore → Cascade to Options
---------------------------------------------------------
CREATE OR ALTER TRIGGER trg_SurveyQuestion_CascadeRestore
ON SurveyQuestions
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT UPDATE(IsDeleted)
        RETURN;

    IF NOT EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE i.IsDeleted = 0 AND d.IsDeleted = 1
    )
        RETURN;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Restore all related SurveyOptions
        UPDATE so
        SET 
            so.IsDeleted = 0,
            so.DeletedOn = NULL,
            so.DeletedById = NULL
        FROM SurveyOptions so
        INNER JOIN inserted i ON so.SurveyQuestionId = i.Id
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE 
            i.IsDeleted = 0
            AND d.IsDeleted = 1
            AND so.IsDeleted = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END
GO

---------------------------------------------------------
-- 7. SurveyOption Restore → Cascade to SubmissionDetails
---------------------------------------------------------
CREATE OR ALTER TRIGGER trg_SurveyOption_CascadeRestore
ON SurveyOptions
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT UPDATE(IsDeleted)
        RETURN;

    IF NOT EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE i.IsDeleted = 0 AND d.IsDeleted = 1
    )
        RETURN;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Restore all related SubmissionDetails
        UPDATE sd
        SET 
            sd.IsDeleted = 0,
            sd.DeletedOn = NULL,
            sd.DeletedById = NULL
        FROM SubmissionDetails sd
        INNER JOIN inserted i ON sd.OptionId = i.Id
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE 
            i.IsDeleted = 0
            AND d.IsDeleted = 1
            AND sd.IsDeleted = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END
GO

---------------------------------------------------------
-- 8. UserSubmission Restore → Cascade to SubmissionDetails
---------------------------------------------------------
CREATE OR ALTER TRIGGER trg_UserSubmission_CascadeRestore
ON UserSubmissions
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT UPDATE(IsDeleted)
        RETURN;

    IF NOT EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE i.IsDeleted = 0 AND d.IsDeleted = 1
    )
        RETURN;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Restore all related SubmissionDetails
        UPDATE sd
        SET 
            sd.IsDeleted = 0,
            sd.DeletedOn = NULL,
            sd.DeletedById = NULL
        FROM SubmissionDetails sd
        INNER JOIN inserted i ON sd.UserSubmissionId = i.Id
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE 
            i.IsDeleted = 0
            AND d.IsDeleted = 1
            AND sd.IsDeleted = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END
GO

