---------------------------------------------------------
-- 1. Cascade Soft Delete: Survey → SurveyQuestions
---------------------------------------------------------
CREATE OR ALTER TRIGGER trg_Survey_CascadeSoftDelete
ON Surveys
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT UPDATE(IsDeleted)
        RETURN;

    -- Skip if no Survey actually transitioned from not-deleted → deleted
    IF NOT EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE i.IsDeleted = 1 AND d.IsDeleted = 0
    )
        RETURN;

    BEGIN TRY
        BEGIN TRANSACTION;

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

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END
GO

---------------------------------------------------------
-- 2. Cascade Soft Delete: SurveyQuestion → SurveyOptions
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

        UPDATE a
        SET 
            a.IsDeleted = 1,
            a.DeletedOn = GETUTCDATE(),
            a.DeletedById = i.DeletedById
        FROM SurveyOptions a
        INNER JOIN inserted i ON a.SurveyQuestionId = i.Id
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE 
            i.IsDeleted = 1
            AND d.IsDeleted = 0
            AND a.IsDeleted = 0;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END
GO

---------------------------------------------------------
-- 3. Cascade Restore: Survey → SurveyQuestions
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

    -- Skip if no Survey actually transitioned from deleted → restored
    IF NOT EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE i.IsDeleted = 0 AND d.IsDeleted = 1
    )
        RETURN;

    BEGIN TRY
        BEGIN TRANSACTION;

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

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END
GO

---------------------------------------------------------
-- 4. Cascade Restore: SurveyQuestion → SurveyOptions
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

        UPDATE a
        SET 
            a.IsDeleted = 0,
            a.DeletedOn = NULL,
            a.DeletedById = NULL
        FROM SurveyOptions a
        INNER JOIN inserted i ON a.SurveyQuestionId = i.Id
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE 
            i.IsDeleted = 0
            AND d.IsDeleted = 1
            AND a.IsDeleted = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END
GO


---------------------------------------------------------
-- 5. Verify trigger existence
---------------------------------------------------------
SELECT 
    t.name AS TriggerName,
    OBJECT_NAME(t.parent_id) AS TableName,
    t.is_disabled AS IsDisabled
FROM sys.triggers t
WHERE t.name IN (
    'trg_Survey_CascadeSoftDelete',
    'trg_SurveyQuestion_CascadeSoftDelete',
    'trg_Survey_CascadeRestore',
    'trg_SurveyQuestion_CascadeRestore'
)
ORDER BY OBJECT_NAME(t.parent_id), t.name;
GO
