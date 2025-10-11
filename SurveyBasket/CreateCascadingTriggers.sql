---------------------------------------------------------
-- 1. Cascade Soft Delete: Poll → Questions
---------------------------------------------------------
CREATE OR ALTER TRIGGER trg_Poll_CascadeSoftDelete
ON Polls
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT UPDATE(IsDeleted)
        RETURN;

    -- Skip if no Poll actually transitioned from not-deleted → deleted
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
        FROM Questions q
        INNER JOIN inserted i ON q.PollId = i.Id
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
-- 2. Cascade Soft Delete: Question → Answers
---------------------------------------------------------
CREATE OR ALTER TRIGGER trg_Question_CascadeSoftDelete
ON Questions
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
        FROM Answers a
        INNER JOIN inserted i ON a.QuestionId = i.Id
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
-- 3. Cascade Restore: Poll → Questions
---------------------------------------------------------
CREATE OR ALTER TRIGGER trg_Poll_CascadeRestore
ON Polls
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT UPDATE(IsDeleted)
        RETURN;

    -- Skip if no Poll actually transitioned from deleted → restored
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
        FROM Questions q
        INNER JOIN inserted i ON q.PollId = i.Id
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
-- 4. Cascade Restore: Question → Answers
---------------------------------------------------------
CREATE OR ALTER TRIGGER trg_Question_CascadeRestore
ON Questions
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
        FROM Answers a
        INNER JOIN inserted i ON a.QuestionId = i.Id
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
    'trg_Poll_CascadeSoftDelete',
    'trg_Question_CascadeSoftDelete',
    'trg_Poll_CascadeRestore',
    'trg_Question_CascadeRestore'
)
ORDER BY OBJECT_NAME(t.parent_id), t.name;
GO
