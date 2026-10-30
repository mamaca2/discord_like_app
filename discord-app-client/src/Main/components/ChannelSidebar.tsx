import type { FriendDto } from "../../services/friendsService";
import styles from "./ChatArea.module.css";

interface ChatAreaProps {
  friend: FriendDto | null;
}

export default function ChatArea({ friend }: ChatAreaProps) {
  // Nothing selected: just an empty bar
  if (!friend) {
    return <div className={styles.chat} />;
  }

  return (
    <div className={styles.chat}>
      <div className={styles.header}>
        {friend.username}#{friend.tag}
      </div>

      <div className={styles.messages}>
        {/* messages will be rendered here later */}
      </div>

      <div className={styles.inputBar}>
        <input
          className={styles.input}
          placeholder={`Message @${friend.username}`}
        />
      </div>
    </div>
  );
}