-- ##################################################################################################################
-- MailBox
-- ##################################################################################################################
CREATE TABLE mailbox
(
    id          SERIAL PRIMARY KEY,
    password    VARCHAR(255) NOT NULL,
    status      VARCHAR(255) NOT NULL,
    mail_weight FLOAT        NOT NULL
);

-- ##################################################################################################################
-- Users
-- ##################################################################################################################
CREATE TABLE users
(
    id         SERIAL PRIMARY KEY,
    email      VARCHAR(255) NOT NULL,
    password   VARCHAR(255) NOT NULL,
    notification BOOLEAN DEFAULT TRUE,
    fcmToken   VARCHAR(255) DEFAULT '',
    id_mailbox INTEGER NOT NULL,
    FOREIGN KEY (id_mailbox) REFERENCES mailbox (id)
);

-- ##################################################################################################################
-- History
-- ##################################################################################################################
CREATE TABLE history
(
    id          SERIAL PRIMARY KEY,
    status      VARCHAR(255) NOT NULL,
    time        TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    mail_weight FLOAT        NOT NULL,
    id_mailbox  INTEGER      NOT NULL,
    FOREIGN KEY (id_mailbox) REFERENCES mailbox (id)
);