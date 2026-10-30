import type { FriendDto } from "../../services/friendsService";
import styles from "./FriendsSidebar.module.css";

interface FriendsSidebarProps {
  friends: FriendDto[];
  activeFriendId?: string | null;
  onSelectFriend?: (friendId: string) => void;
  onAddFriend?: () => void;
}

export default function FriendsSideBar({
  friends,
  activeFriendId,
  onSelectFriend,
  onAddFriend,
}: FriendsSidebarProps) {
  return (
    <div className={styles.sidebar}>
      <button type="button" className={styles.addFriendButton} onClick={onAddFriend}>
        + Friends
      </button>

      {friends.map((friend) => (
        <button
          type="button"
          key={friend.id}
          className={`${styles.friendButton} ${
            activeFriendId === friend.id ? styles.active : ""
          }`}
          onClick={() => onSelectFriend?.(friend.id)}
        >
          {friend.username}#{friend.tag}
        </button>
      ))}
    </div>
  );
}