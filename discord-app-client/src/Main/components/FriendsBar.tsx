import type { FriendDto } from "../../services/friendsService";
import styles from "../css/FriendsBar.module.css";

interface FriendsBarProps {
  friends: FriendDto[];
  activeFriendId?: string | null;
  onSelectFriend?: (friendId: string) => void;
  onAddFriend?: () => void;
  onShowRequests?: () => void;
}

export default function FriendsBar({
  friends,
  activeFriendId,
  onSelectFriend,
  onAddFriend,
  onShowRequests,
}: FriendsBarProps) {
  return (
    <>
      <button
        type="button"
        className={styles.addFriendButton}
        onClick={onAddFriend}
      >
        + Friends
      </button>

      <button
        type="button"
        className={styles.friendButton}
        onClick={onShowRequests}
      >
        Friend Requests
      </button>

      <div className={styles.sectionTitle}>DIRECT MESSAGES</div>

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
    </>
  );
}