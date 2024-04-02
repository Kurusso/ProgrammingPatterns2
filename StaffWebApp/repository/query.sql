-- name: InsertSession :exec 
INSERT INTO sessions (sessionId, accessToken) VALUES($1, $2);

-- name: GetTokenBySessionId :one
SELECT accessToken FROM sessions 
    WHERE sessionId = $1;

-- name: RemoveSession :exec
DELETE FROM sessions WHERE sessionId = $1;


-- name: GetThemeByUserId :one
SELECT theme FROM preferences
    WHERE userId = $1;

-- name: InitUserPreferences :exec
INSERT INTO preferences (userId) VALUES ($1);

-- name: UpdateUserPreferences :exec
UPDATE preferences SET theme = $2
    WHERE userId = $1;