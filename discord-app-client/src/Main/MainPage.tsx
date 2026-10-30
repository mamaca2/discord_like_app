import { useState, useEffect } from "react";
import { ServerList } from "./components/ServerList";
import FriendsSideBar from "./components/FriendsSideBar";
import ServerSidebar, { type Channel } from "./components/ServerSideBar";
import ChatArea from "./components/ChatArea";
import { getUserServers, type ServerDto } from "../services/serverService";
import { getUserFriends, type FriendDto } from "../services/friendsService";
import styles from "./MainPage.module.css";

interface MainPageProps {
  token: string;
  onLogout: () => void;
}

export default function MainPage({ token, onLogout }: MainPageProps) {
  const [isFriendsActive, setIsFriendsActive] = useState<boolean>(true);
  const [activeServerId, setActiveServerId] = useState<number | null>(null);
  const [activeFriendId, setActiveFriendId] = useState<string | null>(null);
  const [activeChannel, setActiveChannel] = useState<Channel | null>(null);

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
    setIsFriendsActive(true);
    setActiveServerId(null);
    setActiveChannel(null);
  };

  const handleSelectServer = (serverId: number) => {
    setIsFriendsActive(false);
    setActiveServerId(serverId);
    setActiveFriendId(null);
    setActiveChannel(null);
  };

  const activeFriend = friends.find((f) => f.id === activeFriendId) ?? null;
  const activeServer = servers.find((s) => s.id === activeServerId) ?? null;

  const chatTitle = isFriendsActive
    ? activeFriend
      ? `${activeFriend.username}#${activeFriend.tag}`
      : null
    : activeChannel?.name ?? null;

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
        isFriendsActive={isFriendsActive}
        onSelectFriends={handleSelectFriends}
        onSelectServer={handleSelectServer}
      />

      {isFriendsActive ? (
        <FriendsSideBar
          friends={friends}
          activeFriendId={activeFriendId}
          onSelectFriend={setActiveFriendId}
          onAddFriend={() => console.log("open add friend")}
        />
      ) : (
        <ServerSidebar
          serverName={activeServer?.name ?? ""}
          activeChannelId={activeChannel?.id ?? null}
          onSelectChannel={setActiveChannel}
        />
      )}

      <ChatArea title={chatTitle} />
    </div>
  );
}