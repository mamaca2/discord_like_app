import { useState } from "react";
import styles from "../css/SearchBar.module.css";

import {
  searchUsers,
  type UserSearchDto
} from "../../services/userSearchService";

import {
  sendFriendRequest,
  type FriendRequestDto
} from "../../services/friendRequestService";

import type { FriendDto } from "../../services/friendsService";

interface SearchBarProps {
  token: string;
  friends: FriendDto[];
  incomingRequests: FriendRequestDto[];
  outgoingRequests: FriendRequestDto[];

  appendOutgoingFriendRequests: (request: FriendRequestDto) => void;

  onAcceptRequest: (requestId: number) => Promise<void>;
  onDeclineRequest: (requestId: number) => Promise<void>;
  onCancelRequest: (requestId: number) => Promise<void>;
}

export function SearchBar({
  token,
  friends,
  incomingRequests,
  outgoingRequests,
  appendOutgoingFriendRequests,
  onAcceptRequest,
  onDeclineRequest,
  onCancelRequest
}: SearchBarProps) {

  const [query, setQuery] = useState("");
  const [searchResults, setSearchResults] = useState<UserSearchDto[]>([]);
  const [busyUserId, setBusyUserId] = useState<string | null>(null);
  const [error, setError] = useState("");

  const handleSearch = async (query: string) => {
    try {
      setError("");
      const result = await searchUsers(token, query);
      setSearchResults(result.items);
    } catch (error) {
      console.error(error);
      setSearchResults([]);
      setError("Failed to search users");
    }
  };

  const handleFriendRequest = async (receiverId: string) => {
    setBusyUserId(receiverId);

    try {
      const request = await sendFriendRequest(token, receiverId);
      appendOutgoingFriendRequests(request);
    } catch (error) {
      console.error(error);
      setError("Failed to send friend request");
    } finally {
      setBusyUserId(null);
    }
  };

  const handleAction = async (
    userId: string,
    action: () => Promise<void>
  ) => {
    setBusyUserId(userId);

    try {
      setError("");
      await action();
    } catch (error) {
      console.error(error);
      setError("Failed to update friend request");
    } finally {
      setBusyUserId(null);
    }
  };

  return (
    <>
      <form
        onSubmit={(e) => {
          e.preventDefault();
          handleSearch(query);
        }}
      >
        <input
          className={styles.searchInput}
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          placeholder="Search users..."
        />
      </form>

      {error && <p role="alert">{error}</p>}

      <div className={styles.searchResults}>
        {searchResults.map((user) => {
          const incoming = incomingRequests.find(
            request => request.senderId === user.id
          );

          const outgoing = outgoingRequests.find(
            request => request.receiverId === user.id
          );

          const isFriend = friends.some(
            friend => friend.id === user.id
          );

          const isBusy = busyUserId === user.id;

          return (
            <div key={user.id} className={styles.userResult}>
              <span>
                {user.userName}#{user.tag}
              </span>

              {isFriend ? (
                <span className={styles.pendingStatus}>
                  Friends
                </span>
              ) : incoming ? (
                <div className={styles.actions}>
                  <button
                    type="button"
                    className={styles.acceptButton}
                    disabled={isBusy}
                    title="Accept"
                    onClick={() =>
                      handleAction(user.id, () =>
                        onAcceptRequest(incoming.id)
                      )
                    }
                  >
                    ✓
                  </button>

                  <button
                    type="button"
                    className={styles.declineButton}
                    disabled={isBusy}
                    title="Decline"
                    onClick={() =>
                      handleAction(user.id, () =>
                        onDeclineRequest(incoming.id)
                      )
                    }
                  >
                    ✕
                  </button>
                </div>
              ) : outgoing ? (
                <div className={styles.actions}>
                  <span className={styles.pendingStatus}>
                    Pending
                  </span>

                  <button
                    type="button"
                    className={styles.declineButton}
                    disabled={isBusy}
                    title="Cancel request"
                    onClick={() =>
                      handleAction(user.id, () =>
                        onCancelRequest(outgoing.id)
                      )
                    }
                  >
                    ✕
                  </button>
                </div>
              ) : (
                <button
                  type="button"
                  className={styles.addFriendButton}
                  disabled={isBusy}
                  onClick={() => handleFriendRequest(user.id)}
                >
                  Add
                </button>
              )}
            </div>
          );
        })}
      </div>
    </>
  );
}