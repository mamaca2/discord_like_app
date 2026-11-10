
import { useState, useEffect } from "react";

import { ServerList } from "./components/ServerList";
import FriendsBar from "./components/FriendsBar";
import ChatArea from "./components/ChatArea";
import { SearchBar } from "./components/SearchBar";

import { getUserServers, type ServerDto } from "../services/serverService";
import { getUserFriends, type FriendDto } from "../services/friendsService";
import {
  acceptFriendRequest,
  declineFriendRequest,
  getIncomingFriendRequests,
  getOutgoingFriendRequests,
  type FriendRequestDto
} from "../services/friendRequestService";

import styles from "./css/MainPage.module.css";
import middleStyles from "./css/MiddleBar.module.css";
import { FriendRequests } from "./components/FriendRequests";
import Settings from "./components/Settings";

interface MainPageProps {
  token: string;
  onLogout: () => void;
}

type ActiveBar = "friends" | "server" | "search";

type ActiveContent =
  | { type: "chat"; friendId: string }
  | { type: "friendRequests" }
  | { type: "settings" }
  | { type: "empty" };

export default function MainPage({ token, onLogout }: MainPageProps) {
  const [activeServerId, setActiveServerId] = useState<number | null>(null);
  const [activeBar, setActiveBar] = useState<ActiveBar>("friends");

  const [activeContent, setActiveContent] = useState<ActiveContent>({
    type: "empty"
  });

  const activeFriendId =
    activeContent.type === "chat" ? activeContent.friendId : null;

  const [servers, setServers] = useState<ServerDto[]>([]);
  const [friends, setFriends] = useState<FriendDto[]>([]);

  const [incomingFriendRequests, setIncomingFriendRequests] =
    useState<FriendRequestDto[]>([]);

  const [outgoingFriendRequests, setOutgoingFriendRequests] =
    useState<FriendRequestDto[]>([]);

  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let isMounted = true;

    async function fetchData() {
      try {
        setIsLoading(true);
        setError(null);

        const [
          userServers,
          userFriends,
          incoming,
          outgoing
        ] = await Promise.all([
          getUserServers(token),
          getUserFriends(token),
          getIncomingFriendRequests(token),
          getOutgoingFriendRequests(token)
        ]);

        if (isMounted) {
          setServers(userServers);
          setFriends(userFriends);
          setIncomingFriendRequests(incoming);
          setOutgoingFriendRequests(outgoing);
        }
      } catch (error) {
        if (isMounted) {
          setError(
            error instanceof Error ? error.message : "Failed to load data"
          );
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
    setActiveBar("friends");
    setActiveServerId(null);
  };

  const handleSelectServer = (serverId: number) => {
    setActiveServerId(serverId);
    setActiveBar("server");
    setActiveContent({ type: "empty" });
  };

  const handleSelectFriend = (friendId: string) => {
    setActiveBar("friends");
    setActiveServerId(null);
    setActiveContent({ type: "chat", friendId });
  };

  const handleAddFriendClicked = () => {
    setActiveBar("search");
  };

  const handleShowingFriendRequests = () => {
    setActiveContent({ type: "friendRequests" });
  };

  const handleServerIconClicked = () => {
    setActiveContent({ type: "settings" });
  };

  const appendOutgoingFriendRequests = (request: FriendRequestDto) => {
    setOutgoingFriendRequests(prev => [...prev, request]);
  };

  const handleAcceptRequest = async (requestId: number) => {
    await acceptFriendRequest(token, requestId);

    setIncomingFriendRequests(prev =>
      prev.filter(request => request.id !== requestId)
    );

    try {
      const updatedFriends = await getUserFriends(token);
      setFriends(updatedFriends);
    } catch (error) {
      console.error("Failed to refresh friends:", error);
    }
  };

  const handleDeclineRequest = async (requestId: number) => {
    await declineFriendRequest(token, requestId);

    setIncomingFriendRequests(prev =>
      prev.filter(request => request.id !== requestId)
    );
  };

  const handleCancelRequest = async (requestId: number) => {
    await declineFriendRequest(token, requestId);

    setOutgoingFriendRequests(prev =>
      prev.filter(request => request.id !== requestId)
    );
  };

  const handleSendMessage = async (friendId: string, content: string) => {
    console.log(friendId, content);
  };

  if (isLoading) {
    return <div className={styles.container}>Loading...</div>;
  }

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
        onSettingsClick={handleServerIconClicked}
      />

      <aside className={middleStyles.middleBar}>
        {activeBar === "friends" && (
          <FriendsBar
            friends={friends}
            activeFriendId={activeFriendId}
            onSelectFriend={handleSelectFriend}
            onAddFriend={handleAddFriendClicked}
            onShowRequests={handleShowingFriendRequests}
          />
        )}

        {activeBar === "search" && (
          <SearchBar
            token={token}
            friends={friends}
            incomingRequests={incomingFriendRequests}
            outgoingRequests={outgoingFriendRequests}
            appendOutgoingFriendRequests={appendOutgoingFriendRequests}
            onAcceptRequest={handleAcceptRequest}
            onDeclineRequest={handleDeclineRequest}
            onCancelRequest={handleCancelRequest}
          />
        )}
      </aside>

      {activeContent.type === "friendRequests" && (
        <FriendRequests
          incomingRequests={incomingFriendRequests}
          outgoingRequests={outgoingFriendRequests}
          onAccept={handleAcceptRequest}
          onDecline={handleDeclineRequest}
        />
      )}

      {activeContent.type === "settings" && (
        <Settings onLogout={onLogout} />
      )}

      {activeContent.type === "chat" && (
        <ChatArea
          friendId={activeContent.friendId}
          token={token}
          onSendMessage={handleSendMessage}
          onProfileClicked={handleSelectFriend}
        />
      )}
    </div>
  );
}
