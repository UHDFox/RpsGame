CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "Users" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Email" text NOT NULL,
    "Balance" numeric NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE "GameTransactions" (
    "Id" uuid NOT NULL,
    "SenderId" uuid NOT NULL,
    "ReceiverId" uuid NOT NULL,
    "Amount" numeric NOT NULL,
    "TransactionDate" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_GameTransactions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_GameTransactions_Users_ReceiverId" FOREIGN KEY ("ReceiverId") REFERENCES "Users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_GameTransactions_Users_SenderId" FOREIGN KEY ("SenderId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "MatchHistories" (
    "Id" uuid NOT NULL,
    "HostId" uuid NOT NULL,
    "OpponentId" uuid,
    "WinnerId" uuid,
    "Bet" numeric NOT NULL,
    "StartTime" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_MatchHistories" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_MatchHistories_Users_HostId" FOREIGN KEY ("HostId") REFERENCES "Users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_MatchHistories_Users_OpponentId" FOREIGN KEY ("OpponentId") REFERENCES "Users" ("Id")
);

CREATE INDEX "IX_GameTransactions_ReceiverId" ON "GameTransactions" ("ReceiverId");

CREATE INDEX "IX_GameTransactions_SenderId" ON "GameTransactions" ("SenderId");

CREATE INDEX "IX_MatchHistories_HostId" ON "MatchHistories" ("HostId");

CREATE INDEX "IX_MatchHistories_OpponentId" ON "MatchHistories" ("OpponentId");

CREATE INDEX "IX_Users_Name" ON "Users" ("Name");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250125105908_Initial', '9.0.1');

ALTER TABLE "MatchHistories" DROP COLUMN "WinnerId";

CREATE TYPE match_status AS ENUM ('finished', 'postponed', 'pending');

ALTER TABLE "MatchHistories" ADD "PlayerMoves" text[] NOT NULL DEFAULT ARRAY[]::text[];

ALTER TABLE "MatchHistories" ADD "Status" integer NOT NULL DEFAULT 0;

ALTER TABLE "MatchHistories" ADD "Winner" text;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250125153531_AddMatchStatus', '9.0.1');

COMMIT;

