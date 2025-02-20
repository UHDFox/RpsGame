﻿CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
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

INSERT INTO "MatchHistories" ("Id", "HostId", "OpponentId", "Bet", "StartTime", "PlayerMoves", "Status", "Winner")
VALUES
  ('0194a90c-807a-75f0-abcf-6024cbcb79da', 'eaed04f7-e210-4064-8ed0-60c08b339e12', '0a05d6a1-6066-4e0e-a472-b839f5588719', 450, '2025-01-27 21:35:56.379 +0300', '{Н,К}', 0, '0a05d6a1-6066-4e0e-a472-b839f5588719');

INSERT INTO "Users" ("Id", "Name", "Email", "Balance")
VALUES
  ('0a05d6a1-6066-4e0e-a472-b839f5588719', 'George1', 'capi@mail.ru', 900),
  ('eaed04f7-e210-4064-8ed0-60c08b339e12', 'Michael2', 'lilcapy@mail.ru', 2100);

COMMIT;

