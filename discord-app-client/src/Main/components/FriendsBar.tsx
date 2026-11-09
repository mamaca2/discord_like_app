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
        className={styles.addFriendButton}
        onClick={onAddFriend}
      >
        + Friends
      </button>

      <button
        className={styles.friendButton}
        onClick={onShowRequests}
      >
        Friend Requests
      </button>

      {friends.map((friend) => (
        <button
          key={friend.id}
          className={`${styles.friendButton} ${
            activeFriendId === friend.id ? styles.active : ""
          }`}
          onClick={() => onSelectFriend?.(friend.id)}
        >
          <div className={styles.avatarContainer}>
            {friend.image ? (
              <img
                src={friend.image}
                alt={friend.username}
                className={styles.avatar}
              />
            ) : (
              <div className={styles.defaultAvatar}>
                {friend.username.charAt(0).toUpperCase()}
              </div>
            )}

            <span
              className={`${styles.statusCircle} ${
                friend.status === "Online"
                  ? styles.online
                  : styles.offline
              }`}
            />
          </div>

          <span className={styles.username}>
            {friend.username}#{friend.tag}
          </span>
        </button>
      ))}
    </>
  );
}