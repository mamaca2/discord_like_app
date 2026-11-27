import styles from "../../css/chatAreaCss/MessageList.module.css";

export interface MessageDto {
  id: number;
  content: string;
  isMine: boolean;
  sentAt?: string | Date;
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
            <div className={styles.bubble}>
              {message.content}
            </div>

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
