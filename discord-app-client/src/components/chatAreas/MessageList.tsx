import styles from "../../css/chatAreaCss/MessageList.module.css";

export interface MessageDto {
  id: number;
  content: string;
  isMine: boolean;
  sentAt?: string | Date;
  inviteCode?: string | null;
}

interface MessageListProps {
  messages: MessageDto[];
  showAvatar?: boolean;
}

export default function MessageList({
  messages,
  showAvatar = false
}: MessageListProps) {
  return (
    <div className={styles.messageList}>
      {messages.map(message => (
        <div
          key={message.id}
          className={`${styles.messageRow} ${
            message.isMine ? styles.mine : styles.theirs
          }`}
        >
          {!message.isMine && showAvatar && (
            <div className={styles.messageAvatar}>F</div>
          )}

          <div className={styles.messageStack}>
            {message.inviteCode ? (
              <div className={styles.inviteCard}>
                <h3>Server Invitation</h3>

                <p>You've been invited to join a server!</p>

                <a
                  className={styles.inviteLink}
                  href={`/invite/${encodeURIComponent(message.inviteCode)}`}
                >
                  {`${window.location.origin}/invite/${message.inviteCode}`}
                </a>

                {message.isMine ? (
                  <button
                    type="button"
                    className={styles.joinButton}
                    onClick={() => {
                      const url = `${window.location.origin}/invite/${encodeURIComponent(message.inviteCode!)}`;
                      void navigator.clipboard.writeText(url);
                    }}
                  >
                    Copy Invite Link
                  </button>
                ) : (
                  <a
                    className={styles.joinButton}
                    href={`/invite/${encodeURIComponent(message.inviteCode)}`}
                  >
                    Join Server
                  </a>
                )}
              </div>
            ) : (
              <div className={styles.bubble}>
                {message.content}
              </div>
            )}

            {message.sentAt && (
              <span className={styles.timestamp}>
                {new Date(message.sentAt).toLocaleTimeString([], {
                  hour: "2-digit",
                  minute: "2-digit"
                })}
              </span>
            )}
          </div>
        </div>
      ))}
    </div>
  );
}
