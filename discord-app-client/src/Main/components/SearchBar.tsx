import { useState } from "react";
import styles from "../css/SearchBar.module.css";
import { searchUsers, type UserSearchDto } from "../../services/userSearchService";
import { sendFriendRequest, type FriendRequestDto } from "../../services/friendRequestService";

interface SearchBarProps {
  token: string;
  appendOutgoingFriendRequests: (friendRequest: FriendRequestDto) => void;
}

export function SearchBar({ token, appendOutgoingFriendRequests}: SearchBarProps) {
  const [query, setQuery] = useState("");
  const [searchResults, setSearchResults] = useState<UserSearchDto[]>([]);

  const handleSearch = async (query: string) => {
    try {
      const result = await searchUsers(token, query);
      setSearchResults(result.items);
    } catch (error) {
      console.error(error);
      setSearchResults([]);
    }
  };

  const handleFriendRequest = async (receiverId: string) => {
    try {
      const request = await sendFriendRequest(token, receiverId);
      appendOutgoingFriendRequests(request);

      console.log("Friend request sent");
    } catch (error) {
      console.error(error);
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

      <div className={styles.searchResults}>
        {searchResults.map((user) => (
          <div key={user.id} className={styles.userResult}>
            <span>
              {user.userName}#{user.tag}
            </span>

            <button   
              type="button"
              className={styles.addFriendButton}
              onClick={() => handleFriendRequest(user.id)}>
                Add
            </button>
          </div>
        ))}
      </div>
    </>
  );
}