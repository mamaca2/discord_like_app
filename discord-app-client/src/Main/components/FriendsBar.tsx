import { useState } from "react";
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
  const [imageErrors, setImageErrors] = useState<Record<string, boolean>>({});

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

      {friends.map((friend) => {
        const showDefaultAvatar =
          !friend.image || imageErrors[friend.id];

        return (
          <button
            key={friend.id}
            className={`${styles.friendButton} ${
              activeFriendId === friend.id ? styles.active : ""
            }`}
            onClick={() => onSelectFriend?.(friend.id)}
          >
            <div className={styles.avatarContainer}>
              {showDefaultAvatar ? (
                <div className={styles.defaultAvatar}>
                  {friend.userName.charAt(0).toUpperCase()}
                </div>
              ) : (
                <img
                  src={friend.image}
                  alt={friend.userName}
                  className={styles.avatar}
                  onError={() =>
                    setImageErrors((prev) => ({
                      ...prev,
                      [friend.id]: true,
                    }))
                  }
                />
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
              {friend.userName}
            </span>
          </button>
        );
      })}
    </>
  );
}