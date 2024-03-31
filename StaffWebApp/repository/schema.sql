CREATE TYPE theme AS ENUM ('light', 'dark');

CREATE TABLE sessions (
	sessionId uuid PRIMARY KEY,
	accessToken text NULL
);

CREATE TABLE preferences (
	userId uuid PRIMARY KEY,
	theme theme DEFAULT 'dark'
);