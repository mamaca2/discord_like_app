import { useState } from "react";
import type { ServerChatDto } from "../services/serverService";
import type { FriendDto } from "../services/friendsService";
import styles from "../css/ServerBar.module.css";

interface ServerBarProps {
  chats: ServerChatDto[];
  friends: FriendDto[];
  onAddChat: (name: string) => Promise<void>;
  onSelectChat: (chat: ServerChatDto) => void;
  onInviteFriend: (friendId: string) => Promise<void>;
}

export default function ServerBar({
  chats,
  friends,
  onAddChat,
  onSelectChat,
  onInviteFriend
}: ServerBarProps) {
  const [showChats, setShowChats] = useState(true);
  const [showCreateChat, setShowCreateChat] = useState(false);
  const [chatName, setChatName] = useState("");

  const [showInviteFriends, setShowInviteFriends] = useState(false);
  const [invitingId, setInvitingId] = useState<string | null>(null);
  const [invitedIds, setInvitedIds] = useState<string[]>([]);
  const [inviteError, setInviteError] = useState<string | null>(null);

  const handleCreateChat = async () => {
    if (!chatName.trim()) return;

    try {
      await onAddChat(chatName.trim());
      setChatName("");
      setShowCreateChat(false);
    } catch (error) {
      console.error("Failed to create chat:", error);
    }
  };

  const handleInviteFriend = async (friendId: string) => {
    if (invitingId || invitedIds.includes(friendId)) return;

    setInvitingId(friendId);
    setInviteError(null);

    try {
      await onInviteFriend(friendId);
      setInvitedIds(prev => [...prev, friendId]);
    } catch (error) {
      setInviteError("Failed to invite friend.");
      console.error("Failed to invite friend:", error);
    } finally {
      setInvitingId(null);
    }
  };

  return (
    <div className={styles.serverBar}>
      <button
        type="button"
        className={styles.inviteFriendsButton}
        onClick={() => setShowInviteFriends(prev => !prev)}
      >
        <span>Invite People</span>
        <span>{showInviteFriends ? "⌃" : "⌄"}</span>
      </button>

      {showInviteFriends && (
        <div className={styles.inviteFriendsPanel}>
          <div className={styles.invitePanelHeader}>
            <span>Invite friends</span>
            <button
              type="button"
              className={styles.closeInviteButton}
              onClick={() => setShowInviteFriends(false)}
            >
              ×
            </button>
          </div>

          {inviteError && (
            <p className={styles.inviteError}>{inviteError}</p>
          )}

          {friends.length === 0 ? (
            <p className={styles.noFriends}>No friends found.</p>
          ) : (
            friends.map(friend => {
              const isInvited = invitedIds.includes(friend.id);

              return (
                <div key={friend.id} className={styles.friendRow}>
                  <div className={styles.friendAvatar}>
                    {friend.userName.charAt(0).toUpperCase()}
                  </div>

                  <span className={styles.friendName}>
                    {friend.userName}
                  </span>

                  <button
                    type="button"
                    className={styles.inviteButton}
                    onClick={() => handleInviteFriend(friend.id)}
                    disabled={invitingId !== null || isInvited}
                  >
                    {isInvited
                      ? "Invited"
                      : invitingId === friend.id
                        ? "Sending..."
                        : "Invite"}
                  </button>
                </div>
              );
            })
          )}
        </div>
      )}

      <div className={styles.chatsHeader}>
        <button
          type="button"
          className={styles.toggleChats}
          onClick={() => setShowChats(prev => !prev)}
        >
          <span>{showChats ? "⌄" : "›"}</span>
          <span>CHATS</span>
        </button>

        <button
          type="button"
          className={styles.addChatButton}
          onClick={() => setShowCreateChat(prev => !prev)}
        >
          +
        </button>

        {showCreateChat && (
          <div className={styles.createChatPopup}>
            <input
              type="text"
              placeholder="Chat name"
              value={chatName}
              onChange={e => setChatName(e.target.value)}
            />

            <button
              type="button"
              onClick={handleCreateChat}
              disabled={!chatName.trim()}
            >
              Create
            </button>
          </div>
        )}
      </div>

      {showChats && (
        <div className={styles.chatList}>
          {chats.map(chat => (
            <button
              key={chat.id}
              type="button"
              className={styles.chatButton}
              onClick={() => onSelectChat(chat)}
            >
              <span className={styles.hash}>#</span>
              {chat.name}
            </button>
          ))}
        </div>
      )}
    </div>
  );
}
