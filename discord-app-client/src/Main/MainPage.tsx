import { useState, useEffect, use } from "react";

import { ServerList } from "./components/ServerList";
import FriendsBar from "./components/FriendsBar";
import ServerBar from "./components/ServerBar";
import ChatArea from "./components/ChatArea";
import { SearchBar } from "./components/SearchBar";

import { getUserServers, type ServerDto } from "../services/serverService";
import { getUserFriends, type FriendDto } from "../services/friendsService";

import styles from "./css/MainPage.module.css";
import middleStyles from "./css/MiddleBar.module.css";

interface MainPageProps {
  token: string;
  onLogout: () => void;
}

export default function MainPage({ token, onLogout }: MainPageProps) {
  const [activeServerId, setActiveServerId] = useState<number | null>(null);
  const [activeFriendId, setActiveFriendId] = useState<string | null>(null);
  
  type ActiveBar = "friends" | "server" | "search";
  const [activeBar, setActiveBar] = useState<ActiveBar>("friends");

  const [servers, setServers] = useState<ServerDto[]>([]);
  const [friends, setFriends] = useState<FriendDto[]>([]);

  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);



  useEffect(() => {
    let isMounted = true;

    async function fetchData() {
      try {
        setIsLoading(true);
        setError(null);

        const [userServers, userFriends] = await Promise.all([
          getUserServers(token),
          getUserFriends(token),
        ]);

        if (isMounted) {
          setServers(userServers);
          setFriends(userFriends);
        }
      } catch (err) {
        if (isMounted) {
          setError(err instanceof Error ? err.message : "Failed to load initial data");
        }
      } finally {
        if (isMounted) {
          setIsLoading(false);
        }
      }
    }

    fetchData();

    return () => {
      isMounted = false;
    };
  }, [token]);

  const handleSelectFriends = () => {
    setActiveBar("friends")
    setActiveServerId(null);
  };

  const handleSelectServer = (serverId: number) => {
    setActiveServerId(serverId);
    setActiveFriendId(null);
  };


  const handleAddFriendClicked = () => {
    setActiveBar("search");
  };


  // const chatTitle = isPersonalIconClicked
  //   ? activeFriend
  //     ? `${activeFriend.username}#${activeFriend.tag}`
  //     : null
  //   : activeChannel?.name ?? null;

  if (isLoading) return <div className={styles.container}>Loading...</div>;
  if (error) {
    return (
      <div className={styles.container}>
        <p>{error}</p>
        <button onClick={onLogout}>Log out</button>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <ServerList
        servers={servers}
        activeServerId={activeServerId}
        activeBar={activeBar}
        onSelectFriends={handleSelectFriends}
        onSelectServer={handleSelectServer}
      />

      <aside className={middleStyles.middleBar}>
        {activeBar === "friends" && (
          <FriendsBar
            friends={friends}
            activeFriendId={activeFriendId}
            onSelectFriend={setActiveFriendId}
            onAddFriend={handleAddFriendClicked}
          />
        )}

        {/* {activeBar === "server" && (
          <ServerBar serverName={activeServer?.name ?? ""} />
        )} */}

        {activeBar === "search" && (
          <SearchBar
            token={token}
          />
        )}
      </aside>

      {/*<ChatArea title={chatTitle} />*/}
    </div>
  );
}